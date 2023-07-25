using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aydex.SemanticKernel.NL2EF.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Aydex.SemanticKernel.NL2EF.Data.Models;

[Table("genders")]
[EntityTypeConfiguration(typeof(GenderConfiguration))]
public class Gender
{
    public enum Constant
    {
        Female = 1,
        Male = 2
    }

    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = default!;
}
