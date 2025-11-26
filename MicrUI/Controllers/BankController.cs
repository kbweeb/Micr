using BusinessLogic.Logic;
using Domain.ViewModels.Banks;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BankController : Controller
{
    private readonly IBankService _service;
    private readonly IRegionService _regionService;
    private readonly ILogger<BankController> _logger;

    public BankController(IBankService service, IRegionService regionService, ILogger<BankController> logger)
    {
        _service = service;
        _regionService = regionService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
        var regions = await _regionService.GetIndexAsync();
        
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
    public async Task<JsonResult> CreateUpdate(long? bankId, BankFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (bankId.HasValue && bankId.Value > 0)
            {
                var updated = await _service.UpdateAsync(bankId.Value, request);
                _logger.LogInformation("Bank updated: {Id}", bankId.Value);
                return Json(new { success = true, messages = "Bank updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
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
