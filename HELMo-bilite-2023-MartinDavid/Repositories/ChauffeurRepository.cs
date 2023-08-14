using HELMo_bilite_2023_MartinDavid.Models;

namespace HELMo_bilite_2023_MartinDavid.Repositories;

public class ChauffeurRepository
{
    private readonly DbContextHelmoBilite _context;

    public ChauffeurRepository(DbContextHelmoBilite context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Récupère les chauffeurs disponibles pour une livraison. Un chauffeur n'est pas disponible si
    /// une livraison débute au même moment qu'une de ses livraisons se termine (Problème des DateTime).
    /// </summary>
    /// <param name="debut"></param>
    /// <param name="fin"></param>
    /// <returns></returns>
    public IEnumerable<Chauffeur> ChauffeursDisponibles(DateTime debut, DateTime fin)
    {
        return _context.Chauffeurs
            .Where(chauffeur => _context.Livraisons
                .Count(livraison => livraison.IdChauffeur != null &&
                                    livraison.IdChauffeur == chauffeur.Id &&
                                    !(debut > livraison.HeureFinLivraison || fin < livraison.HeureChargement)
                ) == 0);
    }

    public void ModifierStatusLivraisonPermisRetire(bool permisB, bool permisC, bool permisCE, Chauffeur chauffeur)
    {
        if (chauffeur.PermisB && !permisB)
        {
            ModifierEtatLivraison(Permis.B, chauffeur);
        }
        if (chauffeur.PermisC && !permisC)
        {
            ModifierEtatLivraison(Permis.C, chauffeur);
        }
        if (chauffeur.PermisCE && !permisCE)
        {
            ModifierEtatLivraison(Permis.CE, chauffeur);
        }
    }

    private void ModifierEtatLivraison(Permis permis, Chauffeur chauffeur)
    {
        IEnumerable<Livraison> livraisons = _context.Livraisons
            .Where(livraison => _context.Camions
                                    .Count(camion =>
                                        camion.Immatriculation == livraison.IdCamion &&
                                        camion.Type == permis) > 0 && 
                                livraison.IdChauffeur == chauffeur.Id &&
                                livraison.Status == StatusLivraison.EnCours)
            .AsEnumerable();
        foreach (var item in livraisons)
        {
            var update = item;
            update.Status = StatusLivraison.Attente;
            update.IdCamion = null;
            update.IdChauffeur = null;
            _context.Entry(item).CurrentValues.SetValues(update);
            _context.SaveChangesAsync();
        }
    }

    public Chauffeur TrouverChauffeur(string id) => _context.Chauffeurs.Find(id);
}