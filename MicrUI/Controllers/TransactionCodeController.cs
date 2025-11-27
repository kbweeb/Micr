using BusinessLogic.Logic;
using Domain.ViewModels.TransactionCodes;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class TransactionCodeController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<TransactionCodeController> _logger;

    public TransactionCodeController(IMicrPortalService portal, ILogger<TransactionCodeController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetTransactionCodesAsync();
        var items = list.Select(t => new TransactionCodeListItemViewModel
        {
            TransactionCodeId = t.TransactionCodeId,
            Code = t.Code,
            Created = t.Created,
            CreatedBy = t.CreatedBy
        }).ToList();

        return View(new TransactionCodeIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(TransactionCodeFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.TransactionCodeId.HasValue && request.TransactionCodeId.Value > 0)
            {
                var updated = await _portal.UpdateTransactionCodeAsync(request.TransactionCodeId.Value, request);
                _logger.LogInformation("TransactionCode updated: {Id}", request.TransactionCodeId.Value);
                return Json(new { success = true, messages = "Transaction code updated successfully!", data = updated });
            }

            var created = await _portal.CreateTransactionCodeAsync(request);
            _logger.LogInformation("TransactionCode created: {Id}", created.TransactionCodeId);
            return Json(new { success = true, messages = "New transaction code added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving transaction code");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
