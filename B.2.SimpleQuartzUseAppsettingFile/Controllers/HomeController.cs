using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using B._2.SimpleQuartzUseAppsettingFile.Models;
using Common;
using Microsoft.EntityFrameworkCore;

namespace B._2.SimpleQuartzUseAppSettingFile.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.Log(LogLevel.Information, "Home/Index");
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

    public IActionResult Start(string id)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        
        using var db = new MyDbContext();
        db.Database.ExecuteSqlRawAsync("Select * from dbo.Users Where user = '" + id + "';");
        return Ok();
    }
}