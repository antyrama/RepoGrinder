using System.Collections.Concurrent;
using RepoGrinder.DataModels;
#pragma warning disable CS1998

namespace RepoGrinder.Services
{
    public class MemoryDataRepository : IDataRepository<CodeStats>
    {
        private readonly ConcurrentDictionary<string, CodeStats> _data = new();

        public async Task AddAsync(CodeStats entity)
        {
            _data.AddOrUpdate(entity.Id, entity, (_, _) => null!);
        }

        public async Task UpdateAsync(CodeStats entity)
        {
            _data.AddOrUpdate(entity.Id, entity, (_, _) => entity);
        }

        public async Task<CodeStats?> GetByIdAsync(string id)
        {
            return _data.ContainsKey(id) ? _data[id] : null;
        }

        public async Task<IEnumerable<CodeStats>> GetAsync()
        {
            return _data.Values.ToArray();
        }
    }
}
