using DemoApp.EmailHost.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DemoApp.EmailHost.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CS_AS");
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<EmailEntity> Emails { get; set; }
    public DbSet<EmailListEntity> EmailLists { get; set; }
}

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlite();
        return new DataContext(optionsBuilder.Options);
    }
}
