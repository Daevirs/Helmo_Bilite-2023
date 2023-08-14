// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using HELMo_bilite_2023_MartinDavid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HELMo_bilite_2023_MartinDavid.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IUserStore<Utilisateur> _userStore;
        private readonly IUserEmailStore<Utilisateur> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DbContextHelmoBilite _context;

        public RegisterModel(
            UserManager<Utilisateur> userManager,
            IUserStore<Utilisateur> userStore,
            SignInManager<Utilisateur> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            DbContextHelmoBilite context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            public string Role { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [StringLength(200, ErrorMessage = "Email obligatoire", MinimumLength = 1)]
            public string Email { get; set; }
            
            [Required]
            [StringLength(100, ErrorMessage = "Le mot de passe doit faire au minimum 6 caractères de long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de passe")]
            public string Password { get; set; }
            
            [DataType(DataType.Password)]
            [Display(Name = "Confirmer mot de passe")]
            [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation sont différents.")]
            public string ConfirmPassword { get; set; }
            
            // Partie Membre / Admin
            [MaxLength(7)]
            [MinLength(7)]
            [RegularExpression("[A-Z]\\d{6}", ErrorMessage = "Votre matricule ne respecte pas la forme \"A123456\", veuillez réessayer")]
            [Display(Name = "Matricule")]
            public string Matricule { get; set; }
            
            [DataType(DataType.Text)]
            [MaxLength(50)]
            [Display(Name = "Prénom")]
            public string Prenom { get; set; }
            
            [DataType(DataType.Text)]
            [MaxLength(50)]
            [Display(Name = "Nom de famille")]
            public string Nom { get; set; }
            
            // Partie Chauffeur
            [Display(Name = "Permis B")]
            public bool PermisB { get; set; }
            [Display(Name = "Permis C")]
            public bool PermisC { get; set; }
            [Display(Name = "Permis CE")]
            public bool PermisCE { get; set; }
            
            // Partie Dispatcher
            [Display(Name = "Niveau d'étude")]
            [DefaultValue(Diplome.CESS)]
            public Diplome Diplome { get; set; }
            
            // Partie Client
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Nom de l'entreprise")]
            public string NomEntreprise { get; set; }
            
            [StringLength(100)]
            [DataType(DataType.Text)]
            [Display(Name = "Rue (numéro inclus)")]
            [RegularExpression("[a-zA-z ]+[ ][\\d]{1,3}\\b", ErrorMessage = "Votre adresse est invalide")]
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
            
            [StringLength(50)]
            [DataType(DataType.Text)]
            [Display(Name = "Pays")]
            public string Pays { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid && ValidateForm())
            {
                Utilisateur user = CreerUtilisateurParRole(Input.Role);

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var utilisateur = await _userManager.FindByEmailAsync(user.Email);

                    await AssignerUtilisateurAuRoleAsync(Input.Role, utilisateur);

                    await _signInManager.SignInAsync(utilisateur, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private bool ValidateForm()
        {
            bool valid = true;
            switch (Input.Role)
            {
                case "Client":
                    if (string.IsNullOrWhiteSpace(Input.NomEntreprise))
                    {
                        ViewData["Error"] += "Nom d'entreprise invalide \n";
                        valid = false;
                    }
                    if (string.IsNullOrWhiteSpace(Input.CodePostal))
                    {
                        ViewData["Error"] += "Code postal manquant \n";
                        valid = false;
                    }
                    if (string.IsNullOrWhiteSpace(Input.Ville))
                    {
                        ViewData["Error"] += "Nom de ville invalide \n";
                        valid = false;
                    }
                    if (string.IsNullOrWhiteSpace(Input.Pays))
                    {
                        ViewData["Error"] += "Pays non sélectionné \n";
                        valid = false;
                    }
                    break;
                case "Chauffeur":
                    if (!Input.PermisB && !Input.PermisC && !Input.PermisCE)
                    {
                        ViewData["Error"] += "Vous devez sélectionner au minimum un permis \n";
                        valid = false;
                    }
                    break;
                case "Dispatcher":
                    break;
                default:
                    ViewData["Error"] += "Une erreur est survenue lors du choix d'un rôle \n";
                    valid = false;
                    break;
            }
            return valid;
        }
        
        private async Task AssignerUtilisateurAuRoleAsync(string role, Utilisateur user)
        {
            switch (role)
            {
                case "Client":
                    await _userManager.AddToRoleAsync(user, "Client");
                    break;
                case "Chauffeur":
                    await _userManager.AddToRoleAsync(user, "Chauffeur");
                    break;
                case "Dispatcher":  
                    await _userManager.AddToRoleAsync(user, "Dispatcher");
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private Utilisateur CreerUtilisateurParRole(string role)
        {
            switch (role)
            {
                case "Client":
                    var client = Activator.CreateInstance<Client>();
                    client.Email = Input.Email;
                    client.NomEntreprise = Input.NomEntreprise;
                    client.TwoFactorEnabled = false;
                    client.SiegeSocialId = CréerSiegeSocial();
                    return client;
                case "Chauffeur":
                    var chauffeur = Activator.CreateInstance<Chauffeur>();
                    chauffeur.Email = Input.Email;
                    chauffeur.Nom = Input.Nom;
                    chauffeur.Prenom = Input.Prenom;
                    chauffeur.Matricule = Input.Matricule;
                    chauffeur.PermisB = Input.PermisB;
                    chauffeur.PermisC = Input.PermisC;
                    chauffeur.PermisCE = Input.PermisCE;
                    chauffeur.TwoFactorEnabled = false;
                    return chauffeur;
                case "Dispatcher":
                    var dispatcher = Activator.CreateInstance<Dispatcher>();
                    dispatcher.Email = Input.Email;
                    dispatcher.Prenom = Input.Prenom;
                    dispatcher.Nom = Input.Nom;
                    dispatcher.Matricule = Input.Matricule;
                    dispatcher.Diplome = Input.Diplome;
                    dispatcher.TwoFactorEnabled = false;
                    return dispatcher;
                default:
                    throw new ArgumentException();
            }
        }

        private string CréerSiegeSocial()
        {
            var SiegeSocial = new SiegeSocial
            {
                Rue = Input.Rue,
                CodePostal = Input.CodePostal,
                Ville = Input.Ville,
                Pays = Input.Pays
            };

            _context.SiegeSociaux.Add(SiegeSocial);
            _context.SaveChanges();
            return SiegeSocial.Id;
        }

        private IUserEmailStore<Utilisateur> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Utilisateur>)_userStore;
        }
    }
}
