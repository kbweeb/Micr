using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MicrDbContext _db;

        public HomeController(ILogger<HomeController> logger, MicrDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard counts
            var vm = new DashboardVm
            {
                BanksCount = await _db.Banks.CountAsync(),
                BranchesCount = await _db.BankBranches.CountAsync(),
                CustomersCount = await _db.CustomerProfiles.CountAsync(),
                RegionsCount = await _db.RegionZones.CountAsync()
            };

            return View(vm);
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

    // simple view model for dashboard
    public class DashboardVm
    {
        public int BanksCount { get; set; }
        public int BranchesCount { get; set; }
        public int CustomersCount { get; set; }
        public int RegionsCount { get; set; }
    }
}
