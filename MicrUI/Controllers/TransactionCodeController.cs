using BusinessLogic.Logic;
using Domain.ViewModels.TransactionCodes;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class TransactionCodeController : Controller
{
    private readonly ITransactionCodeService _service;
    private readonly ILogger<TransactionCodeController> _logger;

    public TransactionCodeController(ITransactionCodeService service, ILogger<TransactionCodeController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
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
    public async Task<JsonResult> CreateUpdate(long? transactionCodeId, TransactionCodeFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (transactionCodeId.HasValue && transactionCodeId.Value > 0)
            {
                var updated = await _service.UpdateAsync(transactionCodeId.Value, request);
                _logger.LogInformation("TransactionCode updated: {Id}", transactionCodeId.Value);
                return Json(new { success = true, messages = "Transaction code updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
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
