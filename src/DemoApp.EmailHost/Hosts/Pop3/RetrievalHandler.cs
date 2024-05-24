using DemoApp.EmailHost.Hosts.Pop3.Lib;

namespace DemoApp.EmailHost.Hosts.Pop3;

public class RetrievalHandler(IServiceProvider serviceProvider) : BaseVoidHandler<POP3MessageRetrievalRequest>(serviceProvider)
{
    public override void Handle(POP3MessageRetrievalRequest request)
    {
        var user = UserRepository.Get(x => x.Id == Guid.Parse(request.AuthMailboxID)) ?? throw new NullReferenceException();
        var emailDetail = EmailListRepository.Get(x => x.UserId == user.Id && x.EmailId == Guid.Parse(request.MessageUniqueID)) ?? throw new NullReferenceException();
        var email = EmailRepository.Get(x => x.Id == emailDetail.EmailId) ?? throw new NullReferenceException();
        request.UseTextFile(email.FilePath, false);
    }
}
