using System.Diagnostics;
using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HELMo_bilite_2023_MartinDavid.Repositories;

public class LivraisonRepository
{
    private readonly DbContextHelmoBilite _context;
    private IEnumerable<MotifExcuse> Motifs => _context.MotifExcuses;

    public LivraisonRepository(DbContextHelmoBilite context)
    {
        _context = context;
    }

    public Livraison GetLivraisonById(string id) =>
        _context.Livraisons.Where(livraison => livraison.Id == id).First();

    public IEnumerable<MotifExcuse> ListeMotifsExcuse() => Motifs.ToList();

    public MotifExcuse? RecupererMotif(int MotifExcuse) => Motifs.FirstOrDefault(motif => motif.Id == MotifExcuse);
    
    /// <summary>
    /// Ajoute une livraison. Une erreur est lancée si une autre livraison possède le même Id
    /// </summary>
    /// <param name="livraison"> La livraison à insérer</param>
    /// <exception cref="ArgumentException">Si une autre livraison possède le même Id</exception>
    public void AjouterLivraison(Livraison livraison)
    {
        var existant = _context.Set<Livraison>().Local.FirstOrDefault(l => l.Id == livraison.Id);
        if ( existant == null)
        {
            _context.Add(livraison);
        }
        else
        {
            throw new ArgumentException(
                "Une livraison avec le même Id existe déjà, veuillez contacter l'administrateur");
        }
        _context.SaveChanges();
    }
    
    /// <summary>
    /// Modifie une livraison.
    /// </summary>
    /// <param name="livraison">la livraison à modifier</param>
    /// <exception cref="ArgumentException"></exception>
    public void ModifierLivraison(Livraison livraison)
    {
        var livraisonActuelle = _context.Set<Livraison>().Local.FirstOrDefault(l => l.Id == livraison.Id);
        if (livraisonActuelle == null)
        {
            _context.Update(livraison);
        }
        else 
        {
            _context.Entry(livraisonActuelle).CurrentValues.SetValues(livraison);
        }
        _context.SaveChanges();
    }

    public List<LivraisonsViewModel> RecupererLivraison(string id, string role)
    {
        List<Livraison> livraisons = new List<Livraison>();
        switch (role)
        {
            case "Dispatcher":
                livraisons = _context.Livraisons
                    .Where(d => d.Status != StatusLivraison.Echec || d.Status != StatusLivraison.Livree).OrderByDescending(c => c.HeureChargement).ToList();
                break;
            case "Chauffeur":
                livraisons = _context.Livraisons.Where(c => c.IdChauffeur.Equals(id)).OrderByDescending(c => c.HeureChargement).ToList();
                break;
            case "Client":
                livraisons = _context.Livraisons.Where(c => c.ClientId.Equals(id)).OrderByDescending(c => c.HeureChargement).ToList();
                break;
            case "Admin":
                livraisons = _context.Livraisons.OrderByDescending(c => c.HeureChargement).ToList();
                break;  
        }
        var view = new List<LivraisonsViewModel>();
        foreach (var l in livraisons)
        {
            view.Add(Mapper.ConvertionLivraisonToIndexVM(l, _context.Clients.Find(l.ClientId)));
        }
        return view;
    }
    
}