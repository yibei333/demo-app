using DemoApp.EmailHost.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.EmailHost.Controllers;

public class HomeController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public IActionResult Index()
    {
        var model = new HomeViewModel
        {
            UserCount = _userRepository.GetMany(x => !x.IsDeleted).Count(),
            EmailCount = _emailRepository.GetMany(x => !x.IsDeleted).Count()
        };
        return View(model);
    }
}
