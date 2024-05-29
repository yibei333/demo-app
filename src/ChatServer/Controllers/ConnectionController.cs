using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers;

public class ConnectionController : BaseController
{
    public ConnectionController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public IActionResult Index()
    {
        var data = (from a in ConnectionService.Context join b in UserService.Context on a.UserId equals b.Id select new ConnectionViewModel { UserId = a.UserId, UserName = b.Name, DeviceId = a.DeviceId }).ToList();
        return View(data);
    }

    public IActionResult Disconnect(ConnectionViewModel viewModel)
    {
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult DisconnectPost([Bind("UserId,UserName,DeviceId,Message")] ConnectionViewModel viewModel)
    {
        var session = ConnectionService.Get(x => x.UserId == viewModel.UserId && x.DeviceId == viewModel.DeviceId);
        session?.Disconnect();
        return RedirectToAction(nameof(Index));
    }
}

public class ConnectionViewModel
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Guid DeviceId { get; set; }
    public string? Message { get; set; }
}