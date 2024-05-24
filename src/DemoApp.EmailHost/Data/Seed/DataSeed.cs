using DemoApp.EmailHost.Data.Entity;

namespace DemoApp.EmailHost.Data.Seed;

public class DataSeed
{
    public static List<UserEntity> Users =>
    [
        new("foo","foo_password"),
        new("bar","bar_password"),
        new("baz","baz_password"),
        new("qux","qux_password"),
    ];
}
