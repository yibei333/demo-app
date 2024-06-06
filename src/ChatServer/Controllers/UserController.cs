using SharpDevLib.Standard;

namespace ChatServer.Controllers;

public class UserController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public Reply<List<IdNameDto>> Get()
    {
        var user = UserService.GetMany(x => true).Select(x => new IdNameDto(x.Id, x.Name)).ToList();
        return Reply.Succeed(user);
    }
}
