using System.Data;
using Dapper;
using smarth_health.WebApi.Models;

namespace smarth_health.WebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InsertAsync(User user)
        {
            var sql = @"
            INSERT INTO [dbo].[Users] (ID, IdentityUserId, DisplayName, ProfilePhotoPath)
            VALUES (@ID, @IdentityUserId, @DisplayName, @ProfilePhotoPath)";
            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task<User> ReadAsync(Guid identityUserId)
        {
            var sql = @"
            SELECT 
                ID,
                IdentityUserId,
                DisplayName,
                ProfilePhotoPath
            FROM
                [dbo].[Users]
            WHERE
                IdentityUserId = @IdentityUserId";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { IdentityUserId = identityUserId });
        }
    
        public async Task UpdateAsync(User user)
        {
            var sql = @"
            UPDATE 
                [dbo].[Users]
            SET 
                DisplayName = @DisplayName
                ProfilePhotoPath = @ProfilePhotoPath
            WHERE 
                ID = @ID";

            await _dbConnection.ExecuteAsync(sql, user);
        }

        public async Task DeleteAsync(Guid userId)
        {
            await _dbConnection.ExecuteAsync("DELETE FROM [dbo].[Users] WHERE ID = @UserId", new { UserId = userId });
        }
    }
}
