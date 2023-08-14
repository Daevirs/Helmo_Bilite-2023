using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HELMo_bilite_2023_MartinDavid.Models;

namespace HELMo_bilite_2023_MartinDavid.ViewModel;

public class StatistiquesViewModel
{
    [DisplayName("Client")]
    public string Client { get; set; }
    [DisplayName("Chauffeur")]
    public string Chauffeur { get; set; }
    [DisplayName("Date de livraison effectuée")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime Date { get; set; }
    [DisplayName("Status")]
    public StatusLivraison Status { get; set; }
}