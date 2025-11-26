using BusinessLogic.Logic;
using Domain.ViewModels.Statuses;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class StatusController : Controller
{
    private readonly IStatusService _service;
    private readonly ILogger<StatusController> _logger;

    public StatusController(IStatusService service, ILogger<StatusController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
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
    public async Task<JsonResult> CreateUpdate(long? statusId, StatusFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (statusId.HasValue && statusId.Value > 0)
            {
                var updated = await _service.UpdateAsync(statusId.Value, request);
                _logger.LogInformation("Status updated: {Id}", statusId.Value);
                return Json(new { success = true, messages = "Status updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
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
