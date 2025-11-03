using BusinessLogic.Logic;
using Domain.DTOs;
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
    private readonly IRegionService _service;

    public RegionController(MicrDbContext context, IRegionService service)
    {
        _context = context;
        _service = service;
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
    public async Task<IActionResult> Create(RegionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Please provide the required details." });
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
            var result = await _service.CreateAsync(dto);
            return Json(new
            {
                success = true,
                data = new
                {
                    regionName = result.RegionName,
                    description = result.Description,
                    created = result.Created,
                    banks = 0,
                    branches = 0
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "We couldn't save the record. Please try again." });
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
