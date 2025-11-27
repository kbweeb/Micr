using BusinessLogic.Logic;
using Domain.ViewModels.NumberOfLeaflets;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class NumberOfLeafletController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<NumberOfLeafletController> _logger;

    public NumberOfLeafletController(IMicrPortalService portal, ILogger<NumberOfLeafletController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetNumberOfLeafletsAsync();
        var items = list.Select(n => new NumberOfLeafletListItemViewModel
        {
            NumberOfLeafletId = n.NumberOfLeafletId,
            NumberOfLeaflet = n.NumberOfLeaflet,
            Created = n.Created,
            CreatedBy = n.CreatedBy
        }).ToList();

        return View(new NumberOfLeafletIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(NumberOfLeafletFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.NumberOfLeafletId.HasValue && request.NumberOfLeafletId.Value > 0)
            {
                var updated = await _portal.UpdateNumberOfLeafletAsync(request.NumberOfLeafletId.Value, request);
                _logger.LogInformation("NumberOfLeaflet updated: {Id}", request.NumberOfLeafletId.Value);
                return Json(new { success = true, messages = "Number of leaflet updated successfully!", data = updated });
            }

            var created = await _portal.CreateNumberOfLeafletAsync(request);
            _logger.LogInformation("NumberOfLeaflet created: {Id}", created.NumberOfLeafletId);
            return Json(new { success = true, messages = "New number of leaflet added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving number of leaflet");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
