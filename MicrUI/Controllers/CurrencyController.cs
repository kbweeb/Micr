using BusinessLogic.Logic;
using Domain.ViewModels.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class CurrencyController : Controller
{
    private readonly ICurrencyService _service;
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(ICurrencyService service, ILogger<CurrencyController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
        var items = list.Select(c => new CurrencyListItemViewModel
        {
            CurrencyId = c.CurrencyId,
            CurrencyName = c.CurrencyName,
            CurrencyCode = c.CurrencyCode,
            Symbol = c.Symbol,
            IsActive = c.IsActive,
            Created = c.Created,
            CreatedBy = c.CreatedBy
        }).ToList();

        return View(new CurrencyIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(long? currencyId, CurrencyFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (currencyId.HasValue && currencyId.Value > 0)
            {
                var updated = await _service.UpdateAsync(currencyId.Value, request);
                _logger.LogInformation("Currency updated: {Id}", currencyId.Value);
                return Json(new { success = true, messages = "Currency updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
            _logger.LogInformation("Currency created: {Id}", created.CurrencyId);
            return Json(new { success = true, messages = "New currency added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving currency");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
