using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using HELMo_bilite_2023_MartinDavid.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace HELMo_bilite_2023_MartinDavid.Controllers
{
    public class LivraisonsController : Controller
    {
        private readonly ClientRepository _client;
        private readonly ChauffeurRepository _chauffeur;
        private readonly CamionRepository _camion;
        private readonly LivraisonRepository _livraison;
        private readonly DbContextHelmoBilite _context;

        public LivraisonsController(DbContextHelmoBilite context, ClientRepository client, ChauffeurRepository chauffeur, CamionRepository camion, LivraisonRepository livraison)
        {
            _context = context;
            _client = client;
            _chauffeur = chauffeur;
            _camion = camion;
            _livraison = livraison;
        }
        
        private string GetConnectedUser() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: Livraisons
        [Authorize(Roles = "Client, Chauffeur, Dispatcher, Admin")]
        public async Task<IActionResult> Index()
        {
            if (_context.Livraisons == null)
            {
                Problem("Entity set 'DbContextUtilisateurLivraison.Livraisons'  is null.");
            }
            var role = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Role)?.Value;
            ViewBag.Role = role;
            return View(_livraison.RecupererLivraison(GetConnectedUser(), role).AsEnumerable());
        }
        
        // GET: Livraisons/IndexClient
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexClient(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _client.TrouverClient(id);
            SiegeSocial siege = _client.TrouverSiegeSocialClient(client.SiegeSocialId);
            ViewBag.ClientName = client.NomEntreprise;
            ViewBag.ClientAdress = $"{siege.Rue} {siege.CodePostal} {siege.Pays}, {siege.Pays}";

            var livraisons = _context.Livraisons.Where(livraison => livraison.ClientId == id)
                .OrderByDescending(livraison => livraison.HeureDechargementAttendu).ToList();
            var viewModels = new List<LivraisonsViewModel>();
            if (livraisons.Count == 0)
            {
                viewModels.Add(new LivraisonsViewModel
                    {
                        Chargement = "Aucune livraison",
                        Dechargement = "Aucune livraison",
                        Contenu = "Aucune livraison",
                        HeureChargement = "Aucune livraison",
                        HeureDechargementAttendu = "Aucune livraison",
                        Status = StatusLivraison.Attente,
                        Confiance = false
                    }
                );
                return View(viewModels);
            }
            viewModels = new List<LivraisonsViewModel>();
            foreach (var livraison in livraisons)
            {
                viewModels.Add(new LivraisonsViewModel
                {
                    Contenu = livraison.Contenu,
                    HeureChargement = livraison.HeureChargement.ToString("Le dd MMMM yyyy à H:mm"),
                    HeureDechargementAttendu = livraison.HeureDechargementAttendu.ToString("Le dd MMMM yyyy à H:mm"),
                    Chargement = livraison.Chargement,
                    Dechargement = livraison.Dechargement,
                    Status = livraison.Status,
                    Confiance = client.Confiance,
                    Id = livraison.Id
                });
            }

            return View(viewModels);
        }

        // GET: Livraisons/Details/5
        [Authorize(Roles = "Client, Chauffeur, Dispatcher, Admin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Livraisons == null)
            {
                return NotFound();
            }

            var livraison = await _context.Livraisons.FindAsync(id);
            if (livraison == null)
            {
                return NotFound();
            }
            if(livraison.Status != StatusLivraison.Attente) {
                var camion = _camion.TrouverImmatriculation(livraison.IdCamion);
                var chauffeur = _chauffeur.TrouverChauffeur(livraison.IdChauffeur);
                ViewBag.Camion = $"{camion.Modele} : {camion.Immatriculation}";
                ViewBag.Chauffeur = $"{chauffeur.Prenom} {chauffeur.Nom}";
                if (livraison.Status == StatusLivraison.Echec)
                {
                    ViewBag.Motif = _livraison.RecupererMotif(livraison.MotifExcuse).Motif;
                }
            }
            return View(livraison);
        }

        // GET: Livraisons/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Livraisons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([Bind("Chargement,Dechargement,Contenu,HeureChargement,HeureDechargementAttendu")] AjouterLivraisonViewModel livraison)
        {
            try
            { 
                if (ModelState.IsValid)
                {
                    livraison.HeureFinLivraison = livraison.HeureDechargementAttendu.AddHours(1);
                    _livraison.AjouterLivraison(Mapper.ConvertionVMToLivraison(livraison, GetConnectedUser()));
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException e)
            {
                ViewData["Error"] = e.Message;
            }
            return View(livraison);
        }

        // GET: Livraisons/Edit/5
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Livraisons == null)
            {
                return NotFound();
            }
            var livraison = await _context.Livraisons.FindAsync(id);
            if (livraison == null)
            {
                return NotFound();
            }
            return View(Mapper.ConvertionLivraisonToVM(livraison));
        }

        // POST: Livraisons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Chargement,Dechargement,Contenu,HeureChargement,HeureDechargementAttendu,ClientId")] EditLivraisonViewModel livraison)
        {
            if (id != livraison.Id)
            {
                return NotFound();
            }

            // vérifie si la livraison existe
            var livraisonActuelle = await _context.Livraisons.FindAsync(id);
            if (livraisonActuelle == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _livraison.ModifierLivraison(Mapper.ConvertionVMToLivraisonExistante(livraison, id, GetConnectedUser()));
                }
                catch (ArgumentException e)
                {
                    ViewData["Error"] = e.Message;
                    return View(livraison);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivraisonExiste(livraison.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(livraison);
        }
        
        // GET: Livraisons/Attribuer/5
        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> Attribuer(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Livraison livraison = _livraison.GetLivraisonById(id);
            ViewBag.Camions = CamionDisponibles(livraison);
            ViewBag.Chauffeurs = ChauffeursDisponibles(livraison);
            return View();
        }

        // POST: Livraison/Attribuer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dispatcher")]
        public async Task<IActionResult> Attribuer(string? id, [Bind("Chauffeur, Camion")] AttribuerLivraisonViewModel livraison)
        {
            Livraison LivraisonActuelle = _livraison.GetLivraisonById(id);
            ViewBag.Camions = CamionDisponibles(LivraisonActuelle);
            ViewBag.Chauffeurs = ChauffeursDisponibles(LivraisonActuelle);
            if (ModelState.IsValid)
            {
                Chauffeur chauffeur = _chauffeur.TrouverChauffeur(livraison.Chauffeur);
                Camion camion = _camion.TrouverImmatriculation(livraison.Camion);
                if (chauffeur == null)
                {
                    ViewBag.Error = "Le chauffeur sélectionné n'existe pas";
                    return View(livraison);
                }

                if (camion == null)
                {
                    ViewBag.Error = "Le Camion sélectionné n'existe pas";
                    return View(livraison);
                }

                switch (camion.Type)
                {
                    case Permis.B:
                        if (!chauffeur.PermisB)
                        {
                            ViewBag.Error = "Le chauffeur sélectionné ne possède pas le bon permis (permis nécessaire : B)";
                            return View(livraison);
                        }
                        break;
                    case Permis.C:
                        if (!chauffeur.PermisC && !chauffeur.PermisCE)
                        {
                            ViewBag.Error = "Le chauffeur sélectionné ne possède pas le bon permis (permis nécessaire : C / CE)";
                            return View(livraison);
                        }
                        break;
                    case Permis.CE:
                        if (!chauffeur.PermisCE)
                        {
                            ViewBag.Error = "Le chauffeur sélectionné ne possède pas le bon permis (permis nécessaire : CE)";
                            return View(livraison);
                        }
                        break;
                }

                var livraisonEditee = _context.Livraisons.FirstOrDefault(l => l.Id == id);
                livraisonEditee.IdCamion = camion.Immatriculation;
                livraisonEditee.IdChauffeur = chauffeur.Id;
                livraisonEditee.Status = StatusLivraison.EnCours;
                _livraison.ModifierLivraison(livraisonEditee);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Livraisons/Delete/5
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Livraisons == null)
            {
                return NotFound();
            }

            var livraison = _livraison.GetLivraisonById(id);
            if (livraison == null)
            {
                return NotFound();
            }

            return View(livraison);
        }

        // POST: Livraisons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Livraisons == null)
            {
                return Problem("Entity set 'DbContextUtilisateurLivraison.Livraisons'  is null.");
            }

            var livraison = _livraison.GetLivraisonById(id);
            if (livraison != null)
            {
                _context.Livraisons.Remove(livraison);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Livraisons/Validation/5
        [Authorize(Roles = "Chauffeur")]
        public async Task<IActionResult> Validation(string id)
        {
            if (_context.Livraisons == null)
            {
                return Problem("Entity set 'DbContextUtilisateurLivraison.Livraisons'  is null.");
            }
            var livraison = await _context.Livraisons.FindAsync(id);
            var chauffeur = _chauffeur.TrouverChauffeur(livraison.IdChauffeur);
            return View(Mapper.ConvertionLivraisonToValiderVM(
                livraison,
                $"{chauffeur.Prenom} {chauffeur.Prenom}"));
        }

        // POST: Livraisons/Validation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Chauffeur")]
        public async Task<IActionResult> Validation(string id,
            [Bind("Commentaire, Status")] ValiderLivraisonViewModel valider)
        {
            Livraison livraison = _livraison.GetLivraisonById(id);
            if (livraison == null)
            {
                return NotFound();
            }
            livraison.Commentaire = valider.Commentaire;
            livraison.Status = StatusLivraison.Livree;
            _livraison.ModifierLivraison(livraison);
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Livraison/Echec/5
        [Authorize(Roles = "Chauffeur")]
        public async Task<IActionResult> Echec(string id)
        {
            if (_context.Livraisons == null)
            {
                return Problem("Entity set 'DbContextUtilisateurLivraison.Livraisons'  is null.");
            }
            var livraison = await _context.Livraisons.FindAsync(id);
            var chauffeur = _chauffeur.TrouverChauffeur(livraison.IdChauffeur);
            ViewBag.Motif = MotifsExcuse();
            return View(Mapper.ConvertionLivraisonToEchecVM(
                livraison,
                $"{chauffeur.Prenom} {chauffeur.Nom}"));
        }
        // POST: Livraisons/Echec/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Chauffeur")]
        public async Task<IActionResult> Echec(string id, [Bind("MotifExcuse, Status, Commentaire")] EchecLivraisonViewModel echec)
        {
            Livraison livraison = _livraison.GetLivraisonById(id); 
            if (livraison == null)
            {
                return NotFound();
            }
            livraison.Commentaire = echec.Commentaire;
            livraison.MotifExcuse = Int32.Parse(echec.MotifExcuse);
            livraison.Status = StatusLivraison.Echec;
            _livraison.ModifierLivraison(livraison);
            return RedirectToAction(nameof(Index));
        }
        
        
        // Fonction de récupération de données
        private bool LivraisonExiste(string id)
        {
          return (_context.Livraisons?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private IEnumerable<SelectListItem> ChauffeursDisponibles(Livraison livraison)
        {
            return _chauffeur.ChauffeursDisponibles(livraison.HeureChargement, livraison.HeureFinLivraison)
                .Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = $"{c.Prenom} {c.Nom}"
                })
                .AsEnumerable();
        }

        private IEnumerable<SelectListItem> CamionDisponibles(Livraison livraison)
        {
            return _camion.CamionsDisponibles(livraison.HeureChargement, livraison.HeureFinLivraison)
                .Select(c => new SelectListItem
                {
                    Value = c.Immatriculation,
                    Text = $"{c.Modele} (Permis {c.Type} Requis)"
                })
                .AsEnumerable();
        }

        private IEnumerable<SelectListItem> MotifsExcuse()
        {
            return _livraison.ListeMotifsExcuse()
                .Select(c => new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.Motif
                })
                .AsEnumerable();
        }
    }
}
