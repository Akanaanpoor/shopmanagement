using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class TypeRepository : ITypeRepository
{
    private readonly IMongoCollection<ProductType> _types;

    public TypeRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
        var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        _types = database.GetCollection<ProductType>(configuration["DatabaseSettings:TypeCollectionName"]);
    }

    public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
    {
        return await _types.Find(_ => true).ToListAsync();
    }

    public async Task<ProductType?> GetProductTypeByIdAsync(string id)
    {
        return await _types.Find(b => b.Id == id).FirstOrDefaultAsync();
    }
}