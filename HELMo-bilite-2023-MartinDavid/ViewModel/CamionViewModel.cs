using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using HELMo_bilite_2023_MartinDavid.Models;

namespace HELMo_bilite_2023_MartinDavid.ViewModel;

public class CamionViewModel
{
}

public class AjouterCamionViewModel
{
    [Required]
    [RegularExpression("\\d[-][A-Z]{3}[-]\\d{3}", ErrorMessage = "Plaque d'immatriculation invalide, elle ne respecte pas les normes (Ex: 1-ABC-234)")]
    [DisplayName("Plaque d'immatriculation (Ex: 1-ABC-234)")]
    public string Immatriculation { get; set; }
    [Required]
    [StringLength(50)]
    [DefaultValue(0)]
    [DisplayName("Marque")]
    public string MarqueId { get; set; }
    [Required]
    [StringLength(50)]
    [DisplayName("Modèle")]
    public string Modele { get; set; }
    [Required]
    [DefaultValue(0)]
    [DisplayName("License requise")]
    public Permis Type { get; set; }
    [Required]
    [DefaultValue(0)]
    [DisplayName("Charge maximale (en kg)")]
    public float Tonnage { get; set; }
    [DataType(DataType.Upload)]
    [Required]
    public IFormFile? Photo { get; set; }
    [DisplayName("Photo")]
    public string? CheminPhoto { get; set; }
}

public class EditerCamionViewModel
{
    [Required]
    [RegularExpression("\\d[-][A-Z]{3}[-]\\d{3}", ErrorMessage = "Plaque d'immatriculation invalide, elle ne respecte pas les normes (Ex: 1-ABC-234)")]
    [DisplayName("Plaque d'immatriculation (Ex: 1-ABC-234)")]
    public string Immatriculation { get; set; }
    [Required]
    [StringLength(50)]
    [DefaultValue(0)]
    [DisplayName("Marque")]
    public string MarqueId { get; set; }
    [Required]
    [StringLength(50)]
    [DisplayName("Modèle")]
    public string Modele { get; set; }
    [Required]
    [DefaultValue(0)]
    [DisplayName("License requise")]
    public Permis Type { get; set; }
    [Required]
    [DefaultValue(0)]
    [DisplayName("Charge maximale (en kg)")]
    public float Tonnage { get; set; }
    [DataType(DataType.Upload)]
    public IFormFile? Photo { get; set; }
    [DisplayName("Ancienne photo")]
    public string? CheminPhoto { get; set; }
}

public class DetailsCamionViewModel
{
    [RegularExpression("\\d[-][A-Z]{3}[-]\\d{3}", ErrorMessage = "Plaque d'immatriculation invalide, elle ne respecte pas les normes (Ex: 1-ABC-234)")]
    [DisplayName("Plaque d'immatriculation")]
    public string Immatriculation { get; set; }
    [StringLength(50)]
    [DefaultValue(0)]
    [DisplayName("Marque")]
    public string Marque { get; set; }
    [StringLength(50)]
    [DisplayName("Modèle")]
    public string Modele { get; set; }
    [DefaultValue(0)]
    [DisplayName("License requise")]
    public Permis Type { get; set; }
    [DefaultValue(0)]
    [DisplayName("Charge maximale (en kg)")]
    public float Tonnage { get; set; }
    [DisplayName("Photo du camion")]
    public string? Photo { get; set; }
    public bool Utilisation { get; set; }
}