using BusinessLogic.Logic;
using Domain.ViewModels.BankBranches;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BankBranchController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<BankBranchController> _logger;

    public BankBranchController(IMicrPortalService portal, ILogger<BankBranchController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetBankBranchesAsync();
        var banks = await _portal.GetBanksAsync();
        
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
    public async Task<JsonResult> CreateUpdate(BankBranchFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.BankBranchId.HasValue && request.BankBranchId.Value > 0)
            {
                var updated = await _portal.UpdateBankBranchAsync(request.BankBranchId.Value, request);
                _logger.LogInformation("BankBranch updated: {Id}", request.BankBranchId.Value);
                return Json(new { success = true, messages = "Bank branch updated successfully!", data = updated });
            }

            var created = await _portal.CreateBankBranchAsync(request);
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
