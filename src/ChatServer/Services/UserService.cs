namespace ChatServer.Services;

public class UserService : BaseService<UserEntity>
{
    public static readonly Guid SystemUserId = Guid.NewGuid();

    public override List<UserEntity> SeedData()
    {
        return
        [
            new(){ Id=SystemUserId,Name="system" },
            new(){ Id=Guid.NewGuid(),Name="foo",Password="foo_password" },
            new(){ Id=Guid.NewGuid(),Name="bar",Password="bar_password" },
            new(){ Id=Guid.NewGuid(),Name="baz",Password="baz_password" },
            new(){ Id=Guid.NewGuid(),Name="qux",Password="qux_password" },
        ];
    }
}

public class UserEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
}