using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace HELMo_bilite_2023_MartinDavid.Models;
    
public abstract class Utilisateur : IdentityUser
{
    [Required]
    [StringLength(256)] 
    [EmailAddress]
    public string Email { get; set; }
    
    [StringLength(500)]
    public string? Photo { get; set; }
}

public abstract class Membre : Utilisateur
{
    [Required]
    [RegularExpression("[A-Z]\\d{6}", ErrorMessage = "Votre matricule ne respecte pas la forme \"A123456\", veuillez réessayer")]
    public string Matricule { get; set; }
    [Required]
    [StringLength(50)]
    public string Nom { get; set; }
    [Required]
    [StringLength(50)]
    public string Prenom { get; set; }
    public DateTime? DateNaissance { get; set; }
}

public class Admin : Membre {}

public class Dispatcher : Membre
{
    [Required]
    public Diplome Diplome { get; set; }
}

public class Chauffeur : Membre
{
    [Required] 
    [DisplayName("Permis B")]
    public bool PermisB { get; set; }
    [Required] 
    [DisplayName("Permis C")]
    public bool PermisC { get; set; }
    [Required] 
    [DisplayName("Permis CE")]
    public bool PermisCE { get; set; }
}

public class Client : Utilisateur
{
    [Required]
    [StringLength(500)]
    public string NomEntreprise { get; set; }
    [Required]
    [StringLength(500)]
    public string SiegeSocialId { get; set; }
    [DefaultValue(false)]
    [DisplayName("Mauvais payeur ?")]
    public Boolean Confiance { get; set; }
}