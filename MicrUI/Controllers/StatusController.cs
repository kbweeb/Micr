using BusinessLogic.Logic;
using Domain.ViewModels.Statuses;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class StatusController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<StatusController> _logger;

    public StatusController(IMicrPortalService portal, ILogger<StatusController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetStatusesAsync();
        var items = list.Select(s => new StatusListItemViewModel
        {
            StatusId = s.StatusId,
            StatusName = s.StatusName,
            Created = s.Created,
            CreatedBy = s.CreatedBy
        }).ToList();

        return View(new StatusIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(StatusFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.StatusId.HasValue && request.StatusId.Value > 0)
            {
                var updated = await _portal.UpdateStatusAsync(request.StatusId.Value, request);
                _logger.LogInformation("Status updated: {Id}", request.StatusId.Value);
                return Json(new { success = true, messages = "Status updated successfully!", data = updated });
            }

            var created = await _portal.CreateStatusAsync(request);
            _logger.LogInformation("Status created: {Id}", created.StatusId);
            return Json(new { success = true, messages = "New status added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving status");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
