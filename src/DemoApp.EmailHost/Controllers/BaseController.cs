using DemoApp.EmailHost.Data.Context;
using DemoApp.EmailHost.Data.Entity;
using DemoApp.EmailHost.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.EmailHost.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ILogger _logger;
    protected readonly IDataRepository<UserEntity> _userRepository;
    protected readonly IDataRepository<EmailEntity> _emailRepository;
    protected readonly IDataRepository<EmailListEntity> _emailListRepository;
    protected readonly DataContext _dataContext;
    protected readonly IServiceProvider _serviceProvider;

    public BaseController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
        _userRepository = _serviceProvider.GetRequiredService<IDataRepository<UserEntity>>();
        _emailRepository = _serviceProvider.GetRequiredService<IDataRepository<EmailEntity>>();
        _emailListRepository = _serviceProvider.GetRequiredService<IDataRepository<EmailListEntity>>();
        _dataContext = _serviceProvider.GetRequiredService<DataContext>();
    }
}
