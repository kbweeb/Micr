using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class StatusController : Controller
{
    private readonly MicrDbContext _context;

    public StatusController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var statuses = await _context.Statuses
            .Include(s => s.CreatedByUser)
            .AsNoTracking()
            .OrderBy(s => s.StatusName)
            .ToListAsync();

        return View(statuses);
    }
}
