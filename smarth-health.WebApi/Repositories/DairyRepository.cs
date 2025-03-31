using Dapper;
using smarth_health.WebApi.Models;
using System.Data;

namespace smarth_health.WebApi.Repositories
{
    public class DairyRepository : IDairyRepository
    {
        private readonly IDbConnection _dbConnection;

        public DairyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Dairy>> GetAllAsync()
        {
            var sql = "SELECT * FROM Dairy";
            return await _dbConnection.QueryAsync<Dairy>(sql);
        }

        public async Task<Dairy> GetByIdAsync(Guid id)
        {
            var sql = "SELECT * FROM Dairy WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Dairy>(sql, new { Id = id });
        }

        public async Task CreateAsync(Dairy dairy)
        {
            dairy.Id = Guid.NewGuid();
            dairy.Content = dairy.Content?.Trim();

            var sql = @"
            INSERT INTO Dairy (Id, Content, UserId)
            VALUES (@Id, @Content, @UserId)";

            await _dbConnection.ExecuteAsync(sql, dairy);
        }

        public async Task UpdateAsync(Dairy dairy)
        {
            dairy.Content = dairy.Content?.Trim();

            var sql = @"
            UPDATE Dairy
            SET Content = @Content,
                UserId = @UserId
            WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(sql, dairy);
        }

        public async Task DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM Dairy WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
