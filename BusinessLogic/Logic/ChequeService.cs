using DataAccessLogic;
using Domain.DataTables;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Logic;

public interface IChequeService
{
    Task<ChequeDto> CreateAsync(ChequeDto dto, CancellationToken ct = default);
}

public class ChequeService : IChequeService
{
    private readonly AppDataAccess _db;

    public ChequeService(AppDataAccess db) => _db = db;

    public async Task<ChequeDto> CreateAsync(ChequeDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Number))
            throw new ArgumentException("Cheque number is required", nameof(dto.Number));
        if (dto.Amount < 0)
            throw new ArgumentException("Amount must be >= 0", nameof(dto.Amount));

        var entity = new Cheque
        {
            Number = dto.Number?.Trim(),
            Amount = dto.Amount
        };
        await _db.Cheques.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);

        return new ChequeDto
        {
            Id = entity.Id,
            Number = entity.Number,
            Amount = entity.Amount
        };
    }
}