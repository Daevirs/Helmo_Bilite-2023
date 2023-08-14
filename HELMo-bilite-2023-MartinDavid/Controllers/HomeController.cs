using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HELMo_bilite_2023_MartinDavid.Models;

namespace HELMo_bilite_2023_MartinDavid.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    
    [ResponseCache(Duration = 60*60*24, Location = ResponseCacheLocation.Any, NoStore = false)]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}