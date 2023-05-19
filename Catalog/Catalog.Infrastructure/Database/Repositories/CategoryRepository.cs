using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnection connection;

    public CategoryRepository(DbContext context)
    {
        connection = context.Connection;
    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        const string sql =
            """
            SELECT * FROM categories
            """;

        var res = await connection.QueryAsync<CategoryEntity>(sql);

        return res.ToList();
    }

    public async Task<List<CategoryEntity>> GetChildrenByParentCategoryId(Guid parentCategoryId)
    {
        const string sql =
            """
            SELECT * FROM categories WHERE parent_category_id = @Id
            """;

        var res = await connection.QueryAsync<CategoryEntity>(sql, new { Id = parentCategoryId });

        return res.ToList();
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT * FROM categories WHERE id = @Id
            """;

        var res = await connection.QuerySingleOrDefaultAsync<CategoryEntity>(sql, new { Id = id });

        return res;
    }

    public async Task<bool> UpdateAsync(CategoryEntity category)
    {
        const string sql =
            """
            UPDATE categories SET
                parent_category_id = @ParentCategoryId,
                name = @Name,
                description = @Description,
                image_url = @ImageUrl,
                display_order = @DisplayOrder
            WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, category);

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        const string sql =
            """
            DELETE FROM categories WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(CategoryEntity category)
    {
        const string sql =
            """
            INSERT INTO categories (id, parent_category_id, name, description, image_url, display_order)
            VALUES (@Id, @ParentCategoryId, @Name, @Description, @ImageUrl, @DisplayOrder)
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, category);

        return rowsAffected > 0;
    }
}