using Bogus;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HELMo_bilite_2023_MartinDavid.Models;

public class DbContextHelmoBilite:IdentityDbContext<Utilisateur>
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Chauffeur> Chauffeurs { get; set; }
    public DbSet<Dispatcher> Dispatchers { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Livraison> Livraisons { get; set; }
    public DbSet<SiegeSocial> SiegeSociaux { get; set; }
    public DbSet<Camion> Camions { get; set; }
    public DbSet<Marque> Marques { get; set; }
    public DbSet<MotifExcuse> MotifExcuses { get; set; }

    public DbContextHelmoBilite(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Camion>().Property(c => c.Type)
            .HasConversion(t => t.ToString(),
                p => (Permis)Enum.Parse(typeof(Permis), p)).HasMaxLength(50);
        modelBuilder.Entity<Dispatcher>().Property(d => d.Diplome)
            .HasConversion(d => d.ToString(),
                d => (Diplome)Enum.Parse(typeof(Diplome), d)).HasMaxLength(50);
        
        modelBuilder.Entity<Utilisateur>(u =>
        {
            u.HasDiscriminator<string>("Role")
                .HasValue<Admin>("Admin")
                .HasValue<Client>("Client")
                .HasValue<Dispatcher>("Dispatcher")
                .HasValue<Chauffeur>("Chauffeur");
        
            u.Property("Role").HasMaxLength(50);
        });
        Seed(modelBuilder);
    }
    public void Seed(ModelBuilder modelBuilder){
            List<Marque> marques = new List<Marque> { 
                new() {Id = 1, NomMarque = "Volkswagen" },
                new() {Id = 2, NomMarque = "MAN"},
                new() {Id = 3, NomMarque = "IVECO"},
                new() {Id = 4, NomMarque = "Renault"},
                new() {Id = 5, NomMarque = "Volvo"} };
            for (int i = 0; i < 5; i++)
            {
                modelBuilder.Entity<Marque>().HasData(marques[i]);
            }

            List<string> modeles = new List<string>() 
                { "Crafter", "Atleon", "Eurocargo", "Trucks D", "Peterbilt 389" };

            string lettres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string nombres = "0123456789";
            
            //Création d'une flotte de 10 camions
            for (int i = 0; i < 10; i++)
            {
                string milieuPlaque = "";
                string finPlaque = "";
                for (int j = 0; j < 3; j++)
                {
                    milieuPlaque += lettres[Randomizer.Seed.Next(0, lettres.Length)];
                    finPlaque += nombres[Randomizer.Seed.Next(0, nombres.Length)];
                }
                string immatriculation = "1-" + milieuPlaque + "-" + finPlaque;
                Marque marque = marques.ToArray()[i % 5];
                string modele = modeles.ToArray()[i % 5];
                int type = Randomizer.Seed.Next(1, 3);
                int tonnage = (Randomizer.Seed.Next(15000, 30000) + 50) / 100 * 100;
                modelBuilder.Entity<Camion>().HasData(
                    new Camion
                    {
                        Immatriculation = immatriculation,
                        MarqueId = marque.Id,
                        Modele = modele,
                        Type = (Permis)type,
                        Tonnage = tonnage,
                        Photo = ""
                    });
            }

            modelBuilder.Entity<MotifExcuse>().HasData(
                new MotifExcuse
                {
                    Id = 1,
                    Motif = "Maladie"
                });
            modelBuilder.Entity<MotifExcuse>().HasData(
                new MotifExcuse
                {
                    Id = 2,
                    Motif = "Accident lors du trajet"
                });
            modelBuilder.Entity<MotifExcuse>().HasData(
                new MotifExcuse
                {
                    Id = 3,
                    Motif = "Accident lors de la livraison"
                });
            modelBuilder.Entity<MotifExcuse>().HasData(
                new MotifExcuse
                {
                    Id = 4,
                    Motif = "Client absent"
                });
            modelBuilder.Entity<MotifExcuse>().HasData(
                new MotifExcuse
                {
                    Id = 5,
                    Motif = "Livraison impossible"
                });
    }
}
