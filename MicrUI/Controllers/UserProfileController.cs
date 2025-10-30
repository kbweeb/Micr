using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class UserProfileController : Controller
{
    private readonly MicrDbContext _context;

    public UserProfileController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _context.UserProfiles
            .Include(u => u.ApprovedStatus)
            .Include(u => u.CreatedByUser)
            .AsNoTracking()
            .OrderBy(u => u.Username)
            .ToListAsync();

        return View(users);
    }
}
