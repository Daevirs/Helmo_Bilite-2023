using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HELMo_bilite_2023_MartinDavid.Models;

namespace HELMo_bilite_2023_MartinDavid.ViewModel;

public class LivraisonsViewModel
{
    public string Id { get; set; }
    [DisplayName("Date de chargement")]
    public string HeureChargement { get; set; } 
    
    [DisplayName("Date de déchargement")]
    public string HeureDechargementAttendu { get; set; } 
    
    [StringLength(100)]
    [DisplayName("Description du matériel chargé")]
    public string Contenu { get; set; }
    
    [StringLength(500)]
    [DisplayName("Adresse de chargement")]
    public string Chargement { get; set; }
    
    [StringLength(500)]
    [DisplayName("Adresse de déchargement")]
    public string Dechargement{ get; set; } 
    
    [DisplayName("Status de la livraison")]
    public StatusLivraison Status { get; set; }
    
    // Nom du client qui crée la livraison
    [DisplayName("Client")]
    
    public string Client{ get; set; } 
    
    public bool Confiance { get; set; }
}

public class AjouterLivraisonViewModel
{
    public DateTime _heureDechargementAttendu;
    
    [Required]
    [DisplayName("Date de chargement")]
    public DateTime HeureChargement { get; set; }
    
    [Required]
    [DisplayName("Date de déchargement")]
    public DateTime HeureDechargementAttendu { get => _heureDechargementAttendu;
        set 
        {
            if(HeureChargement >= value)
            {
                throw new ArgumentException("La date de déchargement doit être plus grande que celle de chargement.");
            }
            _heureDechargementAttendu = value;
        }} 
    
    // heure calculée sur base de l'heure de déchargement attendu
    public DateTime HeureFinLivraison{ get; set; } 
    
    [Required]
    [StringLength(100)]
    [DisplayName("Description du matériel chargé")]
    public string Contenu { get; set; }
    
    [Required]
    [StringLength(500)]
    [DisplayName("Adresse de chargement")]
    public string Chargement { get; set; }
    
    [Required]
    [StringLength(500)]
    [DisplayName("Adresse de déchargement")]
    public string Dechargement{ get; set; }
    
    // création de livraison, cette dernière est automatiquement en attente
    public StatusLivraison? Status { get; set; } = StatusLivraison.Attente;
    
    // Id du client qui crée la livraison
    public string? ClientId{ get; set; }
}

public class EditLivraisonViewModel
{
    public DateTime _heureDechargementAttendu;
    public string? Id { get; set; }
    [Required]
    [DisplayName("Date de chargement")]
    public DateTime HeureChargement { get; set; } 
    [Required]
    [DisplayName("Date de déchargement")]
    public DateTime HeureDechargementAttendu { get => _heureDechargementAttendu;
        set 
        {
            if(HeureChargement >= value)
            {
                throw new ArgumentException("La date de déchargement doit être plus grande que celle de chargement.");
            }
            _heureDechargementAttendu = value;
        }}  
    // heure calculée sur base de l'heure de déchargement attendu
    public DateTime? HeureFinLivraison{ get; set; } 
    [Required]
    [StringLength(100)]
    [DisplayName("Description du matériel chargé")]
    public string Contenu { get; set; }
    [Required]
    [StringLength(500)]
    [DisplayName("Adresse de chargement")]
    public string Chargement { get; set; }
    [Required]
    [StringLength(500)]
    [DisplayName("Adresse de déchargement")]
    public string Dechargement{ get; set; } 
    public string? ClientId { get; set; }
}

public class AttribuerLivraisonViewModel
{
    [DisplayName("Camion")]
    [Required]
    public string? Camion { get; set; }
    [DisplayName("Chauffeur")]
    [Required]
    public string? Chauffeur { get; set; }
}

public class ValiderLivraisonViewModel
{
    public string Id { get; set; }
    [DisplayName("Chauffeur")]
    public string Chauffeur { get; set; }
    
    [DisplayName("Commentaire")]
    public string? Commentaire { get; set; }
}

public class EchecLivraisonViewModel
{
    public string Id { get; set; }
    public string Chauffeur { get; set; }

    [DisplayName("Motif d'excuse")]
    [Required]
    public string MotifExcuse { get; set; }
    
    [DisplayName("Commentaire")]
    public string? Commentaire { get; set; }
}