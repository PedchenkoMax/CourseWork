using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IDbConnection connection;

        public BrandRepository(DbContext context)
        {
            connection = context.Connection;
        }

        public async Task<List<BrandEntity>> GetAllAsync()
        {
            const string sql =
                """
                SELECT * FROM "Brands"
                """;

            var res = await connection.QueryAsync<BrandEntity>(sql);

            return res.ToList();
        }

        public async Task<BrandEntity?> GetByIdAsync(Guid id)
        {
            const string sql =
                """
                SELECT * FROM "Brands" WHERE "Id" = @Id
                """;

            var res = await connection.QuerySingleOrDefaultAsync<BrandEntity>(sql, new { Id = id });

            return res;
        }

        public async Task<bool> UpdateAsync(BrandEntity brand)
        {
            // TODO: only display order could be updatable  ??
            const string sql =
                """
                UPDATE "Brands" SET
                    "Name" = @Name,
                    "Description" = @Description,
                    "ImageUrl" = @ImageUrl,
                    "DisplayOrder" = @DisplayOrder
                WHERE "Id" = @Id
                """;

            var rowsAffected = await connection.ExecuteAsync(sql, brand);

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveByIdAsync(Guid id)
        {
            const string sql =
                """
                DELETE FROM "Brands" WHERE "Id" = @Id
                """;

            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

            return rowsAffected > 0;
        }

        public async Task<bool> AddAsync(BrandEntity brand)
        {
            const string sql =
                """
                INSERT INTO "Brands" ("Id", "Name", "Description", "ImageUrl", "DisplayOrder")
                VALUES (@Id, @Name, @Description, @ImageUrl, @DisplayOrder)
                """;

            var rowsAffected = await connection.ExecuteAsync(sql, brand);

            return rowsAffected > 0;
        }
    }
}