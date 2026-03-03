using System.Collections.Generic;
using System.Threading.Tasks;
using MuskanMobile.Domain.Entities;

namespace MuskanMobile.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product product);
}

