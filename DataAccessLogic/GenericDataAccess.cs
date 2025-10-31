using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic;

public interface IGenericDataAccess<T> where T : class
{
    IQueryable<T> Queryable { get; }
    Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}

public class GenericDataAccess<T> : IGenericDataAccess<T> where T : class
{
    private readonly AppDataAccess _db;
    private readonly DbSet<T> _set;

    public GenericDataAccess(AppDataAccess db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public IQueryable<T> Queryable => _set.AsQueryable();

    public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
        => await _set.FindAsync(new[] { id }, ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}