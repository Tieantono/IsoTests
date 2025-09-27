namespace IsoTests.Entities;

public class VideoGame
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public DateTime ReleasedAt { get; set; }

    public List<Genre> Genres { get; set; } = [];
}
