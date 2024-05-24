namespace DemoApp.EmailHost.Hosts.Pop3;

public class ListHandler(IServiceProvider serviceProvider) : BaseHandler<string, IEnumerable<string>>(serviceProvider)
{
    public override IEnumerable<string> Handle(string request)
    {
        var id = Guid.TryParse(request, out var userId) ? userId : Guid.Empty;
        var user = UserRepository.Get(x => x.Id == id) ?? throw new NullReferenceException();
        var details = EmailListRepository.GetMany(x => x.UserId == user.Id && x.Type == 2 && !x.IsDeleted).ToList();
        return details.Select(x => x.EmailId.ToString()).ToList();
    }
}
