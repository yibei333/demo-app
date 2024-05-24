
using DemoApp.EmailHost.Data.Context;
using DemoApp.EmailHost.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.EmailHost.Services;

public class SeedHostedService(IServiceProvider serviceProvider) : IHostedService
{
    private readonly DataContext _dataContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DataContext>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dataContext.Database.MigrateAsync(cancellationToken);

        if (!_dataContext.Users.Any())
        {
            _dataContext.Users.AddRange(DataSeed.Users);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
