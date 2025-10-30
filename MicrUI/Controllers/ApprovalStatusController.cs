using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class ApprovalStatusController : Controller
{
    private readonly MicrDbContext _context;

    public ApprovalStatusController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var statuses = await _context.ApprovalStatuses
            .Include(a => a.CreatedByUser)
            .AsNoTracking()
            .OrderBy(a => a.ApprovalStatusName)
            .ToListAsync();

        return View(statuses);
    }
}
