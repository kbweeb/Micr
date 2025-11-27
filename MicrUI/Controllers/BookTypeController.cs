using BusinessLogic.Logic;
using Domain.ViewModels.BookTypes;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BookTypeController : Controller
{
    private readonly IMicrPortalService _portal;
    private readonly ILogger<BookTypeController> _logger;

    public BookTypeController(IMicrPortalService portal, ILogger<BookTypeController> logger)
    {
        _portal = portal;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _portal.GetBookTypesAsync();
        var accountTypes = await _portal.GetAccountTypesAsync();
        var leaflets = await _portal.GetNumberOfLeafletsAsync();
        var transactionCodes = await _portal.GetTransactionCodesAsync();
        
        var items = list.Select(b => new BookTypeListItemViewModel
        {
            BookTypeId = b.BookTypeId,
            BookTypeCode = b.BookTypeCode,
            BookTypeName = b.BookTypeName,
            AccountTypeId = b.AccountTypeId,
            AccountTypeName = b.AccountTypeName,
            NumberOfLeafletId = b.NumberOfLeafletId,
            NumberOfLeaflet = b.NumberOfLeaflet,
            TransactionCodeId = b.TransactionCodeId,
            TransactionCode = b.TransactionCode,
            Created = b.Created
        }).ToList();

        ViewBag.AccountTypes = accountTypes;
        ViewBag.Leaflets = leaflets;
        ViewBag.TransactionCodes = transactionCodes;
        return View(new BookTypeIndexViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<JsonResult> CreateUpdate(BookTypeFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (request.BookTypeId.HasValue && request.BookTypeId.Value > 0)
            {
                var updated = await _portal.UpdateBookTypeAsync(request.BookTypeId.Value, request);
                _logger.LogInformation("BookType updated: {Id}", request.BookTypeId.Value);
                return Json(new { success = true, messages = "Book type updated successfully!", data = updated });
            }

            var created = await _portal.CreateBookTypeAsync(request);
            _logger.LogInformation("BookType created: {Id}", created.BookTypeId);
            return Json(new { success = true, messages = "New book type added successfully!", data = created });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving book type");
            return Json(new { success = false, messages = $"Error: {ex.Message}" });
        }
    }
}
