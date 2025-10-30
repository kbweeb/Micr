using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BankBranchController : Controller
{
    private readonly MicrDbContext _context;

    public BankBranchController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var branches = await _context.BankBranches
            .Include(b => b.Bank)
            .AsNoTracking()
            .OrderBy(b => b.Bank.BankName)
            .ThenBy(b => b.BankBranchName)
            .ToListAsync();

        return View(branches);
    }
}
