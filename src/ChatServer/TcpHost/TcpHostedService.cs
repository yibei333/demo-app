
using ChatServer.Services;
using System.Net;
using System.Net.Sockets;

namespace ChatServer.TcpHost;

public class TcpHostedService : IHostedService
{
    internal readonly Socket Server;
    const int Port = 7654;

    public TcpHostedService(IServiceProvider serviceProvider)
    {
        Server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        Server.Bind(new IPEndPoint(IPAddress.Any, Port));
        ServiceProvider = serviceProvider.CreateScope().ServiceProvider;
        ConnectionService = ServiceProvider.GetRequiredService<ConnectionService>();
        Logger = ServiceProvider.GetRequiredService<ILogger<TcpHostedService>>();
    }

    public IServiceProvider ServiceProvider { get; }
    public ILogger<TcpHostedService> Logger { get; }
    public ConnectionService ConnectionService { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Server.Listen();
        Logger.LogInformation("tcp listen on port:{port}", Port);
        Accept();
        await Task.CompletedTask;
    }

    void Accept()
    {
        Server.BeginAccept((result) =>
        {
            try
            {
                var connection = Server.EndAccept(result);
                var session = new TcpSession(connection, this);
                ConnectionService.Add(session);
                session.Receive();
                Accept();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }, null);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Server.Dispose();
        await Task.CompletedTask;
    }

    internal TcpSession? GetSessionByUserId(Guid userId)
    {
        return ConnectionService.Get(x => x.UserId == userId);
    }

    internal void RemoveSession(TcpSession session)
    {
        ConnectionService.Remove(session);
    }
}
