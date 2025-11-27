using BusinessLogic.Logic;
using Domain.ViewModels.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class CurrencyController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(IMicrPortalService portal, ILogger<CurrencyController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetCurrenciesAsync();
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
    public async Task<JsonResult> CreateUpdate(CurrencyFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.CurrencyId.HasValue && request.CurrencyId.Value > 0)
            {
                var updated = await _portal.UpdateCurrencyAsync(request.CurrencyId.Value, request);
                _logger.LogInformation("Currency updated: {Id}", request.CurrencyId.Value);
                return Json(new { success = true, messages = "Currency updated successfully!", data = updated });
            }

            var created = await _portal.CreateCurrencyAsync(request);
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
