using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BankController : Controller
{
    private readonly MicrDbContext _context;

    public BankController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var banks = await _context.Banks
            .Include(b => b.Region)
            .AsNoTracking()
            .OrderBy(b => b.BankName)
            .ToListAsync();

        return View(banks);
    }
}
