using Domain.ViewModels.BookTypes;
using Microsoft.EntityFrameworkCore;
using MicrDbChequeProcessingSystem.Data;

namespace BusinessLogic.Logic;

public interface IBookTypeService
{
    Task<List<BookTypeDto>> GetIndexAsync(CancellationToken ct = default);
    Task<BookTypeDto> CreateAsync(BookTypeFormViewModel form, CancellationToken ct = default);
    Task<BookTypeDto> UpdateAsync(long bookTypeId, BookTypeFormViewModel form, CancellationToken ct = default);
}

public class BookTypeService : IBookTypeService
{
    private readonly MicrDbContext _context;

    public BookTypeService(MicrDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookTypeDto>> GetIndexAsync(CancellationToken ct = default)
    {
        return await _context.BookTypes
            .Include(b => b.AccountType)
            .Include(b => b.NumberOfLeaflet)
            .Include(b => b.TransactionCode)
            .AsNoTracking()
            .OrderBy(b => b.BookTypeName)
            .Select(b => new BookTypeDto
            {
                BookTypeId = b.BookTypeId,
                BookTypeCode = b.BookTypeCode,
                BookTypeName = b.BookTypeName,
                AccountTypeId = b.AccountTypeId,
                AccountTypeName = b.AccountType.AccountTypeName,
                NumberOfLeafletId = b.NumberOfLeafletId,
                NumberOfLeaflet = b.NumberOfLeaflet.NumberOfLeaflet1,
                TransactionCodeId = b.TransactionCodeId,
                TransactionCode = b.TransactionCode.Code,
                Created = b.CreatedDate.ToString("dd MMM yyyy")
            })
            .ToListAsync(ct);
    }

    public async Task<BookTypeDto> CreateAsync(BookTypeFormViewModel form, CancellationToken ct = default)
    {
        var userId = form.CreatedByUserId ?? await _context.UserProfiles.Select(u => u.UserId).FirstOrDefaultAsync(ct);
        if (userId == 0) userId = 1;

        var entity = new MicrDbChequeProcessingSystem.Models.BookType
        {
            BookTypeCode = form.BookTypeCode.Trim(),
            BookTypeName = form.BookTypeName.Trim(),
            AccountTypeId = form.AccountTypeId,
            NumberOfLeafletId = form.NumberOfLeafletId,
            TransactionCodeId = form.TransactionCodeId,
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.BookTypes.Add(entity);
        await _context.SaveChangesAsync(ct);

        var accountType = await _context.AccountTypes.FindAsync(new object[] { form.AccountTypeId }, ct);
        var leaflet = await _context.NumberOfLeaflets.FindAsync(new object[] { form.NumberOfLeafletId }, ct);
        var transCode = await _context.TransactionCodes.FindAsync(new object[] { form.TransactionCodeId }, ct);

        return new BookTypeDto
        {
            BookTypeId = entity.BookTypeId,
            BookTypeCode = entity.BookTypeCode,
            BookTypeName = entity.BookTypeName,
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = accountType?.AccountTypeName,
            NumberOfLeafletId = entity.NumberOfLeafletId,
            NumberOfLeaflet = leaflet?.NumberOfLeaflet1,
            TransactionCodeId = entity.TransactionCodeId,
            TransactionCode = transCode?.Code,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }

    public async Task<BookTypeDto> UpdateAsync(long bookTypeId, BookTypeFormViewModel form, CancellationToken ct = default)
    {
        var entity = await _context.BookTypes
            .Include(b => b.AccountType)
            .Include(b => b.NumberOfLeaflet)
            .Include(b => b.TransactionCode)
            .FirstOrDefaultAsync(b => b.BookTypeId == bookTypeId, ct)
            ?? throw new InvalidOperationException("Book type not found");

        entity.BookTypeCode = form.BookTypeCode.Trim();
        entity.BookTypeName = form.BookTypeName.Trim();
        entity.AccountTypeId = form.AccountTypeId;
        entity.NumberOfLeafletId = form.NumberOfLeafletId;
        entity.TransactionCodeId = form.TransactionCodeId;

        await _context.SaveChangesAsync(ct);

        return new BookTypeDto
        {
            BookTypeId = entity.BookTypeId,
            BookTypeCode = entity.BookTypeCode,
            BookTypeName = entity.BookTypeName,
            AccountTypeId = entity.AccountTypeId,
            AccountTypeName = entity.AccountType.AccountTypeName,
            NumberOfLeafletId = entity.NumberOfLeafletId,
            NumberOfLeaflet = entity.NumberOfLeaflet.NumberOfLeaflet1,
            TransactionCodeId = entity.TransactionCodeId,
            TransactionCode = entity.TransactionCode.Code,
            Created = entity.CreatedDate.ToString("dd MMM yyyy")
        };
    }
}
