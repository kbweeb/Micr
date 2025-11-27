using BusinessLogic.Logic;
using Domain.ViewModels.Regions;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class RegionController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<RegionController> _logger;

    public RegionController(IMicrPortalService portal, ILogger<RegionController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetRegionsAsync();
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
    public async Task<JsonResult> CreateUpdate(RegionFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.RegionId.HasValue && request.RegionId.Value > 0)
            {
                var updated = await _portal.UpdateRegionAsync(request.RegionId.Value, request);
                _logger.LogInformation("Region updated: {Id}", request.RegionId.Value);
                return Json(new { success = true, messages = "Region updated successfully!", data = updated });
            }

            var created = await _portal.CreateRegionAsync(request);
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
