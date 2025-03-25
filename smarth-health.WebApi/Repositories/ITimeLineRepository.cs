namespace smarth_health.WebApi.Repositories;
using smarth_health.WebApi.Models;
public interface ITimelineRepository
{
    Task InsertAsync(Timeline timeline);
    Task<Timeline> ReadAsync(Guid timelineId);
    Task UpdateAsync(Timeline timeline);
    Task DeleteAsync(Guid timelineId);
}
