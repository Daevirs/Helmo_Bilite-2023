using Bogus;
using HELMo_bilite_2023_MartinDavid.Models;
using Microsoft.AspNetCore.Identity;

namespace HELMo_bilite_2023_MartinDavid;
public static class DataInitializer {
    public static void SeedRole(RoleManager<IdentityRole> roleManager)
    {
        if (roleManager.RoleExistsAsync("Admin").Result == false)
        {
            IdentityRole admin = new IdentityRole() { Name = "Admin" };
            var result = roleManager.CreateAsync(admin);
            result.Wait();
        }

        if (roleManager.RoleExistsAsync("Client").Result == false)
        {
            IdentityRole user = new IdentityRole() { Name = "Client" };
            var result = roleManager.CreateAsync(user);
            result.Wait();
        }

        if (roleManager.RoleExistsAsync("Dispatcher").Result == false)
        {
            IdentityRole dispatcher = new IdentityRole() { Name = "Dispatcher" };
            var result = roleManager.CreateAsync(dispatcher);
            result.Wait();
        }
            
        if (roleManager.RoleExistsAsync("Chauffeur").Result == false)
        {
            IdentityRole chauffeur = new IdentityRole() { Name = "Chauffeur" };
            var result = roleManager.CreateAsync(chauffeur);
            result.Wait();
        }
    }

    public static async Task Seed(UserManager<Utilisateur> _userManager)
    {
        // création d'un utilisateur pour chaque rôle 
            
        var dispatcher = new Dispatcher
        {
            Prenom = "Dimitry",
            Nom = "Parcher",
            Email = "d.parcher@helmo-bilite.be",
            UserName = "d.parcher@helmo-bilite.be",
            Matricule = "D000001",
            Diplome = Diplome.Bachelier
        };
        var resultD = _userManager.CreateAsync(dispatcher, "Azer+1").Result;
        Console.WriteLine("Dispatcher");
        if (resultD.Succeeded)
        {
            
            Console.WriteLine("Dispatcher réussi");
            var dispatch = _userManager.AddToRoleAsync(dispatcher, "Dispatcher").Result;
        }

        var chauffeur = new Chauffeur()
        {
            Prenom = "Cedric",
            Nom = "Hoffer",
            UserName = "c.hoffer@helmo-bilite.be",
            Email = "c.hoffer@helmo-bilite.be",
            Matricule = "C000002",
            PermisB = true,
            PermisC = true,
            PermisCE = true
        };
        var resultCh = _userManager.CreateAsync(chauffeur, "Azer+1").Result;
        Console.WriteLine("chauffeur");
        if (resultCh.Succeeded)
        {
            Console.WriteLine("Chauffeur réussi");
            var chauff = _userManager.AddToRoleAsync(chauffeur, "Chauffeur").Result;
        }

        var admin = new Admin
        {
            Prenom = "Adrien",
            Nom = "Ming",
            Email = "a.ming@helmo-bilite.be",
            UserName = "a.ming@helmo-bilite.be",
            Matricule = "A000003",
        };
        var resultAd = _userManager.CreateAsync(admin, "Azer+1").Result;
        Console.WriteLine("Admin");
        if (resultAd.Succeeded)
        {
            Console.WriteLine("Admin réussi");
            var adm = _userManager.AddToRoleAsync(admin, "Admin").Result;
        }
    }
    
    /*
    NB: Utilisateurs créé (Mot de passe = Azer+1)
    
    Client : 
    gunther.rehtnug@cisco.com
    client@testing.be
    client2@testing.be
    client3@testing.be
    client4@testing.be
    
    Dispatcher : 
    d.parcher@helmo-bilite.be
    j.mosan@helmo-bilite.be
    
    Chauffeur : 
    c.hoffer@helmo-bilite.be (B, C, CE)
    j.vandentruck@helmo-bilite.be (B, C, CE)
    chauffeur@testing.be (B)
    chauffeur2@testing.be (C)
    chauffeur3@testing.be (CE)
    
    Admin :
    a.ming@helmo-bilite.be
    */
}