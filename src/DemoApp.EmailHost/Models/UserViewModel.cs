namespace DemoApp.EmailHost.Models;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Error { get; set; }
}
