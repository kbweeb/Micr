using BusinessLogic.Logic;
using Domain.ViewModels.NumberOfLeaflets;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class NumberOfLeafletController : Controller
{
    private readonly INumberOfLeafletService _service;
    private readonly ILogger<NumberOfLeafletController> _logger;

    public NumberOfLeafletController(INumberOfLeafletService service, ILogger<NumberOfLeafletController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
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
    public async Task<JsonResult> CreateUpdate(long? numberOfLeafletId, NumberOfLeafletFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (numberOfLeafletId.HasValue && numberOfLeafletId.Value > 0)
            {
                var updated = await _service.UpdateAsync(numberOfLeafletId.Value, request);
                _logger.LogInformation("NumberOfLeaflet updated: {Id}", numberOfLeafletId.Value);
                return Json(new { success = true, messages = "Number of leaflet updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
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
