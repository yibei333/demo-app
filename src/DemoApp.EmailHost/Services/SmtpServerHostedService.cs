using DemoApp.EmailHost.Common;
using DemoApp.EmailHost.Hosts.Smtp;
using SmtpServer;
using System.Security.Cryptography.X509Certificates;

namespace DemoApp.EmailHost.Services;

public class SmtpServerHostedService(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    readonly IServiceProvider _serviceProvider = serviceProvider.CreateScope().ServiceProvider;
    readonly IConfiguration _configuration = configuration;
    SmtpServer.SmtpServer? _server;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var port = _configuration.GetValue<int>("SmtpPort");
        if (port <= 0) port = 465;
        var cert = new X509Certificate2(Statics.CertPath, File.ReadAllText(Statics.CertPasswordPath));

        var options = new SmtpServerOptionsBuilder()
                        .ServerName(_configuration.GetValue<string>("Domain"))
                        .Endpoint(builder =>
                        {
                            builder.Port(_configuration.GetValue<int>("SmtpPort"), true).Certificate(cert).SupportedSslProtocols(System.Security.Authentication.SslProtocols.Tls12);
                        })
                        .Build();

        var internalServiceProvider = new SmtpServer.ComponentModel.ServiceProvider();
        internalServiceProvider.Add(new SampleMessageStore(_serviceProvider));
        internalServiceProvider.Add(new SampleMailboxFilter(_serviceProvider));
        internalServiceProvider.Add(new SampleUserAuthenticator(_serviceProvider));

        _server = new SmtpServer.SmtpServer(options, internalServiceProvider);
        Start(cancellationToken);
        _serviceProvider.GetRequiredService<ILogger<SmtpServerHostedService>>().LogInformation("smtp listen on port:{port}", port);
        await Task.CompletedTask;
    }

    async void Start(CancellationToken cancellationToken)
    {
        if (_server is null) return;
        await _server.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _server?.Shutdown();
        await Task.CompletedTask;
    }
}
