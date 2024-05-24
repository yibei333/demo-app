using DemoApp.EmailHost.Data.Context;
using DemoApp.EmailHost.Data.Entity;
using DemoApp.EmailHost.Data.Repository;

namespace DemoApp.EmailHost.Hosts.Smtp;

public abstract class SmtpBase
{
    protected SmtpBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        UserRepository = serviceProvider.GetRequiredService<IDataRepository<UserEntity>>();
        EmailListRepository = serviceProvider.GetRequiredService<IDataRepository<EmailListEntity>>();
        EmailRespository = serviceProvider.GetRequiredService<IDataRepository<EmailEntity>>();
        DataContext = serviceProvider.GetRequiredService<DataContext>();
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
    }

    protected IServiceProvider ServiceProvider { get; }
    protected IDataRepository<UserEntity> UserRepository { get; }
    protected IDataRepository<EmailListEntity> EmailListRepository { get; }
    protected IDataRepository<EmailEntity> EmailRespository { get; }
    protected DataContext DataContext { get; }
    protected ILogger Logger { get; }
}
