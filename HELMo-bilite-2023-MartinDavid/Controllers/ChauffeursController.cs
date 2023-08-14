using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using HELMo_bilite_2023_MartinDavid.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMo_bilite_2023_MartinDavid.Controllers;

public class ChauffeursController : Controller
{
    private readonly DbContextHelmoBilite _context;
    private readonly ChauffeurRepository _chauffeur;
    public ChauffeursController(DbContextHelmoBilite context, ChauffeurRepository chauffeur)
    {
        _context = context;
        _chauffeur = chauffeur;
    }
    // GET: Driver
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        return _context.Chauffeurs != null ?
            View(_context.Chauffeurs.ToList()) :
            Problem("Entity set 'DbContextHelmoBilite.Chauffeurs'  is null.");
    }
    
    // GET: Drivers/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Chauffeurs == null)
            {
                return NotFound();
            }
            var chauffeur = _chauffeur.TrouverChauffeur(id);
            if (chauffeur == null)
            {
                return NotFound();
            }
            ViewBag.Error = "";
            var toEdit = new ChauffeurViewModel()
            {
                Id = chauffeur.Id,
                Prenom = chauffeur.Prenom,
                Nom = chauffeur.Nom,
                Matricule = chauffeur.Matricule,
                PermisB = chauffeur.PermisB,
                PermisC = chauffeur.PermisC,
                PermisCE = chauffeur.PermisCE
            };
            return View(toEdit);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("PermisB,PermisC,PermisCE")] ChauffeurViewModel chauffeur)
        {
            if (id.Equals(chauffeur.Id))
            {
                return NotFound();
            }
            ViewBag.Error = "";
            Chauffeur chauffeurActuel = _chauffeur.TrouverChauffeur(id);
            if (chauffeurActuel == null)
            {
                ViewBag.Error = "Le conducteur que vous souhaitez modifier n'existe pas.";
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _chauffeur.ModifierStatusLivraisonPermisRetire(
                        chauffeur.PermisB, chauffeur.PermisC, chauffeur.PermisCE, chauffeurActuel);
                    Chauffeur toSave = _chauffeur.TrouverChauffeur(id);
                    toSave.PermisB = chauffeur.PermisB;
                    toSave.PermisC = chauffeur.PermisC;
                    toSave.PermisCE = chauffeur.PermisCE;
                    _context.Entry(chauffeurActuel).CurrentValues.SetValues(toSave);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chauffeur);
        }
}