
using System.Net;
using System.Net.Sockets;

namespace ChatServer.TcpHost;

public class TcpHostedService : IHostedService
{
    readonly ILogger<TcpHostedService> _logger;
    readonly Socket _server;
    readonly List<TcpSession> _connections = [];

    public TcpHostedService(ILogger<TcpHostedService> logger)
    {
        _logger = logger;
        _server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _server.Bind(new IPEndPoint(IPAddress.Any, 7654));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();
        _server.Listen();

        while (true)
        {
            var connection = await _server.AcceptAsync(cancellationToken);
            var session = new TcpSession(connection);
            _connections.Add(session);
            session.Receive();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _server.DisconnectAsync(false, cancellationToken);
        _server.Dispose();
    }
}
