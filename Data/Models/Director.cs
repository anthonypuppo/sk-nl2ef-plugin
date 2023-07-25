using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aydex.SemanticKernel.NL2EF.Data.Models;

[Table("directors")]
public class Director
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = default!;

    [Column("gender")]
    [ForeignKey(nameof(Gender))]
    public int GenderId { get; set; }

    public virtual Gender Gender { get; set; } = default!;

    public virtual ICollection<Movie> Movies { get; set; } = default!;
}
