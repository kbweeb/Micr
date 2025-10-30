using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class TransactionCodeController : Controller
{
    private readonly MicrDbContext _context;

    public TransactionCodeController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var codes = await _context.TransactionCodes
            .Include(t => t.CreatedByUser)
            .AsNoTracking()
            .OrderBy(t => t.Code)
            .ToListAsync();

        return View(codes);
    }
}
