namespace IsoTests.Entities;

public class Genre
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<VideoGame> VideoGames { get; set; } = [];
}
