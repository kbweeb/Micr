using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.ViewModels;

namespace MicrDbChequeProcessingSystem.Controllers;

public class AccountTypeController : Controller
{
    private readonly MicrDbContext _context;

    public AccountTypeController(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.AccountTypes
            .AsNoTracking()
            .OrderBy(a => a.AccountTypeName)
            .Select(a => new AccountTypeListItem
            {
                Name = a.AccountTypeName,
                Code = a.AccountTypeCode,
                Description = a.Description,
                Created = a.CreatedDate.ToString("dd MMM yyyy")
            })
            .ToListAsync();

        var viewModel = new AccountTypeIndexViewModel { Items = items };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AccountTypeCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Please provide the required details." });
        }

        // Resolve current user id
        long createdBy = await ResolveCurrentUserId();

        // Generate a simple code from the name
        var name = request.AccountTypeName.Trim();
        var baseCode = new string(name
            .Where(char.IsLetterOrDigit)
            .Take(10)
            .Select(char.ToUpper)
            .ToArray());
        if (string.IsNullOrWhiteSpace(baseCode)) baseCode = "ACCTYPE";

        var code = baseCode;
        int i = 1;
        while (await _context.AccountTypes.AnyAsync(a => a.AccountTypeCode == code))
        {
            code = (baseCode + i.ToString()).Substring(0, Math.Min(10, (baseCode + i.ToString()).Length));
            i++;
        }

        var entry = new AccountType
        {
            AccountTypeName = name,
            AccountTypeCode = code,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            IsActive = true,
            CreatedByUserId = createdBy,
            CreatedDate = DateTime.UtcNow
        };

        try
        {
            _context.AccountTypes.Add(entry);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { success = false, message = "We couldn't save the record. Please try again." });
        }

        return Json(new
        {
            success = true,
            data = new
            {
                id = entry.AccountTypeId,
                accountTypeName = entry.AccountTypeName,
                code = entry.AccountTypeCode,
                description = entry.Description,
                created = entry.CreatedDate.ToLocalTime().ToString("dd MMM yyyy HH:mm")
            }
        });
    }

    private async Task<long> ResolveCurrentUserId()
    {
        var winUser = Environment.UserName;
        var user = await _context.UserProfiles.AsNoTracking()
            .OrderBy(u => u.UserId)
            .FirstOrDefaultAsync(u => u.Username == winUser) ??
                   await _context.UserProfiles.AsNoTracking().OrderBy(u => u.UserId).FirstOrDefaultAsync();
        return user?.UserId ?? 1;
    }
}
