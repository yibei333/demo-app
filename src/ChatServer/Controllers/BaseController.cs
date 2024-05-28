using ChatServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers;

public abstract class BaseController : Controller
{
    public BaseController(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Logger = ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
        UserService = ServiceProvider.GetRequiredService<UserService>();
        MessageService = ServiceProvider.GetRequiredService<MessageService>();
    }

    public IServiceProvider ServiceProvider { get; }
    public ILogger Logger { get; }
    public UserService UserService { get; }
    public MessageService MessageService { get; }
}
