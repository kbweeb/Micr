using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;
using MicrDbChequeProcessingSystem.Dtos;
using MicrDbChequeProcessingSystem.Models;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/account-types")]
public class AccountTypesController : ControllerBase
{
    private readonly MicrDbContext _context;

    public AccountTypesController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await _context.AccountTypes
            .AsNoTracking()
            .Select(a => new AccountTypeDto(
                a.AccountTypeId,
                a.AccountTypeName,
                a.AccountTypeCode,
                a.IsActive,
                a.CreatedDate
            ))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return Ok(list);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var dto = await _context.AccountTypes
            .AsNoTracking()
            .Where(a => a.AccountTypeId == id)
            .Select(a => new AccountTypeDto(
                a.AccountTypeId,
                a.AccountTypeName,
                a.AccountTypeCode,
                a.IsActive,
                a.CreatedDate
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(AccountTypeUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var creatorId = request.CreatedByUserId;
        if (creatorId is null)
        {
            creatorId = await _context.UserProfiles
                .AsNoTracking()
                .Select(u => (long?)u.UserId)
                .OrderBy(id => id)
                .FirstOrDefaultAsync(cancellationToken);

            if (creatorId is null)
            {
                ModelState.AddModelError(nameof(request.CreatedByUserId), "No user available to attribute record creation.");
                return ValidationProblem(ModelState);
            }
        }

        var entity = new AccountType
        {
            AccountTypeName = request.AccountTypeName.Trim(),
            AccountTypeCode = request.AccountTypeCode.Trim(),
            IsActive = request.IsActive,
            CreatedByUserId = creatorId.Value,
            CreatedDate = DateTime.UtcNow
        };

        _context.AccountTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return await Get(entity.AccountTypeId, cancellationToken);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(AccountTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, AccountTypeUpsertRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = await _context.AccountTypes.FirstOrDefaultAsync(a => a.AccountTypeId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.AccountTypeName = request.AccountTypeName.Trim();
        entity.AccountTypeCode = request.AccountTypeCode.Trim();
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return await Get(id, cancellationToken);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var entity = await _context.AccountTypes.FirstOrDefaultAsync(a => a.AccountTypeId == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var isReferenced = await _context.BookTypes.AnyAsync(b => b.AccountTypeId == id, cancellationToken);
        if (isReferenced)
        {
            return Conflict(new { message = "Cannot delete account type while cheque books reference it." });
        }

        _context.AccountTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
