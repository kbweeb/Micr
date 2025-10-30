using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Dtos;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/branches")]
public class BankBranchesController : ControllerBase
{
    private readonly MicrDbContext _context;

    public BankBranchesController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BankBranchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] long? bankId, CancellationToken cancellationToken)
    {
        var query = _context.BankBranches.AsNoTracking().Include(b => b.Bank).AsQueryable();
        if (bankId.HasValue)
        {
            query = query.Where(b => b.BankId == bankId);
        }

        var list = await query
            .Select(b => new BankBranchDto(
                b.BankBranchId,
                b.BankBranchName,
                b.BankId,
                b.Bank.BankName,
                b.IsEnabled,
                b.CreatedDate
            ))
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

        return Ok(list);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BankBranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var branch = await _context.BankBranches
            .AsNoTracking()
            .Include(b => b.Bank)
            .Where(b => b.BankBranchId == id)
            .Select(b => new BankBranchDto(
                b.BankBranchId,
                b.BankBranchName,
                b.BankId,
                b.Bank.BankName,
                b.IsEnabled,
                b.CreatedDate
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return branch is null ? NotFound() : Ok(branch);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BankBranchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(BankBranchUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var bankExists = await _context.Banks.AnyAsync(b => b.BankId == request.BankId, cancellationToken);
        if (!bankExists)
        {
            ModelState.AddModelError(nameof(request.BankId), "Bank not found");
            return ValidationProblem(ModelState);
        }

        var entity = new BankBranch
        {
            BankBranchName = request.BankBranchName.Trim(),
            BankId = request.BankId,
            IsEnabled = request.IsEnabled,
            CreatedByUserId = request.CreatedByUserId,
            CreatedDate = DateTime.UtcNow
        };

        _context.BankBranches.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return await Get(entity.BankBranchId, cancellationToken);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(BankBranchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, BankBranchUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = await _context.BankBranches.FirstOrDefaultAsync(b => b.BankBranchId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var bankExists = await _context.Banks.AnyAsync(b => b.BankId == request.BankId, cancellationToken);
        if (!bankExists)
        {
            ModelState.AddModelError(nameof(request.BankId), "Bank not found");
            return ValidationProblem(ModelState);
        }

        entity.BankBranchName = request.BankBranchName.Trim();
        entity.BankId = request.BankId;
        entity.IsEnabled = request.IsEnabled;

        await _context.SaveChangesAsync(cancellationToken);

        return await Get(id, cancellationToken);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.BankBranches.FirstOrDefaultAsync(b => b.BankBranchId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        _context.BankBranches.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
