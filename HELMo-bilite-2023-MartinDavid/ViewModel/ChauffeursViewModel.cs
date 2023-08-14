using System.ComponentModel;

namespace HELMo_bilite_2023_MartinDavid.ViewModel;

public class ChauffeurViewModel
{
    public string? Id { get; set; }
    [DisplayName("Matricule du chauffeur")]
    public string? Matricule { get; set; }
    
    [DisplayName("Nom")]
    public string? Nom { get; set; }
    
    [DisplayName("Prénom")]
    public string? Prenom { get; set; }
    
    [DisplayName("Permis B")]
    public bool PermisB { get; set; }
    
    [DisplayName("Permis C")]
    public bool PermisC { get; set; }
    
    [DisplayName("Permis CE")]
    public bool PermisCE { get; set; }
}