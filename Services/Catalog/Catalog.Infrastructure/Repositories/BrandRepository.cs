using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly IMongoCollection<ProductBrand> _brands;

    public BrandRepository(IOptions<DatabaseConfiguration> configuration)
    {
        var configurations = configuration.Value;
        
        var client = new MongoClient(configurations.ConnectionString);
        var database = client.GetDatabase(configurations.DatabaseName);
        _brands = database.GetCollection<ProductBrand>(configurations.BrandCollectionName);
    }

    public async Task<IEnumerable<ProductBrand>> GetAllBrandsAsync()
    {
        return await _brands.Find(_ => true).ToListAsync();
    }

    public async Task<ProductBrand> GetBrandByIdAsync(string id)
    {
        return await _brands.Find(b => b.Id == id).FirstOrDefaultAsync();
    }
}