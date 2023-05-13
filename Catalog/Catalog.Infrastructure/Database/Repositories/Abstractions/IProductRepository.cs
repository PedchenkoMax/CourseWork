﻿using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface IProductRepository
{
    Task<List<ProductEntity>> GetAllAsync();
    Task<ProductEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(ProductEntity product);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(ProductEntity product);
}