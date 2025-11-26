using BusinessLogic.Logic;
using Domain.ViewModels.ApprovalStatuses;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class ApprovalStatusController : Controller
{
    private readonly IApprovalStatusService _service;
    private readonly ILogger<ApprovalStatusController> _logger;

    public ApprovalStatusController(IApprovalStatusService service, ILogger<ApprovalStatusController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
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
    public async Task<JsonResult> CreateUpdate(long? approvalStatusId, ApprovalStatusFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (approvalStatusId.HasValue && approvalStatusId.Value > 0)
            {
                var updated = await _service.UpdateAsync(approvalStatusId.Value, request);
                _logger.LogInformation("ApprovalStatus updated: {Id}", approvalStatusId.Value);
                return Json(new { success = true, messages = "Approval status updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
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
