using DemoApp.EmailHost.Hosts.Pop3.Lib;

namespace DemoApp.EmailHost.Hosts.Pop3;

public class ErrorHandler(IServiceProvider serviceProvider) : BaseVoidHandler<IPOP3ConnectionInfo, Exception>(serviceProvider)
{
    public override void Handle(IPOP3ConnectionInfo request1, Exception request2)
    {
        var ip = request1.ClientIP.ToString();
        ServiceProvider.GetRequiredService<ILogger<ErrorHandler>>().LogError(exception: request2, message: "client ip:{ip}", ip);
    }
}
