using BusinessLogic.Logic;
using Domain.ViewModels.AccountTypes;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class AccountTypeController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<AccountTypeController> _logger;

    public AccountTypeController(IMicrPortalService portal, ILogger<AccountTypeController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetAccountTypesAsync();
        var items = list.Select(a => new AccountTypeListItemViewModel
        {
            AccountTypeId = a.AccountTypeId,
            AccountTypeName = a.AccountTypeName,
            AccountTypeCode = a.AccountTypeCode,
            Description = a.Description,
            Created = a.Created
        }).ToList();

        return View(new AccountTypeIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(AccountTypeFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.AccountTypeId.HasValue && request.AccountTypeId.Value > 0)
            {
                var updated = await _portal.UpdateAccountTypeAsync(request.AccountTypeId.Value, request);
                _logger.LogInformation("AccountType updated: {Id}", request.AccountTypeId.Value);
                return Json(new { success = true, messages = "Account type updated successfully!", data = updated });
            }

            var created = await _portal.CreateAccountTypeAsync(request);
            _logger.LogInformation("AccountType created: {Id}", created.AccountTypeId);
            return Json(new { success = true, messages = "New account type added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving account type");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
