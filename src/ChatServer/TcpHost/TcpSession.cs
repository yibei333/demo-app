using SharpDevLib.Standard;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.TcpHost;

public class TcpSession
{
    public TcpSession(Socket socket)
    {
        Socket = socket;
    }

    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public Socket Socket { get; set; }

    public async void Receive()
    {
        await Task.Yield();

        var buffer = new byte[2048];

        while (true)
        {
            var length = await Socket.ReceiveAsync(buffer);
            HandleData(buffer[..length]);
        }
    }

    async void HandleData(byte[] bytes)
    {
        await Task.Yield();

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
    }

    void HandleMessage(byte[] bytes)
    {

    }
}
