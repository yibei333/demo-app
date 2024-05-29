using ChatServer.Services;
using SharpDevLib.Standard;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.TcpHost;

public class TcpSession
{
    public TcpSession(Socket socket, TcpHostedService hostedService)
    {
        Socket = socket;
        HostedService = hostedService;
        Logger = HostedService.ServiceProvider.GetRequiredService<ILogger<TcpSession>>();
    }

    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public Socket Socket { get; set; }
    public TcpHostedService HostedService { get; }
    public ILogger<TcpSession> Logger { get; }

    public void Receive()
    {
        var buffer = new byte[2048];
       
        try
        {
            Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, (result) =>
            {
                try
                {
                    var length = Socket.EndReceive(result);
                    if (length > 0)
                    {
                        HandleData(buffer[..length]);
                        Receive();
                    }
                    else
                    {
                        Disconnect();
                    }
                }
                catch (SocketException ex)
                {
                    Disconnect();
                    Logger.LogError(ex, ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    Receive();
                }
            }, null);
        }
        catch (SocketException ex)
        {
            Disconnect();
            Logger.LogError(ex, ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            Receive();
        }
    }

    void HandleData(byte[] bytes)
    {
        var segmentLength = 4;
        var segmentData = bytes[..segmentLength];
        var segment = BitConverter.ToInt32(segmentData);
        if (segment == 1)
        {
            HandleLogin(bytes[segmentLength..]);
        }
        else if (segment == 2)
        {
            HandleMessage(bytes[segmentLength..]);
        }
    }

    void HandleLogin(byte[] bytes)
    {
        var content = Encoding.UTF8.GetString(bytes);
        var array = content.SplitToList();
        if (array.Count != 2) return;
        this.UserId = array[0].ToGuid();
        this.DeviceId = array[1].ToGuid();

        SendMessage(UserService.SystemUserId, MessageService.LoginSuccessMessageId);
    }

    void HandleMessage(byte[] bytes)
    {
        var userIdSegmentLength = 36;
        var userIdSegmentData = bytes[..userIdSegmentLength];
        var userId = userIdSegmentData.ToUtf8String().ToGuid();
        var messageId = bytes[userIdSegmentLength..].ToUtf8String().ToGuid();

        var session = HostedService.GetSessionByUserId(userId);
        if (session is null) return;

        session.SendMessage(UserId, messageId);
    }

    public void SendMessage(Guid fromUserId, Guid messageId)
    {
        var fromUserData = fromUserId.ToString().ToUtf8Bytes();
        var contentBytes = messageId.ToString().ToUtf8Bytes();
        var data = fromUserData.Concat(contentBytes).ToArray();
        Socket.Send(data);
    }

    public void Disconnect()
    {
        if (Socket.Connected) Socket.Disconnect(false);
        HostedService.RemoveSession(this);
        Socket.Dispose();
    }
}
