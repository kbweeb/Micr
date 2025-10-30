using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Dtos;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/regions")]
public class RegionsController : ControllerBase
{
    private readonly MicrDbContext _context;

    public RegionsController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RegionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var regions = await _context.RegionZones
            .AsNoTracking()
            .Select(region => new RegionDto(
                region.RegionId,
                region.RegionName,
                region.Banks.Count,
                region.Banks.SelectMany(b => b.BankBranches).Count(),
                region.CreatedDate
            ))
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return Ok(regions);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RegionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var region = await _context.RegionZones
            .AsNoTracking()
            .Where(r => r.RegionId == id)
            .Select(region => new RegionDto(
                region.RegionId,
                region.RegionName,
                region.Banks.Count,
                region.Banks.SelectMany(b => b.BankBranches).Count(),
                region.CreatedDate
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (region is null)
        {
            return NotFound();
        }

        return Ok(region);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(RegionUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = new RegionZone
        {
            RegionName = request.RegionName.Trim(),
            CreatedByUserId = request.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        _context.RegionZones.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return await Get(entity.RegionId, cancellationToken);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(RegionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, RegionUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = await _context.RegionZones.FirstOrDefaultAsync(r => r.RegionId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.RegionName = request.RegionName.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return await Get(id, cancellationToken);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.RegionZones.Include(r => r.Banks).FirstOrDefaultAsync(r => r.RegionId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Banks.Any())
        {
            return Conflict(new { message = "Cannot delete a region that still has banks." });
        }

        _context.RegionZones.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
