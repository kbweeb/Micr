using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.ViewModels;

namespace MicrDbChequeProcessingSystem.Controllers;

public class RegionController : Controller
{
    private readonly MicrDbContext _context;

    public RegionController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new RegionCreateRequest());
    }

    public async Task<IActionResult> Index()
    {
        var systemRegions = (await _context.RegionZones
                .Include(r => r.Banks)
                    .ThenInclude(b => b.BankBranches)
                .AsNoTracking()
                .OrderBy(r => r.RegionName)
                .ToListAsync())
            .Select(r => new RegionListItem
            {
                Name = r.RegionName,
                Description = r.Description,
                Created = (r.CreatedDate ?? DateTime.MinValue).ToString("dd MMM yyyy"),
                Banks = r.Banks.Count,
                Branches = r.Banks.Sum(b => b.BankBranches.Count)
            })
            .ToList();

        var viewModel = new RegionIndexViewModel
        {
            Items = systemRegions
        };

        return View(viewModel);
    }

    // HTML form submit handler
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateForm(RegionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", request);
        }

        var entry = new RegionZone
        {
            RegionName = request.RegionName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CreatedByUserId = await ResolveCurrentUserId(),
            CreatedDate = DateTime.UtcNow
        };

        try
        {
            _context.RegionZones.Add(entry);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "We couldn't save the record. Please try again.");
            return View("Create", request);
        }

        TempData["Message"] = "Region created";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RegionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Please provide the required details." });
        }

        var entry = new RegionZone
        {
            RegionName = request.RegionName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CreatedByUserId = await ResolveCurrentUserId(),
            CreatedDate = DateTime.UtcNow
        };

        try
        {
            _context.RegionZones.Add(entry);
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
                regionName = entry.RegionName,
                description = entry.Description,
                created = (entry.CreatedDate ?? DateTime.UtcNow).ToLocalTime().ToString("dd MMM yyyy HH:mm"),
                banks = 0,
                branches = 0
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
