namespace DemoApp.EmailHost.Data.Entity;

public class UserEntity(string name, string password) : BaseEntity
{
    public string Name { get; set; } = name;

    public string Password { get; set; } = password;
}
