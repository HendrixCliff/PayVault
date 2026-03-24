namespace PayVault.Application.Interfaces.Repositories
{
   public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, bool includeRelated = false);

    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
}