using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly IMongoCollection<ProductBrand> _brands;

    public BrandRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
        var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        _brands = database.GetCollection<ProductBrand>(configuration["DatabaseSettings:BrandCollectionName"]);
    }

    public async Task<IEnumerable<ProductBrand>> GetProductBrandsAsync()
    {
        return await _brands.Find(_ => true).ToListAsync();
    }

    public async Task<ProductBrand> GetProductBrandByIdAsync(string id)
    {
        return await _brands.Find(b => b.Id == id).FirstOrDefaultAsync();
    }
}