using HELMo_bilite_2023_MartinDavid.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace HELMo_bilite_2023_MartinDavid.Repositories;

public class ClientRepository
{
    private readonly DbContextHelmoBilite _context;
    public IEnumerable<Client> Clients => _context.Clients;
    public ClientRepository(DbContextHelmoBilite context)
    {
        _context = context;
    }

    public Client TrouverClient(string id) => _context.Clients.Find(id);

    public SiegeSocial TrouverSiegeSocialClient(string id) => _context.SiegeSociaux.Find(id);
    
}