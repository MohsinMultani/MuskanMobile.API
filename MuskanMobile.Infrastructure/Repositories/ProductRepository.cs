
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MuskanMobile.Domain.Entities;
using MuskanMobile.Infrastructure.Data;

namespace MuskanMobile.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MuskanMobileDbContext _dbContext;

    public ProductRepository(MuskanMobileDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
    }
}

