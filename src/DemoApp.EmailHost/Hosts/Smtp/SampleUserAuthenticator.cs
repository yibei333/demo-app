using SmtpServer;
using SmtpServer.Authentication;

namespace DemoApp.EmailHost.Hosts.Smtp;

public class SampleUserAuthenticator(IServiceProvider serviceProvider) : SmtpBase(serviceProvider), IUserAuthenticator
{
    public async Task<bool> AuthenticateAsync(ISessionContext context, string name, string password, CancellationToken token)
    {
        await Task.Yield();
        return UserRepository.Get(x => x.Name == name && x.Password == password) is not null;
    }
}
