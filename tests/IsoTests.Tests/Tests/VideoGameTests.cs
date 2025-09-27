
using System.Collections;
using IsoTests.Controllers;
using IsoTests.Dtos;
using IsoTests.Entities;
using IsoTests.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IsoTests.Tests.Tests;

public class VideoGameTests(DbContextFactoryFixture factoryFixture, ITestOutputHelper output)
        : TransactionalTestBase<IsoDbContext>(factoryFixture.Factory)
{
    [Theory]
    [ClassData(typeof(VideoGameFormFactory))]
    public async Task Can_Add_VideoGame(CreateVideoGameForm newGame)
    {
        var vgController = new VideoGameController(Db);

        var result = await vgController.Create(newGame, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdGameId = Assert.IsType<Guid>(createdResult.Value);

        output.WriteLine("Created Result: {0}", createdResult.Value);

        var createdGame = await Db.VideoGames
            .Include(vg => vg.Genres)
            .FirstOrDefaultAsync(vg => vg.Id == createdGameId, CancellationToken.None);

        Assert.NotNull(createdGame);
        Assert.Equal(newGame.Name, createdGame.Name);
        Assert.Equal(newGame.Description, createdGame.Description);
        Assert.Equal(newGame.ReleasedAt, createdGame.ReleasedAt);
        Assert.Equal(newGame.GenreIds.Count, createdGame.Genres.Count);
        Assert.All(newGame.GenreIds, genreId =>
        Assert.Contains(createdGame.Genres, g => g.Id == genreId));

        var games = await Db.VideoGames
            .Include(vg => vg.Genres)
            .ToListAsync(CancellationToken.None);

        Assert.Contains(games, g => g.Id == createdGameId);
        Assert.Single(games);
    }
}

public class VideoGameFormFactory : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new CreateVideoGameForm(
                Name: "The Legend of Zelda: Breath of the Wild",
                Description: "An open-world adventure game set in the land of Hyrule.",
                ReleasedAt: new DateTime(2017, 3, 3, 0, 0, 0, DateTimeKind.Utc),
                GenreIds: [1]
            )
        };

        yield return new object[]
        {
            new CreateVideoGameForm(
                Name: "God of War",
                Description: "An action-adventure game based on Norse mythology.",
                ReleasedAt: new DateTime(2018, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                GenreIds: [2]
            )
        };

        yield return new object[]
        {
            new CreateVideoGameForm(
                Name: "Minecraft",
                Description: "A sandbox game that allows players to build and explore virtual worlds.",
                ReleasedAt: new DateTime(2011, 11, 18, 0, 0, 0, DateTimeKind.Utc),
                GenreIds: [3]
            )
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}