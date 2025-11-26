using Microsoft.AspNetCore.Mvc;

namespace MicrDbChequeProcessingSystem.Controllers;

public class ChequesController : Controller
{
    public ChequesController()
    {
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(object model)
    {
        // TODO: Implement cheque creation
        TempData["Message"] = "Cheque saved";
        return RedirectToAction("Index", "Home");
    }
}
