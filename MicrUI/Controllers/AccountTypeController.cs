using BusinessLogic.Logic;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Models;
using MicrDbChequeProcessingSystem.ViewModels;

namespace MicrDbChequeProcessingSystem.Controllers;

public class AccountTypeController : Controller
{
    private readonly MicrDbContext _context;
    private readonly IAccountTypeService _service;

    public AccountTypeController(MicrDbContext context, IAccountTypeService service)
    {
        _context = context;
        _service = service;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AccountTypeCreateRequest());
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.AccountTypes
            .AsNoTracking()
            .OrderBy(a => a.AccountTypeName)
            .Select(a => new AccountTypeListItem
            {
                Name = a.AccountTypeName,
                Code = a.AccountTypeCode,
                Description = a.Description,
                Created = a.CreatedDate.ToString("dd MMM yyyy")
            })
            .ToListAsync();

        var viewModel = new AccountTypeIndexViewModel { Items = items };

        return View(viewModel);
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
            await _service.CreateAsync(dto);
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
    public async Task<IActionResult> Create(AccountTypeCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Please provide the required details." });
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
            var result = await _service.CreateAsync(dto);
            return Json(new
            {
                success = true,
                data = new
                {
                    id = result.AccountTypeId,
                    accountTypeName = result.AccountTypeName,
                    code = result.AccountTypeCode,
                    description = result.Description,
                    created = result.Created
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "We couldn't save the record. Please try again." });
        }
    }

    private async Task<long> ResolveCurrentUserId()
    {
        var winUser = Environment.UserName;
        var user = await _context.UserProfiles.AsNoTracking()
            .OrderBy(u => u.UserId)
            .FirstOrDefaultAsync(u => u.Username == winUser) ??
                   await _context.UserProfiles.AsNoTracking().OrderBy(u => u.UserId).FirstOrDefaultAsync();
        return user?.UserId ?? 1;
    }
}
