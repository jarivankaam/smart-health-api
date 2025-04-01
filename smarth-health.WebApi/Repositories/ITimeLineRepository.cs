using smarth_health.WebApi.Models;

namespace smarth_health.WebApi.Repositories
{
    public interface ITimelineRepository
    {
        Task InsertAsync(Timeline timeline);
        Task<Timeline> GetByTimeLineByIdAsync(Guid timelineId);
        Task<Timeline> GetByTimeLineByUserIdAsync(Guid userId);
        Task UpdateAsync(Timeline timeline);
        Task DeleteAsync(Guid timelineId);
    }
}


