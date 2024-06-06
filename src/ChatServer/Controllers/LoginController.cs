using Microsoft.AspNetCore.Mvc;
using SharpDevLib.Standard;

namespace ChatServer.Controllers;

public class LoginController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public IActionResult Index(LoginViewModel viewModel)
    {
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Login([Bind("Name,Password")] LoginViewModel viewModel)
    {
        if (viewModel.Name.IsNullOrWhiteSpace())
        {
            viewModel.ErrorMessage = "name required";
            return View(nameof(Index), viewModel);
        }

        if (viewModel.Password.IsNullOrWhiteSpace())
        {
            viewModel.ErrorMessage = "password required";
            return View(nameof(Index), viewModel);
        }

        var user = UserService.Get(x => x.Name == viewModel.Name);
        if (user is null)
        {
            viewModel.ErrorMessage = "user not found";
            return View(nameof(Index), viewModel);
        }

        if (user.Password != viewModel.Password)
        {
            viewModel.ErrorMessage = "password error";
            return View(nameof(Index), viewModel);
        }

        var token = user.Id.ToString().ToUtf8Bytes().Base64Encode();
        return RedirectToAction(nameof(Callback), new TokenModel { Token = token, Name = user.Name });
    }

    public IActionResult Callback(TokenModel viewModel)
    {
        return View(viewModel);
    }
}

public class LoginViewModel
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? ErrorMessage { get; set; }
}

public class TokenModel
{
    public string? Name { get; set; }
    public string? Token { get; set; }
}