using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using HELMo_bilite_2023_MartinDavid.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HELMo_bilite_2023_MartinDavid.Controllers
{
    public class CamionsController : Controller
    {
        private readonly DbContextHelmoBilite _context;
        private static IWebHostEnvironment _environment;
        private readonly CamionRepository _camion;

        public CamionsController(DbContextHelmoBilite context,
            CamionRepository camion,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _camion = camion;
        }

        // GET: Camions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            if (_context.Camions == null)
            {
                Problem("Entity set 'DbContextUtilisateurLivraison.Camion'  is null.");
            }
            List<Camion> camions = _context.Camions.ToList();
            var view = new List<DetailsCamionViewModel>();
            foreach (var c in camions)
            {
                view.Add(new DetailsCamionViewModel()
                {
                    Immatriculation = c.Immatriculation,
                    Marque = _camion.TrouverMarque(c.MarqueId).NomMarque,
                    Modele = c.Modele,
                    Type = c.Type,
                    Tonnage = c.Tonnage,
                    Photo = c.Photo,
                    Utilisation = _camion.EstUtilise(c.Immatriculation)
                });
            }

            return View(view);

        }

        // GET: Camions/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Camions == null)
            {
                return NotFound();
            }

            var camion = await _context.Camions
                .FirstOrDefaultAsync(m => m.Immatriculation == id);
            Marque marque = _camion.TrouverMarque(camion.MarqueId);
            var details = new DetailsCamionViewModel()
            {
                Immatriculation = camion.Immatriculation,
                Modele = camion.Modele,
                Marque = marque.NomMarque,
                Type = camion.Type,
                Tonnage = camion.Tonnage,
                Photo = camion.Photo,
                Utilisation = false // valeur inutile dans ce contexte
            };
            if (camion == null)
            {
                return NotFound();
            }

            return View(details);
        }

        public IEnumerable<SelectListItem> GetMarqueCamions()
            => _camion.Marques.Select(marque => new SelectListItem
            {
                Value = marque.Id.ToString(),
                Text = marque.NomMarque
            })
                .AsEnumerable();

        // GET: Camions/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Error = "";
            ViewBag.Marque = GetMarqueCamions();
            return View();
        }

        // POST: Camions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Immatriculation,Modele,MarqueId,Type,Tonnage,Photo")] AjouterCamionViewModel camion)
        {
            ViewBag.Error = "";
            if (ModelState.IsValid)
            {
                if (_camion.TrouverImmatriculation(camion.Immatriculation) != null)
                {   
                    ViewBag.Error = "La plaque d'immatriculation existe déjà en base de données";
                }
                string photo = await sauvegarderPhoto(camion.Photo, camion.Immatriculation);
                Camion sauvegarde = new Camion()
                {
                    Immatriculation = camion.Immatriculation,
                    Modele = camion.Modele,
                    MarqueId = Int32.Parse(camion.MarqueId),
                    Type = camion.Type,
                    Tonnage = camion.Tonnage,
                    Photo = photo
                };
                _context.Add(sauvegarde);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(camion);
        }

        // GET: Camions/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Camions == null)
            {
                return NotFound();
            }

            var camion = _camion.TrouverImmatriculation(id);
            if (camion == null)
            {
                return NotFound();
            }
            ViewBag.Marque = GetMarqueCamions();
            var edit = new EditerCamionViewModel()
            {
                Immatriculation = camion.Immatriculation,
                Modele = camion.Modele,
                MarqueId = camion.MarqueId.ToString(),
                Type = camion.Type,
                Tonnage = camion.Tonnage,
                CheminPhoto = camion.Photo
            };
            return View(edit);
        }

        // POST: Camions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Immatriculation,Modele,MarqueId,Type,Tonnage,Photo")] EditerCamionViewModel camion)
        {
            if (id != camion.Immatriculation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Camion camionActuel = _camion.TrouverImmatriculation(id);
                    string photo;
                    if (camion.Photo != null)
                    {
                        Console.WriteLine("ok");
                        photo = await sauvegarderPhoto(camion.Photo, id);
                    }
                    else
                    {
                        Console.WriteLine("why");
                        photo = camion.CheminPhoto!;
                    }
                    Camion sauvegarde = new()
                    {
                        Immatriculation = camion.Immatriculation,
                        Modele = camion.Modele,
                        MarqueId = Int32.Parse(camion.MarqueId),
                        Type = camion.Type,
                        Tonnage = camion.Tonnage,
                        Photo = photo
                    };
                    _context.Entry(camionActuel).CurrentValues.SetValues(sauvegarde);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CamionExiste(camion.Immatriculation))
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(camion);
        }

        // GET: Camions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Camions == null)
            {
                return NotFound();
            }

            var camion = await _context.Camions
                .FirstOrDefaultAsync(m => m.Immatriculation == id);
            if (camion == null)
            {
                return NotFound();
            }

            return View(camion);
        }

        // POST: Camions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Camions == null)
            {
                return Problem("Entity set 'DbContextUtilisateurLivraison.Camion'  is null.");
            }
            var camion = await _context.Camions.FindAsync(id);
            if (camion != null)
            {
                _context.Camions.Remove(camion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CamionExiste(string id)
        {
          return (_context.Camions?.Any(e => e.Immatriculation == id)).GetValueOrDefault();
        }
        
        public static async Task<string> sauvegarderPhoto(IFormFile photo, string immatriculation)
        {
            var imgPath = Path.Combine("Q210060", "img", "PhotoCamion");
            try
            {
                var webRootPath = _environment.WebRootPath;
                var fileName = GetPath("PhotoCamion", $"{immatriculation}.jpg");
                var filePath = Path.Combine(webRootPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                return Path.Combine("/", fileName);
            }
            catch (IOException copyError)
            {
                Console.WriteLine(copyError.Message);
            }
            return imgPath;
        }
        
        private static string GetPath(string dir, string picture)
        {
            return Path.Combine("Q210060", "img", dir, picture);
        }
    }
}
