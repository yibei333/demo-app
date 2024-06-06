using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers;

public class HomeController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public IActionResult Index()
    {
        return View();
    }
}
