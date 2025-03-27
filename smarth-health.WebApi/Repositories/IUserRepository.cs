using smarth_health.WebApi.Models;

namespace smarth_health.WebApi.Repositories
{
    public interface IUserRepository
    {
        public Task InsertAsync(User user);
        public Task<User> ReadAsync(Guid identityUserId);
        public Task UpdateAsync(User user);
        public Task DeleteAsync(Guid userId);
    }
}
