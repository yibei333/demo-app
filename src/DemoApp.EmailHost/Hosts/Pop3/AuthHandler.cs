using DemoApp.EmailHost.Hosts.Pop3.Lib;

namespace DemoApp.EmailHost.Hosts.Pop3;

public class AuthHandler(IServiceProvider serviceProvider) : BaseVoidHandler<POP3AuthenticationRequest>(serviceProvider)
{
    public override void Handle(POP3AuthenticationRequest request)
    {
        var user = UserRepository.Get(x => x.Name == request.SuppliedUsername && x.Password == request.SuppliedPassword);
        if (user is not null)
        {
            request.AuthMailboxID = user.Id.ToString();
        }
    }
}
