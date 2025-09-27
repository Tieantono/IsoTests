using System.Threading.Tasks;
using IsoTests.Dtos;
using IsoTests.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IsoTests.Controllers;

[Route("api/video-game")]
[ApiController]
public class VideoGameController(IsoDbContext db) : ControllerBase
{
    private readonly IsoDbContext _db = db;

    [HttpGet]
    public async Task<IActionResult> GetById(CancellationToken ct = default)
    {
        var videoGames = await (
            from vg in _db.VideoGames
            join vggm in _db.VideoGameGenreMappings on vg.Id equals vggm.VideoGameId
            join g in _db.Genres on vggm.GenreId equals g.Id
            select new
            {
                vg.Id,
                vg.Name,
                vg.Description,
                vg.ReleasedAt,
                Genres = vg.Genres.Select(genre => new
                {
                    genre.Id,
                    genre.Name
                }).ToList()
            })
            .ToListAsync(ct);

        return Ok(videoGames);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateVideoGameForm form,
        CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var genres = await _db.Genres
            .Where(g => form.GenreIds.Contains(g.Id))
            .ToListAsync(ct);

        var genreDiffs = form.GenreIds
            .Except(genres.Select(g => g.Id))
            .ToArray();

        if (genres.Count != form.GenreIds.Count)
        {
            return BadRequest(new
            {
                Error = "Some genres do not exist",
                MissingGenreIds = genreDiffs
            });
        }

        var videoGame = new VideoGame
        {
            Id = Guid.NewGuid(),
            Name = form.Name,
            Description = form.Description,
            ReleasedAt = form.ReleasedAt,
            Genres = genres
        };
        _db.VideoGames.Add(videoGame);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), videoGame.Id);
    }
}
