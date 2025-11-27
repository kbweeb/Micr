using BusinessLogic.Logic;
using Domain.ViewModels.Banks;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BankController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<BankController> _logger;

    public BankController(IMicrPortalService portal, ILogger<BankController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetBanksAsync();
        var regions = await _portal.GetRegionsAsync();
        
        var items = list.Select(b => new BankListItemViewModel
        {
            BankId = b.BankId,
            BankName = b.BankName,
            SortCode = b.SortCode,
            RegionId = b.RegionId,
            RegionName = b.RegionName,
            IsEnabled = b.IsEnabled,
            Created = b.Created
        }).ToList();

        ViewBag.Regions = regions;
        return View(new BankIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(BankFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.BankId.HasValue && request.BankId.Value > 0)
            {
                var updated = await _portal.UpdateBankAsync(request.BankId.Value, request);
                _logger.LogInformation("Bank updated: {Id}", request.BankId.Value);
                return Json(new { success = true, messages = "Bank updated successfully!", data = updated });
            }

            var created = await _portal.CreateBankAsync(request);
            _logger.LogInformation("Bank created: {Id}", created.BankId);
            return Json(new { success = true, messages = "New bank added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bank");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
