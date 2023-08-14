using System.ComponentModel.DataAnnotations;

namespace HELMo_bilite_2023_MartinDavid.Models;

public class Marque
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string NomMarque { get; set; }
}