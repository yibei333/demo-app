using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Standard;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChatClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static App Instance { get; private set; } = null!;

    public const string ApiUrl = "http://localhost:5071";
    public static Guid UserId { get; set; }
    public static string? UserName { get; set; }
    public static readonly Guid DeviceId = Guid.NewGuid();

    public IServiceProvider ServiceProvider { get; }

    public App()
    {
        Instance = this;

        IServiceCollection services = new ServiceCollection();
        services.AddHttp();
        ServiceProvider = services.BuildServiceProvider();
    }
}
