using BusinessLogic.Logic;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.ViewModels;

namespace MicrDbChequeProcessingSystem.Controllers;

public class AccountTypeController : Controller
{
    private readonly IApplicationLogic _appLogic;
    private readonly ILogger<AccountTypeController> _logger;

    public AccountTypeController(IApplicationLogic appLogic, ILogger<AccountTypeController> logger)
    {
        _appLogic = appLogic;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AccountTypeCreateRequest());
    }

    public async Task<IActionResult> Index()
    {
        var list = await _appLogic.GetAccountTypesAsync();
        var items = list.Select(a => new AccountTypeListItem
        {
            Id = a.AccountTypeId,
            Name = a.AccountTypeName,
            Code = a.AccountTypeCode,
            Description = a.Description,
            Created = a.Created
        }).ToList();

        return View(new AccountTypeIndexViewModel { Items = items });
    }

    [HttpGet]
    public async Task<JsonResult> GetAccountTypeData()
    {
        var list = await _appLogic.GetAccountTypesAsync();
        var data = list.Select(a => new
        {
            accountTypeId = a.AccountTypeId,
            accountTypeName = a.AccountTypeName,
            description = a.Description,
            code = a.AccountTypeCode,
            created = a.Created
        }).ToList();
        return Json(new { data, Success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdateAccountType(long? accountTypeId, AccountTypeCreateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new ResponseMessage { Success = false, Messages = "Please provide the required details." });
            }

            var createdBy = await ResolveCurrentUserId();
            var dto = new AccountTypeCreateDto
            {
                AccountTypeName = request.AccountTypeName,
                Description = request.Description,
                CreatedByUserId = createdBy
            };

            // Basic duplicate/name validation mirroring legacy behavior
            var existing = await _appLogic.GetAccountTypesAsync();
            var normalizedName = (request.AccountTypeName ?? string.Empty).Trim().ToLowerInvariant();

            if (accountTypeId.HasValue && accountTypeId.Value > 0)
            {
                // Validate target exists
                if (!existing.Any(a => a.AccountTypeId == accountTypeId.Value))
                {
                    return Json(new ResponseMessage { Success = false, Messages = "Selected Account Type was not found." });
                }

                // Validate uniqueness (exclude self)
                if (existing.Any(a => (a.AccountTypeName ?? string.Empty).Trim().ToLowerInvariant() == normalizedName && a.AccountTypeId != accountTypeId.Value))
                {
                    return Json(new ResponseMessage { Success = false, Messages = "Account Type name already exists." });
                }

                await _appLogic.UpdateAccountTypeAsync(accountTypeId.Value, dto);
                _logger.LogInformation("Account Type updated successfully: {Id}", accountTypeId.Value);
                return Json(new ResponseMessage { Success = true, Messages = "Account Type updated successfully!" });
            }
            else
            {
                // Validate uniqueness for create
                if (existing.Any(a => (a.AccountTypeName ?? string.Empty).Trim().ToLowerInvariant() == normalizedName))
                {
                    return Json(new ResponseMessage { Success = false, Messages = "Account Type name already exists." });
                }

                var created = await _appLogic.CreateAccountTypeAsync(dto);
                _logger.LogInformation("New Account Type added successfully: {Id}", created.AccountTypeId);
                return Json(new ResponseMessage { Success = true, Messages = "New Account Type added successfully!" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating Account Type");
            return Json(new ResponseMessage { Success = false, Messages = "Could not add/update Account Type. Please contact the system administrator." });
        }
    }

    private Task<long> ResolveCurrentUserId() => Task.FromResult(1L);
}
