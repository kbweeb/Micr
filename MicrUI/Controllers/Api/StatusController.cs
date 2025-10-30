using Microsoft.AspNetCore.Mvc;
using MicrDbChequeProcessingSystem.Dtos;
using MicrDbChequeProcessingSystem.Services;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/status")]
public class StatusController : ControllerBase
{
    private readonly ISystemStatusService _systemStatusService;

    public StatusController(ISystemStatusService systemStatusService)
    {
        _systemStatusService = systemStatusService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(SystemStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var snapshot = await _systemStatusService.GetSnapshotAsync(cancellationToken);
        return Ok(snapshot);
    }
}
