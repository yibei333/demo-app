using ChatServer.Services;
using SharpDevLib.Standard;
using System.Text;

namespace ChatServer.TcpHost;

public class TcpConnection
{
    public TcpConnection(ConnectionService connectionService, TcpSession<TcpConnectionMetadata> session)
    {
        ConnectionService = connectionService;
        Session = session;
        Session.Received += Session_Received;
    }

    public ConnectionService ConnectionService { get; }
    public TcpSession<TcpConnectionMetadata> Session { get; }

    void Session_Received(object? sender, TcpSessionDataEventArgs<TcpConnectionMetadata> e)
    {
        var segmentLength = 4;
        var segmentData = e.Bytes[..segmentLength];
        var segment = BitConverter.ToInt32(segmentData);
        if (segment == 1) HandleLogin(e.Bytes[segmentLength..]);
        else if (segment == 2) HandleMessage(e.Bytes[segmentLength..]);
    }

    void HandleLogin(byte[] bytes)
    {
        var content = Encoding.UTF8.GetString(bytes);
        var array = content.SplitToList();
        if (array.Count != 2) return;
        Session.Metadata.UserId = array[0].ToGuid();
        Session.Metadata.DeviceId = array[1].ToGuid();

        SendMessage(UserService.SystemUserId, MessageService.LoginSuccessMessageId);
    }

    void HandleMessage(byte[] bytes)
    {
        var userIdSegmentLength = 36;
        var userIdSegmentData = bytes[..userIdSegmentLength];
        var userId = userIdSegmentData.ToUtf8String().ToGuid();
        var messageId = bytes[userIdSegmentLength..].ToUtf8String().ToGuid();

        var session = ConnectionService.Get(x => x.Session.Metadata.UserId == userId);
        if (session is null) return;

        session.SendMessage(Session.Metadata.UserId, messageId);
    }

    public void SendMessage(Guid fromUserId, Guid messageId)
    {
        var fromUserData = fromUserId.ToString().ToUtf8Bytes();
        var contentBytes = messageId.ToString().ToUtf8Bytes();
        var data = fromUserData.Concat(contentBytes).ToArray();
        Session.Send(data);
    }

    public void Disconnect()
    {
        Session.Dispose();
    }
}
