// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HELMo_bilite_2023_MartinDavid.Models;
using HELMo_bilite_2023_MartinDavid.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HELMo_bilite_2023_MartinDavid.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly DbContextHelmoBilite _context;
        private readonly ClientRepository _client;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(
            UserManager<Utilisateur> userManager,
            SignInManager<Utilisateur> signInManager,
            IWebHostEnvironment environment,
            DbContextHelmoBilite context,
            ClientRepository client)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _client = client;
            _context = context;
        }

        
        public string Username { get; set; }

        
        [TempData]
        public string StatusMessage { get; set; }

        
        [BindProperty]
        public InputModel Input { get; set; }

        
        public class InputModel
        {
            [DataType(DataType.Text)]
            [Display(Name = "Nom de famille")]
            [MaxLength(50)]
            public string Nom { get; set; }
            [DataType(DataType.Text)]
            [Display(Name = "Prénom")]
            [MaxLength(50)]
            public string Prenom { get; set; }
            [MaxLength(7)]
            [Display(Name = "Matricule")]
            public string Matricule { get; set; }
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Nom de l'entreprise")]
            public string NomEntreprise { get; set; }
            public string siegeId { get; set; }
            
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Rue (numéro inclus)")]
            [RegularExpression("[a-zA-z ]+[ ][\\d]{1,2}\\b", ErrorMessage = "Votre adresse est invalide")]
            public string Rue { get; set; }

            [StringLength(5)]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Code postal")]
            [RegularExpression("\\d{4,5}", ErrorMessage = "Code postal invalide")]
            public string CodePostal { get; set; }
            
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Ville")]
            public string Ville { get; set; }

            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
            [Display(Name = "Date d'anniversaire")]
            [DataType(DataType.Date)]
            public DateTime? DateNaissance { get; set; }
            [Display(Name = "Niveau d'étude")]
            public Diplome Diplome { get; set; }
            [Display(Name = "Permis B")]
            public bool PermisB { get; set; }
            [Display(Name = "Permis C")]
            public bool PermisC { get; set; }
            [Display(Name = "Permis CE")]
            public bool PermisCE { get; set; }
            [DisplayName("Changer la photo")]
            [DataType(DataType.Upload)]
            public IFormFile? photo { get; set; }
            [DisplayName("Ancienne photo")]
            public string? cheminPhoto { get; set; }
        }

        private async Task LoadAsync(Utilisateur user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            if (user is Client)
            {
                var client = (Client)user;
                var siege = _client.TrouverSiegeSocialClient(client.SiegeSocialId);
                Input = new InputModel
                {
                    NomEntreprise = client.NomEntreprise,
                    Rue = siege.Rue,
                    CodePostal = siege.CodePostal,
                    Ville = siege.Ville,
                    siegeId = siege.Id,
                    cheminPhoto = client.Photo
                };
            } else if (user is Dispatcher)
            {
                var dispatcher = (Dispatcher)user;
                Input = new InputModel
                {
                    Nom = dispatcher.Nom,
                    Prenom = dispatcher.Prenom,
                    Matricule = dispatcher.Matricule,
                    DateNaissance = dispatcher.DateNaissance,
                    Diplome = dispatcher.Diplome,
                    cheminPhoto = dispatcher.Photo
                };
            } else if (user is Chauffeur)
            {
                var chauffeur = (Chauffeur)user;
                Input = new InputModel
                {
                    Nom = chauffeur.Nom,
                    Prenom = chauffeur.Prenom,
                    Matricule = chauffeur.Matricule,
                    DateNaissance = chauffeur.DateNaissance,
                    PermisB = chauffeur.PermisB,
                    PermisC = chauffeur.PermisC,
                    PermisCE = chauffeur.PermisCE,
                    cheminPhoto= chauffeur.Photo
                };
            } else if (user is Admin)
            {
                var admin = (Admin)user;
                Input = new InputModel
                {
                    Nom = admin.Nom,
                    Prenom = admin.Prenom,
                    cheminPhoto = admin.Photo,
                    DateNaissance = admin.DateNaissance
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                ViewData["Error"] = "Erreur dans le formulaire, vérifiez vos champs";
                return Page();
            }

            if(user is Client)
            {
                var client = (Client)user;
                var siege = _client.TrouverSiegeSocialClient(client.SiegeSocialId);
                SiegeSocial nouveauSiege = new SiegeSocial();
                if (siege.Rue.Equals(Input.Rue) && !String.IsNullOrEmpty(Input.Rue))
                {
                    nouveauSiege.Rue = Input.Rue;
                }
                else
                {
                    Console.WriteLine(siege.Rue);
                    Console.WriteLine(Input.Rue);
                    nouveauSiege.Rue = siege.Rue;
                }
                if (!siege.CodePostal.Equals(Input.CodePostal) && !String.IsNullOrEmpty(Input.CodePostal))
                {
                    nouveauSiege.CodePostal = Input.CodePostal;
                }else
                {
                    Console.WriteLine(siege.CodePostal);
                    Console.WriteLine(Input.CodePostal);
                    nouveauSiege.CodePostal = siege.CodePostal;
                }
                if (!siege.Ville.Equals(Input.Ville) && !String.IsNullOrEmpty(Input.Ville))
                {
                    nouveauSiege.Ville = Input.Ville;
                }else
                {
                    Console.WriteLine(siege.Ville);
                    Console.WriteLine(Input.Ville);
                    nouveauSiege.Ville = siege.Ville;
                }
                if (!client.NomEntreprise.Equals(Input.NomEntreprise) && !String.IsNullOrEmpty(Input.NomEntreprise))
                {
                    client.NomEntreprise = Input.NomEntreprise;
                }
                if(Input.photo != null)
                {
                    string logoImgPath = await SaveFileClient(Input.photo, client);
                    client.Photo = logoImgPath;
                }

                nouveauSiege.Id = siege.Id;
                nouveauSiege.Pays = siege.Pays;
                _context.Entry(siege).CurrentValues.SetValues(nouveauSiege);

            } else if (user is Dispatcher)
            {
                var dispatcher = (Dispatcher)user;
                if(Input.DateNaissance >= DateTime.Now)
                {
                    ViewData["Error"] = "Bonjour Marty, vous n'êtes toujours pas autorisé à utiliser nos services";
                    await LoadAsync(user);
                    return Page();
                }
                if (Input.DateNaissance != dispatcher.DateNaissance)
                {
                    dispatcher.DateNaissance = Input.DateNaissance;
                }
                if (Input.photo != null)
                {
                    string profilePicImgPath = await SaveFileHelmo(Input.photo, dispatcher);
                    dispatcher.Photo = profilePicImgPath;
                }
            } else if (user is Chauffeur)
            {
                var chauffeur = (Chauffeur)user;
                if (Input.DateNaissance >= DateTime.Now)
                {
                    ViewData["Error"] = "Bonjour Marty, vous n'êtes toujours pas autorisé à utiliser nos services !";
                    await LoadAsync(user);
                    return Page();
                }
                if (Input.DateNaissance != chauffeur.DateNaissance)
                {
                    chauffeur.DateNaissance = Input.DateNaissance;
                }
                if (Input.photo != null)
                {
                    string profilePicImgPath = await SaveFileHelmo(Input.photo, chauffeur);
                    chauffeur.Photo = profilePicImgPath;
                }
            }
            else if(user is Admin)
            {
                var admin = (Admin)user;
                if (Input.DateNaissance >= DateTime.Now)
                {
                    ViewData["Error"] = "Bonjour Marty, vous n'êtes toujours pas autorisé à utiliser nos services !";
                    await LoadAsync(user);
                    return Page();
                }
                if (Input.DateNaissance != admin.DateNaissance)
                {
                    admin.DateNaissance = Input.DateNaissance;
                }
                if (Input.photo != null)
                {
                    string profilePicImgPath = await SaveFileHelmo(Input.photo, admin);
                    admin.Photo = profilePicImgPath;
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Votre profil a été mis à jour";
            return RedirectToPage();
        }
        private async Task<string> SaveFileClient(IFormFile photo, Client client)
        {
            var imgPath = Path.Combine("Q210060", "img", "LogoClient");
            try
            {
                var webRootPath = _environment.WebRootPath;
                var fileName = GetPath("LogoClient", $"{client.NomEntreprise}" + ".jpg");
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
        private async Task<string> SaveFileHelmo(IFormFile pictureFile, Membre membre)
        {
            var imgPath = Path.Combine("Q210060", "img", "HelmoPhotoProfil");
            try
            {
                var webRootPath = _environment.WebRootPath;
                var fileName = GetPath("HelmoPhotoProfil", $"{membre.Matricule}" + ".jpg");
                var filePath = Path.Combine(webRootPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await pictureFile.CopyToAsync(fileStream);
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
