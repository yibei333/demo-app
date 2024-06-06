using ChatServer.Services;
using ChatServer.TcpHost;
using SharpDevLib.Standard;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTcp();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<MessageService>();
builder.Services.AddSingleton<ConnectionService>();
builder.Services.AddHostedService<TcpHostedService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

