using IsoTests.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IsoTests.Tests.Fixtures;

public class DbContextFactoryFixture
{
    public IDbContextFactory<IsoDbContext> Factory { get; }

    public DbContextFactoryFixture()
    {
        var options = new DbContextOptionsBuilder<IsoDbContext>()
            .UseNpgsql("Host=localhost;Database=iso_db;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention()
            .Options;

        Factory = new PooledDbContextFactory<IsoDbContext>(options);
    }
}