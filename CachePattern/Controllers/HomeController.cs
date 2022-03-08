using CachePattern.Models;
using CachePattern.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;

namespace CachePattern.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICacheService _cacheService;

    public HomeController(ILogger<HomeController> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public IActionResult Index()
    {
        var currentTimeUTC = DateTime.UtcNow.ToString();
        byte[] encodedCurrentTimeUTC = System.Text.Encoding.UTF8.GetBytes(currentTimeUTC);
        _cacheService.Set("cachedTimeUTC", encodedCurrentTimeUTC);

        return View();
    }

    public IActionResult Privacy()
    {
        var cacheTimeUTC = string.Empty;
        var encodedCurrentTimeUTC = _cacheService.Get<byte[]>("cachedTimeUTC");
        if (encodedCurrentTimeUTC != null)
            cacheTimeUTC = System.Text.Encoding.UTF8.GetString(encodedCurrentTimeUTC);

        ViewBag.cacheTimeUTC = cacheTimeUTC;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}