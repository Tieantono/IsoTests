using Microsoft.EntityFrameworkCore;

namespace IsoTests.Entities;

public partial class IsoDbContext : DbContext
{
    public IsoDbContext(DbContextOptions<IsoDbContext> options)
        : base(options)
    {
    }

    public DbSet<VideoGame> VideoGames { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<VideoGameGenreMapping> VideoGameGenreMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VideoGame>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired();
            entity.Property(e => e.Description)
                .IsRequired();
            
            entity.HasMany(e => e.Genres)
                .WithMany(g => g.VideoGames)
                .UsingEntity<VideoGameGenreMapping>(
                    r => r.HasOne<Genre>().WithMany().HasForeignKey(vg => vg.GenreId),
                    l => l.HasOne<VideoGame>().WithMany().HasForeignKey(vg => vg.VideoGameId)
                );
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired();
            entity.HasData(
                new Genre { Id = 1, Name = "Action" },
                new Genre { Id = 2, Name = "Adventure" },
                new Genre { Id = 3, Name = "RPG" },
                new Genre { Id = 4, Name = "Strategy" },
                new Genre { Id = 5, Name = "Simulation" },
                new Genre { Id = 6, Name = "Sports" },
                new Genre { Id = 7, Name = "Puzzle" }
            );
        });
    }
}
