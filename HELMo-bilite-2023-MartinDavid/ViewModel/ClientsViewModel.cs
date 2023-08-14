using System.ComponentModel;
using HELMo_bilite_2023_MartinDavid.Models;

namespace HELMo_bilite_2023_MartinDavid.ViewModel;

public class ClientsViewModel
{
    [DisplayName("Nom de l'entreprise")]
    public string? NomEntreprise { get; set; }
    [DisplayName("Adresse de l'entreprise")]
    public string? Siege { get; set; }
    [DisplayName("Adresse mail de l'entreprise")]
    public string? Email { get; set; }
    [DisplayName("Mauvais payeur ?")]
    public bool Confiance { get; set; }
}