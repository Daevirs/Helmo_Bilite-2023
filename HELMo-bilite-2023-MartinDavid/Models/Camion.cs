using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace HELMo_bilite_2023_MartinDavid.Models;

public class Camion
{
    [Key] 
    [StringLength(9)]
    [DisplayName("Plaque d'immatriculation")]
    public string Immatriculation { get; set; }
    [Required]
    [StringLength(50)]
    [ForeignKey("Marque")]
    [DisplayName("Marque")]
    public int MarqueId { get; set; }
    [Required]
    [StringLength(50)]
    [DisplayName("Modèle")]
    public string Modele { get; set; }
    [Required]
    [DisplayName("License requise")]
    public Permis Type { get; set; }
    [Required]
    [DisplayName("Charge maximale (en kg)")]
    public float Tonnage { get; set; }
    [DisplayName("Photo")]
    public string? Photo { get; set; }
}