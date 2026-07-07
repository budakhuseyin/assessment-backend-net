namespace ReportService.Application.Interfaces.Repositories;

/// <summary>
/// Tüm entity türleri için ortak CRUD işlemlerini tanımlayan generic repository arayüzü.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
