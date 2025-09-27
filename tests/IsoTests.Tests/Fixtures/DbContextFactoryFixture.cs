using System.Reflection;
using IsoTests.Entities;
using IsoTests.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

[assembly: AssemblyFixture(typeof(DbContextFactoryFixture))]
[assembly: CaptureConsole]

namespace IsoTests.Tests.Fixtures;

public class DbContextFactoryFixture
{
    public IDbContextFactory<IsoDbContext> Factory { get; }

    public DbContextFactoryFixture()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        ArgumentNullException.ThrowIfNull(path);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connString = configuration.GetConnectionString("Default");

        Console.WriteLine("Connection String: {0}", connString ?? "NULL");

        var options = new DbContextOptionsBuilder<IsoDbContext>()
            .UseNpgsql(connString)
            .UseSnakeCaseNamingConvention()
            .Options;

        Factory = new PooledDbContextFactory<IsoDbContext>(options);

        using var db = Factory.CreateDbContext();
        db.Database.EnsureCreated();
    }
}