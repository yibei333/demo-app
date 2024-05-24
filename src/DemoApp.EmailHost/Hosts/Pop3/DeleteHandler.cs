namespace DemoApp.EmailHost.Hosts.Pop3;

public class DeleteHandler(IServiceProvider serviceProvider) : BaseVoidHandler<string, IList<string>>(serviceProvider)
{
    public override void Handle(string request1, IList<string> request2)
    {
        var user = UserRepository.Get(x => x.Id == Guid.Parse(request1)) ?? throw new NullReferenceException();
        var emailList = EmailListRepository.GetMany(x => x.UserId == user.Id && request2.Select(x => x.ToLower()).Contains(x.EmailId.ToString().ToLower())).ToList();
        emailList.ForEach(x => x.IsDeleted = true);
        EmailListRepository.UpdateRange(emailList);
    }
}
