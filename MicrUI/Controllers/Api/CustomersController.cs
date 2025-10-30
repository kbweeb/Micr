using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace MicrDbChequeProcessingSystem.Controllers.Api;

[ApiController]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly MicrDbContext _context;

    public CustomersController(MicrDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? branchId, CancellationToken cancellationToken)
    {
        var query = _context.CustomerProfiles
            .AsNoTracking()
            .Include(c => c.RequestingBankBranch)
            .ThenInclude(bb => bb.Bank)
            .AsQueryable();

        if (branchId.HasValue)
        {
            query = query.Where(c => c.RequestingBankBranchId == branchId);
        }

        var payload = await query
            .Select(c => new
            {
                c.CustomerId,
                c.CustomerName,
                c.AccountNumber,
                Branch = c.RequestingBankBranch.BankBranchName,
                Bank = c.RequestingBankBranch.Bank.BankName,
                c.CreatedDate
            })
            .OrderBy(c => c.CustomerName)
            .ToListAsync(cancellationToken);

        return Ok(payload);
    }
}
