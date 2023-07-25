using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aydex.SemanticKernel.NL2EF.Data.Models;

[Table("movies")]
public class Movie
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = default!;

    [Column("tagline")]
    public string? Tagline { get; set; }

    [Column("budget")]
    public long Budget { get; set; }

    [Column("popularity")]
    public int Popularity { get; set; }

    [Column("release_date")]
    public DateTimeOffset ReleaseDate { get; set; }

    [Column("revenue")]
    public long Revenue { get; set; }

    [Column("vote_average")]
    public double VoteAverage { get; set; }

    [Column("vote_count")]
    public int VoteCount { get; set; }

    [Column("overview")]
    public string? Overview { get; set; }

    [Column("director_id")]
    [ForeignKey(nameof(Director))]
    public int DirectorId { get; set; }

    public virtual Director Director { get; set; } = default!;
}
