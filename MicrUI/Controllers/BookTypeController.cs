using BusinessLogic.Logic;
using Domain.ViewModels.BookTypes;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class BookTypeController : Controller
{
    private readonly IBookTypeService _service;
    private readonly IAccountTypeService _accountTypeService;
    private readonly INumberOfLeafletService _leafletService;
    private readonly ITransactionCodeService _transactionCodeService;
    private readonly ILogger<BookTypeController> _logger;

    public BookTypeController(
        IBookTypeService service, 
        IAccountTypeService accountTypeService,
        INumberOfLeafletService leafletService,
        ITransactionCodeService transactionCodeService,
        ILogger<BookTypeController> logger)
    {
        _service = service;
        _accountTypeService = accountTypeService;
        _leafletService = leafletService;
        _transactionCodeService = transactionCodeService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _service.GetIndexAsync();
        var accountTypes = await _accountTypeService.GetIndexAsync();
        var leaflets = await _leafletService.GetIndexAsync();
        var transactionCodes = await _transactionCodeService.GetIndexAsync();
        
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
    public async Task<JsonResult> CreateUpdate(long? bookTypeId, BookTypeFormViewModel request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, messages = "Please provide the required details." });
            }

            if (bookTypeId.HasValue && bookTypeId.Value > 0)
            {
                var updated = await _service.UpdateAsync(bookTypeId.Value, request);
                _logger.LogInformation("BookType updated: {Id}", bookTypeId.Value);
                return Json(new { success = true, messages = "Book type updated successfully!", data = updated });
            }

            var created = await _service.CreateAsync(request);
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
