using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class CurrencyController : Controller
{
    private readonly MicrDbContext _context;

    public CurrencyController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var currencies = await _context.Currencies
            .Include(c => c.CreatedByUser)
            .AsNoTracking()
            .OrderBy(c => c.CurrencyName)
            .ToListAsync();

        return View(currencies);
    }
}
