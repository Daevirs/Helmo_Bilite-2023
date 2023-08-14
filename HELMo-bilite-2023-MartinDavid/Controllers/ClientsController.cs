using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using HELMo_bilite_2023_MartinDavid.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HELMo_bilite_2023_MartinDavid.Controllers;

public class ClientsController : Controller
{
    private readonly DbContextHelmoBilite _context;
    private readonly ClientRepository _client;

    public ClientsController(DbContextHelmoBilite context, ClientRepository client)
    {
        _context = context;
        _client = client;
    }

    // GET: Clients
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        return _client.Clients != null
            ? View(_client.Clients
                .Where(client => _context.Livraisons.Count(livraison => client.Id == livraison.ClientId) > 0)
                .ToList())
            : Problem("Entity set 'HelmobiliteDbContext.Drivers'  is null.");
    }

    // GET: Clients/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
    {
        if (_context.Chauffeurs == null)
        {
            return NotFound();
        }

        var client = _client.TrouverClient(id);
        if (client == null)
        {
            return NotFound();
        }

        ViewBag.Error = "";
        var siege = _client.TrouverSiegeSocialClient(client.SiegeSocialId);
        var toEdit = new ClientsViewModel
        {
            NomEntreprise = client.NomEntreprise,
            Siege = $"{siege.Rue} {siege.CodePostal} {siege.Ville}, {siege.Pays}",
            Email = client.Email,
            Confiance = client.Confiance
        };
        return View(toEdit);
    }

    // POST: Clients/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id,
        [Bind("Confiance")] ClientsViewModel client)
    {
        ViewBag.Error = "";
        if (_client.TrouverClient(id) == null)
        {
            ViewBag.Error = "Le client que vous souhaitez modifier n'existe pas.";
        }
        else if (ModelState.IsValid)
        {
            try
            {
                Client toSave = _client.TrouverClient(id);
                toSave.Confiance = client.Confiance;
                _context.Entry(_client.TrouverClient(id)).CurrentValues.SetValues(toSave);
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_client.TrouverClient(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(client);
    }
}