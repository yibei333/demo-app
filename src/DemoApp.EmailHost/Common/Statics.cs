using SharpDevLib.Standard;

namespace DemoApp.EmailHost.Common;

public class Statics
{
    public static readonly string RootDirectory = AppDomain.CurrentDomain.BaseDirectory.CombinePath("data");

    public static readonly string DbDirectory = RootDirectory.CombinePath("db");
    public static readonly string DbFilePath = DbDirectory.CombinePath("database.db");
    public static readonly string DbConnectionString = $"Data Source={DbFilePath}";

    public static readonly string CertDirectory = RootDirectory.CombinePath("cert");
    public static readonly string CertPath = CertDirectory.CombinePath("cert.pfx");
    public static readonly string CertPasswordPath = CertDirectory.CombinePath("password.txt");

    public static readonly string EmailDirectory = RootDirectory.CombinePath("email");

    public static void Init()
    {
        RootDirectory.EnsureDirectoryExist();
        DbDirectory.EnsureDirectoryExist();
        CertDirectory.EnsureDirectoryExist();
        EmailDirectory.EnsureDirectoryExist();
    }
}
