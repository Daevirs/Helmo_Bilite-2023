using System.Web;
using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using HELMo_bilite_2023_MartinDavid.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMo_bilite_2023_MartinDavid.Controllers;

public class StatsController : Controller
{
    
    private readonly ClientRepository _client;
    private readonly ChauffeurRepository _chauffeur;
    private readonly DbContextHelmoBilite _context;

    public StatsController(ClientRepository client, ChauffeurRepository chauffeur, DbContextHelmoBilite context)
    {
        _client = client;
        _chauffeur = chauffeur;
        _context = context;
    }
    // GET: Stats
        public async Task<IActionResult> Index(DateTime date, string chercherClient = "", string chercherChauffeur = "")
        {
            int livraisonReussie = 0;
            int nbreLivraison = 0;
            // Récupérer toutes les livraisons qui ont été livrées ou non
            var livraisons = _context.Livraisons.Where(livraison => livraison.Status == StatusLivraison.Livree || livraison.Status == StatusLivraison.Echec);

            // Cas où aucune livraisons
            if (_context.Livraisons == null || !livraisons.Any())
            {
                return emptyViewModel();
            }

            var statsViewModelList = new List<StatistiquesViewModel>();
            if(string.IsNullOrEmpty(chercherClient) && string.IsNullOrEmpty(chercherChauffeur) && date == DateTime.MinValue)
            {
                // Cas où on ne fait aucune recherche
                foreach (Livraison livraison in livraisons)
                {
                    nbreLivraison++;
                    StatistiquesViewModel statsViewModel = createViewModel(livraison, out statsViewModel);
                    statsViewModelList.Add(statsViewModel);
                    if (livraison.Status == StatusLivraison.Livree)
                    {
                        livraisonReussie++;
                    }
                }

                Statistiques(livraisonReussie, nbreLivraison);
                return View(statsViewModelList.ToList());
                // Cas où on spécifie une recherche sur le client
            }
            if(!string.IsNullOrEmpty(chercherClient))
            {
                livraisons = livraisons.Join(_context.Clients,
                d => d.ClientId,
                c => c.Id,
                (d, c) => new { Delivery = d, Client = c })
                .Where(dc => dc.Client.UserName.Contains(chercherClient))
                .Select(dc => dc.Delivery);
                // Cas où on spécifie une recherche sur le chauffeur
            }
            if(!string.IsNullOrEmpty(chercherChauffeur))
            {
                livraisons = livraisons.Join(_context.Chauffeurs,
                d => d.IdChauffeur,
                c => c.Id,
                (d, c) => new { Delivery = d, Driver = c })
                .Where(dc => dc.Driver.UserName.Contains(chercherChauffeur))
                .Select(dc => dc.Delivery);
                // Cas où on spécifie une recherche sur la date (on récupère les livraisons sur 1 jour)
            } if (date != DateTime.MinValue)
            {
                livraisons = livraisons.Where(d => d.HeureFinLivraison < date.AddDays(1) && d.HeureFinLivraison > date);
            }

            if (livraisons == null | !livraisons.Any())
            {
                return emptyViewModel();
            }
            foreach (Livraison livraison in livraisons)
            {
                nbreLivraison++;
                StatistiquesViewModel statsViewModel = createViewModel(livraison, out statsViewModel);
                statsViewModelList.Add(statsViewModel);
                if (livraison.Status == StatusLivraison.Livree)
                {
                    livraisonReussie++;
                }
            }

            Statistiques(livraisonReussie, nbreLivraison);
            return View(statsViewModelList.ToList());
        }

        private void Statistiques(int livraisonReussie, int nbreLivraison)
        {
            ViewBag.Reussie = livraisonReussie;
            ViewBag.Ratee = nbreLivraison - livraisonReussie;
            if(livraisonReussie > 0){ 
                ViewBag.Pourcentage =  (livraisonReussie / (float) nbreLivraison) * 100;
            }
            else
            {
                ViewBag.Pourcentage = 0;
            }
        }

        private IActionResult emptyViewModel()
        {
            var emptyViewModel = new StatistiquesViewModel
            {
                Client = "N/A",
                Chauffeur = "N/A",
                Date = DateTime.UnixEpoch,
                Status = StatusLivraison.Attente
            };
            var emptyViewModelList = new List<StatistiquesViewModel>
                {
                    emptyViewModel
                };
            ViewBag.Reussie = 0;
            ViewBag.Ratee = 0;
            ViewBag.Pourcentage = 0.0;
            return View(emptyViewModelList);
        }

        private StatistiquesViewModel createViewModel(Livraison livraison, out StatistiquesViewModel statsViewModel)
        {
            statsViewModel = new StatistiquesViewModel();
            var client = _client.TrouverClient(livraison.ClientId);
            statsViewModel.Client = client.UserName;

            var driver = _chauffeur.TrouverChauffeur(livraison.IdChauffeur);
            statsViewModel.Chauffeur = driver.UserName;

            statsViewModel.Date = livraison.HeureFinLivraison;
            statsViewModel.Status = livraison.Status;
            return statsViewModel;
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChercherClient()
        {
            var pseudo = HttpContext.Request.Query["ChercherClient"].ToString();
            Console.WriteLine(pseudo);
            var list = await _context.Clients.Where(c => c.UserName.Contains(pseudo)).Select(u => u.UserName)
                .ToListAsync();
            foreach (var VARIABLE in list)
            {
                Console.WriteLine(VARIABLE);
            }
            return Ok(list);
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChercherChauffeur()
        {
            var pseudo = HttpContext.Request.Query["ChercherChauffeur"].ToString();
            Console.WriteLine(pseudo);
            var list = await _context.Chauffeurs.Where(c => c.UserName.Contains(pseudo)).Select(u => u.UserName)
                .ToListAsync();
            foreach (var VARIABLE in list)
            {
                Console.WriteLine(VARIABLE);
            }
            return Ok(list);
        }
}