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

    // HTML form submit handler
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateForm(AccountTypeCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", request);
        }

        try
        {
            var createdBy = await ResolveCurrentUserId();
            var dto = new AccountTypeCreateDto
            {
                AccountTypeName = request.AccountTypeName,
                Description = request.Description,
                CreatedByUserId = createdBy
            };
            await _appLogic.CreateAccountTypeAsync(dto);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "We couldn't save the record. Please try again.");
            return View("Create", request);
        }

        TempData["Message"] = "Account type created";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(long? accountTypeId, AccountTypeCreateRequest request)
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

            if (accountTypeId.HasValue && accountTypeId.Value > 0)
            {
                var updated = await _appLogic.UpdateAccountTypeAsync(accountTypeId.Value, dto);
                _logger.LogInformation("Account Type updated successfully: {Id}", accountTypeId.Value);
                return Json(new { Success = true, Messages = "Account Type updated successfully!", data = updated });
            }
            else
            {
                var created = await _appLogic.CreateAccountTypeAsync(dto);
                _logger.LogInformation("New Account Type added successfully: {Id}", created.AccountTypeId);
                return Json(new { Success = true, Messages = "New Account Type added successfully!", data = created });
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
