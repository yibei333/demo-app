using ChatServer.Services;
using SharpDevLib.Standard;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.TcpHost;

public class TcpSession : IDisposable
{
    public TcpSession(Socket socket, TcpHostedService hostedService)
    {
        Socket = socket;
        HostedService = hostedService;
    }

    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public Socket Socket { get; set; }
    public TcpHostedService HostedService { get; }

    public void Dispose()
    {
        Socket?.Dispose();
    }

    public async void Receive()
    {
        await Task.Yield();
        var buffer = new byte[2048];

        try
        {
            var length = await Socket.ReceiveAsync(buffer);
            HandleData(buffer[..length]);
            Receive();
        }
        catch (SocketException ex)
        {
            HostedService.RemoveSession(this);
            await Console.Out.WriteLineAsync(ex.Message);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
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

        var fromUserData = UserService.SystemUserId.ToString().ToUtf8Bytes();
        var contentBytes = MessageService.LoginSuccessMessageId.ToString().ToUtf8Bytes();
        var data = fromUserData.Concat(contentBytes).ToArray();
        Socket.Send(data);
    }

    void HandleMessage(byte[] bytes)
    {
        var userIdSegmentLength = 16;
        var userIdSegmentData = bytes[..userIdSegmentLength];
        var userId = userIdSegmentData.ToUtf8String().ToGuid();
        var contentBytes = bytes[userIdSegmentLength..];

        var session = HostedService.GetSessionByUserId(userId);
        if (session is null) return;

        var fromUserData = UserId.ToString().ToUtf8Bytes();
        var data = fromUserData.Concat(contentBytes).ToArray();
        session.Socket.Send(data);
    }
}
