
using DemoApp.EmailHost.Common;
using DemoApp.EmailHost.Hosts.Pop3;
using DemoApp.EmailHost.Hosts.Pop3.Lib;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace DemoApp.EmailHost.Services;

public class Pop3ServerHostedService(IServiceProvider serviceProvider) : IHostedService
{
    readonly IServiceProvider _serviceProvider = serviceProvider.CreateScope().ServiceProvider;
    POP3Listener? _listener;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var cert = new X509Certificate2(Statics.CertPath, File.ReadAllText(Statics.CertPasswordPath));

        _listener = new POP3Listener
        {
            SecureCertificate = cert,
        };

        _listener.Events.OnAuthenticate = new AuthHandler(_serviceProvider).Handle;
        _listener.Events.OnMessageList = new ListHandler(_serviceProvider).Handle;
        _listener.Events.OnMessageRetrieval = new RetrievalHandler(_serviceProvider).Handle;
        _listener.Events.OnMessageDelete = new DeleteHandler(_serviceProvider).Handle;
        _listener.Events.OnError = new ErrorHandler(_serviceProvider).Handle;

        var port = _serviceProvider.GetRequiredService<IConfiguration>().GetValue<int>("Pop3Port");
        if (port <= 0) port = 995;
        _listener.ListenOn(IPAddress.Any, port, true);
        _serviceProvider.GetRequiredService<ILogger<Pop3ServerHostedService>>().LogInformation("pop3 listen on port:{port}", port);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _listener?.Stop();
        await Task.CompletedTask;
    }
}
