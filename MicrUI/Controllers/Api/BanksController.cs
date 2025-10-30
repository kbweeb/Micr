using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Dtos;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/banks")]
public class BanksController : ControllerBase
{
    private readonly MicrDbContext _context;

    public BanksController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BankDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var banks = await _context.Banks
            .AsNoTracking()
            .Include(b => b.Region)
            .Include(b => b.BankBranches)
            .OrderBy(b => b.BankName)
            .ToListAsync(cancellationToken);

        var dtos = banks.Select(b => new BankDto(
            b.BankId,
            b.BankName,
            b.SortCode,
            b.RegionId,
            b.Region?.RegionName ?? string.Empty,
            b.IsEnabled ?? true,
            b.BankBranches.Count,
            b.CreatedDate
        )).ToList();

        return Ok(dtos);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.Banks
            .AsNoTracking()
            .Include(b => b.Region)
            .Include(b => b.BankBranches)
            .SingleOrDefaultAsync(b => b.BankId == id, cancellationToken);

        if (entity is null)
        {
            return NotFound();
        }

        var dto = new BankDto(
            entity.BankId,
            entity.BankName,
            entity.SortCode,
            entity.RegionId,
            entity.Region?.RegionName ?? string.Empty,
            entity.IsEnabled ?? true,
            entity.BankBranches.Count,
            entity.CreatedDate
        );

        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(BankUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var regionExists = await _context.RegionZones.AnyAsync(r => r.RegionId == request.RegionId, cancellationToken);
        if (!regionExists)
        {
            ModelState.AddModelError(nameof(request.RegionId), "Region not found");
            return ValidationProblem(ModelState);
        }

        var entity = new Bank
        {
            BankName = request.BankName,
            SortCode = request.SortCode,
            RegionId = request.RegionId,
            IsEnabled = request.IsEnabled ?? true,
            CreatedByUserId = request.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        _context.Banks.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Banks
            .AsNoTracking()
            .Include(b => b.Region)
            .Include(b => b.BankBranches)
            .SingleAsync(b => b.BankId == entity.BankId, cancellationToken);

        var dto = new BankDto(
            created.BankId,
            created.BankName,
            created.SortCode,
            created.RegionId,
            created.Region?.RegionName ?? string.Empty,
            created.IsEnabled ?? true,
            created.BankBranches.Count,
            created.CreatedDate
        );

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(BankDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, BankUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = await _context.Banks.FirstOrDefaultAsync(b => b.BankId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var regionExists = await _context.RegionZones.AnyAsync(r => r.RegionId == request.RegionId, cancellationToken);
        if (!regionExists)
        {
            ModelState.AddModelError(nameof(request.RegionId), "Region not found");
            return ValidationProblem(ModelState);
        }

        entity.BankName = request.BankName;
        entity.SortCode = request.SortCode;
        entity.RegionId = request.RegionId;
        entity.IsEnabled = request.IsEnabled ?? entity.IsEnabled;

        await _context.SaveChangesAsync(cancellationToken);

        return await GetById(id, cancellationToken);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.Banks.Include(b => b.BankBranches).FirstOrDefaultAsync(b => b.BankId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.BankBranches.Any())
        {
            return Conflict(new { message = "Cannot delete bank that still has branches." });
        }

        _context.Banks.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
