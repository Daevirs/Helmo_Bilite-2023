using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.ViewModel;

namespace HELMo_bilite_2023_MartinDavid.Repositories;

public class Mapper
{
    public static Livraison ConvertionVMToLivraison(AjouterLivraisonViewModel viewModel, string clientId)
    {
        return new Livraison
        {
            HeureChargement = viewModel.HeureChargement,
            HeureDechargementAttendu = viewModel.HeureDechargementAttendu,
            HeureFinLivraison = viewModel.HeureDechargementAttendu.AddHours(1),
            Contenu = viewModel.Contenu,
            Chargement = viewModel.Chargement,
            Dechargement = viewModel.Dechargement,
            Status = StatusLivraison.Attente,
            ClientId = clientId
        };
    }

    public static Livraison ConvertionVMToLivraisonExistante(EditLivraisonViewModel viewModel, string livraisonId, string clientId)
    {
        return new Livraison
        {
            Id = livraisonId,
            HeureChargement = viewModel.HeureChargement,
            HeureDechargementAttendu = viewModel.HeureDechargementAttendu,
            HeureFinLivraison = viewModel.HeureDechargementAttendu.AddHours(1),
            Contenu = viewModel.Contenu,
            Chargement = viewModel.Chargement,
            Dechargement = viewModel.Dechargement,
            ClientId = clientId
        };
    }

    public static EditLivraisonViewModel ConvertionLivraisonToVM(Livraison livraison)
    {
        return new EditLivraisonViewModel()
        {
            Id = livraison.Id,
            HeureChargement = livraison.HeureChargement,
            HeureDechargementAttendu = livraison.HeureDechargementAttendu,
            Contenu = livraison.Contenu,
            Chargement = livraison.Chargement,
            Dechargement = livraison.Dechargement,
            ClientId = livraison.ClientId,
            _heureDechargementAttendu = livraison._heureDechargementAttendu
        };
    }
        
    public static LivraisonsViewModel ConvertionLivraisonToIndexVM(Livraison livraison, Client client)
    {
        return new LivraisonsViewModel()
        {
            Id = livraison.Id,
            Contenu = livraison.Contenu,
            HeureChargement = livraison.HeureChargement.ToString("Le dd MMMM yyyy à H:mm"),
            HeureDechargementAttendu = livraison.HeureDechargementAttendu.ToString("Le dd MMMM yyyy à H:mm"),
            Chargement = livraison.Chargement,
            Dechargement = livraison.Dechargement,
            Status = livraison.Status,
            Client = client.NomEntreprise,
            Confiance = client.Confiance,
        };
    }
        
    public static ValiderLivraisonViewModel ConvertionLivraisonToValiderVM(Livraison livraison, string chauffeur)
    {
        return new ValiderLivraisonViewModel
        {
            Chauffeur = chauffeur
        };
    }
        
    public static EchecLivraisonViewModel ConvertionLivraisonToEchecVM(Livraison livraison, string chauffeur)
    {
        return new EchecLivraisonViewModel
        {
            Id = livraison.Id,
            Chauffeur = chauffeur
        };
    }
}