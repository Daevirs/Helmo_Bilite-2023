using HELMo_bilite_2023_MartinDavid.Models;
using Microsoft.EntityFrameworkCore;

namespace HELMo_bilite_2023_MartinDavid.Repositories;

public class CamionRepository
{
    private readonly DbContextHelmoBilite _context;
    public IEnumerable<Marque> Marques => _context.Marques; 

    public CamionRepository(DbContextHelmoBilite context)
    {
        _context = context;
    }

    public Camion? TrouverImmatriculation (string immatriculation) => _context.Camions.Find(immatriculation);

    public Marque? TrouverMarque(int id) => _context.Marques.Find(id);

    public IEnumerable<Camion> CamionsDisponibles(DateTime debut, DateTime fin)
    {
        return _context.Camions
            .Where(camions => _context.Livraisons
                .Count(livraison => livraison.IdCamion != null &&
                                    livraison.IdCamion == camions.Immatriculation &&
                                    !(debut > livraison.HeureFinLivraison || fin < livraison.HeureChargement)
                ) == 0);
    }

    public bool EstUtilise(string immatriculation) => _context.Livraisons
        .Any(livraison => livraison.IdCamion == immatriculation && 
                          livraison.Status == StatusLivraison.EnCours);
    
}