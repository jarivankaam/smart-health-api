using smarth_health.WebApi.Models;

namespace smarth_health.WebApi.Repositories
{
    public interface IDairyRepository
    {
        Task<IEnumerable<Dairy>> GetAllAsync();
        Task<Dairy> GetByIdAsync(Guid id);
        Task CreateAsync(Dairy dairy);
        Task UpdateAsync(Dairy dairy);
        Task DeleteAsync(Guid id);
    }
}