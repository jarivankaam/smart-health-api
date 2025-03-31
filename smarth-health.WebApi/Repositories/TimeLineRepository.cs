using Dapper;
using smarth_health.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace smarth_health.WebApi.Repositories
{
    public class TimelineRepository : ITimelineRepository
    {
        private readonly IDbConnection _dbConnection;

        public TimelineRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Timeline>> GetAllAsync()
        {
            var sql = "SELECT * FROM Timeline";
            return await _dbConnection.QueryAsync<Timeline>(sql);
        }

        public async Task<Timeline> ReadAsync(Guid id)
        {
            var sql = "SELECT Id, Name, RouteType, UserID FROM Timeline WHERE Id = @Id";
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Timeline>(sql, new { Id = id });

            if (result == null)
            {
                Console.WriteLine($"No timeline found for Id: {id}");
            }
            else
            {
                Console.WriteLine($"Timeline found: {result.ID}, {result.Name}, {result.RouteType}");
            }

            return result;
        }

        public async Task InsertAsync(Timeline timeline)
        {
            var sql = @"
            INSERT INTO Timeline (Id, Name, RouteType, UserID)
            VALUES (@Id, @Name, @RouteType, @UserID)";

            await _dbConnection.ExecuteAsync(sql, timeline);
        }

        public async Task UpdateAsync(Timeline timeline)
        {
            var sql = @"
            UPDATE Timeline
            SET Name = @Name,
                RouteType = @RouteType,
                UserID = @UserID
            WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(sql, timeline);
        }

        public async Task DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM Timeline WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
