using BusinessLogic.Logic;
using Domain.ViewModels.ApprovalStatuses;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class ApprovalStatusController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<ApprovalStatusController> _logger;

    public ApprovalStatusController(IMicrPortalService portal, ILogger<ApprovalStatusController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetApprovalStatusesAsync();
        var items = list.Select(a => new ApprovalStatusListItemViewModel
        {
            ApprovalStatusId = a.ApprovalStatusId,
            ApprovalStatusName = a.ApprovalStatusName,
            Created = a.Created,
            CreatedBy = a.CreatedBy
        }).ToList();

        return View(new ApprovalStatusIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(ApprovalStatusFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.ApprovalStatusId.HasValue && request.ApprovalStatusId.Value > 0)
            {
                var updated = await _portal.UpdateApprovalStatusAsync(request.ApprovalStatusId.Value, request);
                _logger.LogInformation("ApprovalStatus updated: {Id}", request.ApprovalStatusId.Value);
                return Json(new { success = true, messages = "Approval status updated successfully!", data = updated });
            }

            var created = await _portal.CreateApprovalStatusAsync(request);
            _logger.LogInformation("ApprovalStatus created: {Id}", created.ApprovalStatusId);
            return Json(new { success = true, messages = "New approval status added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving approval status");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
