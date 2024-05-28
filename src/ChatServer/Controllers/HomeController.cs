using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers;

public class HomeController : BaseController
{
    public HomeController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}
