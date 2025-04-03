using smarth_health.WebApi.Models;

namespace smarth_health.WebApi.Repositories
{
    public interface ITimeLineItemRepository
    {
        Task<IEnumerable<TimeLineItem>>? GetAllAsync(Guid timeLineId);
        Task<TimeLineItem> GetByIdAsync(Guid id);
        Task<TimeLineItem> GetByType(string type);
        Task CreateAsync(TimeLineItem item);
        Task UpdateAsync(TimeLineItem item);
        Task DeleteAsync(Guid id);
    }
}