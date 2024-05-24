using DemoApp.EmailHost.Common;
using DemoApp.EmailHost.Data.Context;
using DemoApp.EmailHost.Data.Repository;
using DemoApp.EmailHost.Services;
using Microsoft.EntityFrameworkCore;

Statics.Init();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(Statics.DbConnectionString));
builder.Services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));
builder.Services.AddHostedService<SeedHostedService>();

builder.Services.AddHostedService<Pop3ServerHostedService>();
builder.Services.AddHostedService<SmtpServerHostedService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
