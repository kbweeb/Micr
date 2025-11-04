using BusinessLogic.Logic;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.ViewModels;
using Microsoft.Extensions.Logging;

namespace MicrDbChequeProcessingSystem.Controllers;

public class RegionController : Controller
{
    private readonly MicrDbContext _context;
    private readonly IRegionService _service;
    private readonly ILogger<RegionController> _logger;

    public RegionController(MicrDbContext context, IRegionService service, ILogger<RegionController> logger)
    {
        _context = context;
        _service = service;
        _logger = logger;
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
                Id = r.RegionId,
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

        try
        {
            var createdBy = await ResolveCurrentUserId();
            var dto = new RegionCreateDto
            {
                RegionName = request.RegionName,
                Description = request.Description,
                CreatedByUserId = createdBy
            };
            await _service.CreateAsync(dto);
            TempData["Message"] = "Region created";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "We couldn't save the record. Please try again.");
            return View("Create", request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(long? regionId, RegionCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Messages = "Please provide the required details." });
            }

            var createdBy = await ResolveCurrentUserId();
            var dto = new RegionCreateDto
            {
                RegionName = request.RegionName,
                Description = request.Description,
                CreatedByUserId = createdBy
            };

            if (regionId.HasValue && regionId.Value > 0)
            {
                var updated = await _service.UpdateAsync(regionId.Value, dto);
                _logger.LogInformation("Region updated successfully: {Id}", regionId.Value);
                return Json(new { Success = true, Messages = "Region updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(dto);
            _logger.LogInformation("New Region added successfully: {Id}", created.RegionId);
            return Json(new { Success = true, Messages = "New Region added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating Region");
            return Json(new { Success = false, Messages = "We couldn't save the record. Please try again." });
        }
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
