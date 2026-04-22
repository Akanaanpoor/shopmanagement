using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class TypeRepository : ITypeRepository
{
    private readonly IMongoCollection<ProductType> _types;

    public TypeRepository(IOptions<DatabaseConfiguration> configuration)
    {
        var configurations = configuration.Value;
        
        var client = new MongoClient(configurations.ConnectionString);
        var database = client.GetDatabase(configurations.DatabaseName);
        _types = database.GetCollection<ProductType>(configurations.TypeCollectionName);
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