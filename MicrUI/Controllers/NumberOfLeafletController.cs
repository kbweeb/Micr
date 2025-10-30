using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class NumberOfLeafletController : Controller
{
    private readonly MicrDbContext _context;

    public NumberOfLeafletController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var leaflets = await _context.NumberOfLeaflets
            .Include(l => l.CreatedByUser)
            .AsNoTracking()
            .OrderBy(l => l.NumberOfLeaflet1)
            .ToListAsync();

        return View(leaflets);
    }
}
