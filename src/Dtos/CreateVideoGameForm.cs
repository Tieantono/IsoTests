using System.ComponentModel.DataAnnotations;

namespace IsoTests.Dtos;

public record CreateVideoGameForm(
    [Required]
    string Name,

    [Required]
    string Description,

    [Required]
    DateTime ReleasedAt,

    [Required]
    List<int> GenreIds
);
