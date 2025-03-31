using Dapper;
using smarth_health.WebApi.Models;
using System.Data;

namespace smarth_health.WebApi.Repositories
{
    public class TimeLineItemRepository : ITimeLineItemRepository
    {
        private readonly IDbConnection _dbConnection;

        public TimeLineItemRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<TimeLineItem>> GetAllAsync()
        {
            var sql = "SELECT * FROM TimeLineItems";
            return await _dbConnection.QueryAsync<TimeLineItem>(sql);
        }

        public async Task<TimeLineItem> GetByIdAsync(Guid id)
        {
            var sql = "SELECT * FROM TimeLineItems WHERE ID = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<TimeLineItem>(sql, new { Id = id });
        }

        public async Task CreateAsync(TimeLineItem item)
        {
            SanitizeItemStrings(item);

            var sql = @"
            INSERT INTO TimeLineItems (ID, Content, Video, VideoPath, Medicine, ToolTipContent, ImagePath, Position, TimeLineID)
            VALUES (@ID, @Content, @Video, @VideoPath, @Medicine, @ToolTipContent, @ImagePath, @Position, @TimeLineID)";

            await _dbConnection.ExecuteAsync(sql, item);
        }

        public async Task UpdateAsync(TimeLineItem item)
        {
            SanitizeItemStrings(item);

            var sql = @"
            UPDATE TimeLineItems
            SET Content = @Content,
                Video = @Video,
                VideoPath = @VideoPath,
                Medicine = @Medicine,
                ToolTipContent = @ToolTipContent,
                ImagePath = @ImagePath,
                Position = @Position,
                TimeLineID = @TimeLineID
            WHERE ID = @ID";

            await _dbConnection.ExecuteAsync(sql, item);
        }

        public async Task DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM TimeLineItems WHERE ID = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        private void SanitizeItemStrings(TimeLineItem item)
        {
            item.Content = item.Content?.Trim();
            item.VideoPath = item.VideoPath?.Trim();
            item.ToolTipContent = item.ToolTipContent?.Trim();
            item.ImagePath = item.ImagePath?.Trim();
        }
    }
}