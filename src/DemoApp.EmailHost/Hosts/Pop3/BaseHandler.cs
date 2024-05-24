using DemoApp.EmailHost.Data.Entity;
using DemoApp.EmailHost.Data.Repository;

namespace DemoApp.EmailHost.Hosts.Pop3;

public abstract class BaseHandler(IServiceProvider serviceProvider)
{
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;
    protected IDataRepository<UserEntity> UserRepository { get; } = serviceProvider.GetRequiredService<IDataRepository<UserEntity>>();
    protected IDataRepository<EmailListEntity> EmailListRepository { get; } = serviceProvider.GetRequiredService<IDataRepository<EmailListEntity>>();
    protected IDataRepository<EmailEntity> EmailRepository { get; } = serviceProvider.GetRequiredService<IDataRepository<EmailEntity>>();
}

public abstract class BaseVoidHandler<TRequest>(IServiceProvider serviceProvider) : BaseHandler(serviceProvider)
{
    public abstract void Handle(TRequest request);
}


public abstract class BaseHandler<TRequest, TResponse>(IServiceProvider serviceProvider) : BaseHandler(serviceProvider)
{
    public abstract TResponse Handle(TRequest request);
}

public abstract class BaseVoidHandler<TRequest1, TRequest2>(IServiceProvider serviceProvider) : BaseHandler(serviceProvider)
{
    public abstract void Handle(TRequest1 request1, TRequest2 request2);
}