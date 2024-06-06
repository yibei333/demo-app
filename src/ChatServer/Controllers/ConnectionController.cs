using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers;

public class ConnectionController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public IActionResult Index()
    {
        var data = (from a in ConnectionService.Context join b in UserService.Context on a.Session.Metadata.UserId equals b.Id select new ConnectionViewModel { UserId = a.Session.Metadata.UserId, UserName = b.Name, DeviceId = a.Session.Metadata.DeviceId }).ToList();
        return View(data);
    }

    public IActionResult Disconnect(ConnectionViewModel viewModel)
    {
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult DisconnectPost([Bind("UserId,UserName,DeviceId,Message")] ConnectionViewModel viewModel)
    {
        var session = ConnectionService.Get(x => x.Session.Metadata.UserId == viewModel.UserId && x.Session.Metadata.DeviceId == viewModel.DeviceId);
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