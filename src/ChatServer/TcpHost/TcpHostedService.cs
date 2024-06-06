
using ChatServer.Services;
using SharpDevLib.Standard;
using System.Net;

namespace ChatServer.TcpHost;

public class TcpHostedService : IHostedService
{
    const int Port = 7654;
    readonly TcpListener<TcpConnectionMetadata> _listener;

    public TcpHostedService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider.CreateScope().ServiceProvider;
        ConnectionService = ServiceProvider.GetRequiredService<ConnectionService>();
        Logger = ServiceProvider.GetRequiredService<ILogger<TcpHostedService>>();
        _listener = ServiceProvider.GetRequiredService<ITcpListenerFactory>().Create(IPAddress.Any, Port, () => new TcpConnectionMetadata());
    }

    public IServiceProvider ServiceProvider { get; }
    public ILogger<TcpHostedService> Logger { get; }
    public ConnectionService ConnectionService { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _listener.SessionAdded += (s, e) =>
        {
            ConnectionService.Add(new TcpConnection(ConnectionService, e.Session));
        };
        _listener.SessionRemoved += (s, e) =>
        {
            var connection = ConnectionService.Get(x => x.Session == e.Session);
            if (connection is not null) ConnectionService.Remove(connection);
        };
        _listener.StateChanged += (s, e) =>
        {
            if(e.Current==TcpListnerStates.Listening) Logger.LogInformation("tcp listen on port:{port}", Port);
        };
        Listen(cancellationToken);
        await Task.CompletedTask;
    }

    async void Listen(CancellationToken cancellationToken)
    {
        await _listener.ListenAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.Dispose();
        await Task.CompletedTask;
    }
}
