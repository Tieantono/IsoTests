using IsoTests.Entities;
using IsoTests.Tests.Fixtures;

namespace IsoTests.Tests.Tests;

public class GenreTests(DbContextFactoryFixture factoryFixture)
        : TransactionalTestBase<IsoDbContext>(factoryFixture.Factory)
{
    [Theory]
    [InlineData(1, "Action")]
    [InlineData(2, "Adventure")]
    [InlineData(3, "RPG")]
    [InlineData(4, "Strategy")]
    [InlineData(5, "Simulation")]
    [InlineData(6, "Sports")]
    [InlineData(7, "Puzzle")]
    public void Should_Initialize_Genre_Correctly(int id, string name)
    {
        var genre = new Genre
        {
            Id = id,
            Name = name
        };

        Assert.Equal(id, genre.Id);
        Assert.Equal(name, genre.Name);
    }
}
