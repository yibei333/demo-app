
using System.Net;
using System.Net.Sockets;

namespace ChatServer.TcpHost;

public class TcpHostedService : IHostedService
{
    internal readonly Socket Server;
    readonly List<TcpSession> _connections = [];

    public TcpHostedService(IServiceProvider serviceProvider)
    {
        Server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        Server.Bind(new IPEndPoint(IPAddress.Any, 7654));
        ServiceProvider = serviceProvider.CreateScope().ServiceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Start(cancellationToken);
        await Task.CompletedTask;
    }

    async void Start(CancellationToken cancellationToken)
    {
        await Task.Yield();
        Server.Listen();

        while (true)
        {
            var connection = await Server.AcceptAsync(cancellationToken);
            var session = new TcpSession(connection, this);
            _connections.Add(session);
            session.Receive();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Server.DisconnectAsync(false, cancellationToken);
        Server.Dispose();
    }

    internal TcpSession? GetSessionByUserId(Guid userId)
    {
        return _connections.FirstOrDefault(x => x.UserId == userId);
    }

    internal void RemoveSession(TcpSession session)
    {
        _connections.Remove(session);
        session.Dispose();
    }
}
