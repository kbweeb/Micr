using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BookTypeController : Controller
{
    private readonly MicrDbContext _context;

    public BookTypeController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookTypes = await _context.BookTypes
            .Include(b => b.AccountType)
            .Include(b => b.NumberOfLeaflet)
            .Include(b => b.TransactionCode)
            .AsNoTracking()
            .OrderBy(b => b.BookTypeName)
            .ToListAsync();

        return View(bookTypes);
    }
}
