using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.ViewModels;
using Domain.DTOs;

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

    [HttpGet]
    public async Task<JsonResult> GetStatusData()
    {
        var list = await _context.Statuses
            .Include(s => s.CreatedByUser)
            .AsNoTracking()
            .OrderBy(s => s.StatusName)
            .Select(s => new
            {
                statusId = s.StatusId,
                statusName = s.StatusName,
                created = s.CreatedDate.ToLocalTime().ToString("dd MMM yyyy"),
                createdBy = s.CreatedByUser.Fullname
            })
            .ToListAsync();

        return Json(new { data = list, Success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdateStatus(long? statusId, StatusCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new ResponseMessage { Success = false, Messages = "Please provide the required details." });
            }

            var normalized = (request.StatusName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return Json(new ResponseMessage { Success = false, Messages = "Status name is required." });
            }

            // Duplicate check (case-insensitive)
            var exists = await _context.Statuses
                .AsNoTracking()
                .Where(s => s.StatusName.ToLower() == normalized.ToLower())
                .ToListAsync();

            if (statusId.HasValue && statusId.Value > 0)
            {
                var target = await _context.Statuses.FindAsync(statusId.Value);
                if (target == null)
                {
                    return Json(new ResponseMessage { Success = false, Messages = "Selected Status was not found." });
                }

                if (exists.Any(s => s.StatusId != statusId.Value))
                {
                    return Json(new ResponseMessage { Success = false, Messages = "Status name already exists." });
                }

                target.StatusName = normalized;
                await _context.SaveChangesAsync();
                return Json(new ResponseMessage { Success = true, Messages = "Status updated successfully!" });
            }
            else
            {
                if (exists.Any())
                {
                    return Json(new ResponseMessage { Success = false, Messages = "Status name already exists." });
                }

                var createdBy = await ResolveCurrentUserId();
                var entity = new Status
                {
                    StatusName = normalized,
                    CreatedByUserId = createdBy,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Statuses.Add(entity);
                await _context.SaveChangesAsync();
                return Json(new ResponseMessage { Success = true, Messages = "New Status added successfully!" });
            }
        }
        catch (Exception)
        {
            return Json(new ResponseMessage { Success = false, Messages = "We couldn't save the record. Please try again." });
        }
    }

    private Task<long> ResolveCurrentUserId() => Task.FromResult(1L);
}
