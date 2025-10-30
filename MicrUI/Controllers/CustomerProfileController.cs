using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers;

public class CustomerProfileController : Controller
{
    private readonly MicrDbContext _context;

    public CustomerProfileController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _context.CustomerProfiles
            .Include(c => c.RequestingBankBranch)
                .ThenInclude(bb => bb.Bank)
            .Include(c => c.CreatedByUser)
            .AsNoTracking()
            .OrderBy(c => c.CustomerName)
            .ToListAsync();

        return View(customers);
    }
}
