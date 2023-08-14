using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HELMo_bilite_2023_MartinDavid.Models;

public class SiegeSocial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    [Required]
    [MaxLength(100)]
    [DisplayName("Rue (numéro inclus)")]
    public string Rue { get; set; }
    [Required]
    [MaxLength(100)]
    [DisplayName("Ville")]
    public string Ville { get; set; }
    [Required]
    [MaxLength(4)]
    [DisplayName("Code postal")]
    public string CodePostal { get; set; }
    [Required]
    [MaxLength(100)]
    [DisplayName("Pays")]
    public string Pays { get; set; }
}