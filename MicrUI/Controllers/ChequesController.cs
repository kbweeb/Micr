using BusinessLogic.Logic;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class ChequesController : Controller
{
    private readonly IChequeService _chequeService;

    public ChequesController(IChequeService chequeService)
    {
        _chequeService = chequeService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ChequeDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChequeDto model, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.Number))
        {
            ModelState.AddModelError(nameof(model.Number), "Number is required");
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _chequeService.CreateAsync(model, ct);
        TempData["Message"] = "Cheque saved";
        return RedirectToAction("Index", "Home");
    }
}
