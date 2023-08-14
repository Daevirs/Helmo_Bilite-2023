using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;

namespace HELMo_bilite_2023_MartinDavid.Models;

public class Livraison
{
    public DateTime _heureDechargementAttendu;
    public string ClientId { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    [Required]
    [StringLength(500)]
    [DisplayName("Adresse de chargement")]
    public string Chargement { get; set; }
    [Required]
    [StringLength(500)]
    [DisplayName("Adresse de livraison")]
    public string Dechargement { get; set; }
    [Required]
    [StringLength(100)]
    public string Contenu { get; set; }
    [Required]
    [DisplayName("Heure de chargement")]
    public DateTime HeureChargement { get; set; }
    public DateTime HeureFinLivraison { get; set; }
    [Required]
    [DisplayName("Heure de livraison")]
    public DateTime HeureDechargementAttendu
    {
        get => _heureDechargementAttendu;
        set 
        {
            if(HeureChargement >= value)
            {
                throw new ArgumentException("La date de déchargement doit être plus grande que celle de chargement.");
            }
            _heureDechargementAttendu = value;
        }
    }
    [DisplayName("Status de la livraison")]
    public StatusLivraison Status { get; set; }
    [DisplayName("Motif d'excuse")]
    public int MotifExcuse { get; set; }
    [DisplayName("Chauffeur attribué")]
    public string? IdChauffeur { get; set; }
    [DisplayName("Camion attribué")]
    public string? IdCamion { get; set; }
    public string? Commentaire { get; set; }
}

public enum StatusLivraison
{
    Attente,
    EnCours,
    Livree,
    Echec
}

public class MotifExcuse
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(500)]
    [DisplayName("Motif d'excuse")]
    public string Motif { get; set; }
}