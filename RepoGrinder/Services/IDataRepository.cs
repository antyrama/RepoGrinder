namespace RepoGrinder.Services
{
    public interface IDataRepository<TEntity>
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(string id);
        Task<IEnumerable<TEntity>> GetAsync();
    }
}
