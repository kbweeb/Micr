using BusinessLogic.Logic;
using Domain.ViewModels.BankBranches;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BankBranchController : Controller
{
    private readonly IBankBranchService _service;
    private readonly IBankService _bankService;
    private readonly ILogger<BankBranchController> _logger;

    public BankBranchController(IBankBranchService service, IBankService bankService, ILogger<BankBranchController> logger)
    {
        _service = service;
        _bankService = bankService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
        var banks = await _bankService.GetIndexAsync();
        
        var items = list.Select(b => new BankBranchListItemViewModel
        {
            BankBranchId = b.BankBranchId,
            BankBranchName = b.BankBranchName,
            BankId = b.BankId,
            BankName = b.BankName,
            IsEnabled = b.IsEnabled,
            Created = b.Created
        }).ToList();

        ViewBag.Banks = banks;
        return View(new BankBranchIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(long? bankBranchId, BankBranchFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (bankBranchId.HasValue && bankBranchId.Value > 0)
            {
                var updated = await _service.UpdateAsync(bankBranchId.Value, request);
                _logger.LogInformation("BankBranch updated: {Id}", bankBranchId.Value);
                return Json(new { success = true, messages = "Bank branch updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
            _logger.LogInformation("BankBranch created: {Id}", created.BankBranchId);
            return Json(new { success = true, messages = "New bank branch added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bank branch");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
