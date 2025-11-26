using BusinessLogic.Logic;
using Domain.ViewModels.Regions;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class RegionController : Controller
{
    private readonly IRegionService _service;
    private readonly ILogger<RegionController> _logger;

    public RegionController(IRegionService service, ILogger<RegionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
        var items = list.Select(r => new RegionListItemViewModel
        {
            RegionId = r.RegionId,
            RegionName = r.RegionName,
            Description = r.Description,
            Created = r.Created,
            Banks = r.Banks,
            Branches = r.Branches
        }).ToList();

        return View(new RegionIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(long? regionId, RegionFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (regionId.HasValue && regionId.Value > 0)
            {
                var updated = await _service.UpdateAsync(regionId.Value, request);
                _logger.LogInformation("Region updated: {Id}", regionId.Value);
                return Json(new { success = true, messages = "Region updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
            _logger.LogInformation("Region created: {Id}", created.RegionId);
            return Json(new { success = true, messages = "New region added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving region");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
