using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specifications;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<ProductType> _types;
    private readonly IMongoCollection<ProductBrand> _brands;

    public ProductRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
        var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        _products = database.GetCollection<Product>(configuration["DatabaseSettings:ProductCollectionName"]);
        _types = database.GetCollection<ProductType>(configuration["DatabaseSettings:TypeCollectionName"]);
        _brands = database.GetCollection<ProductBrand>(configuration["DatabaseSettings:BrandCollectionName"]);
    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task<Product> GetProductById(string productId)
    {
        return await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByName(string productName)
    {
        var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression($".*{productName}.*", "i"));

        return await _products.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByBrand(string brandName)
    {
        return await _products
            .Find(p => p.Brand.Name.ToLower() == brandName.ToLower())
            .ToListAsync();
    }

    public async Task<Product> AddProduct(Product product)
    {
        await _products.InsertOneAsync(product);

        return product;
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var updatedItem = await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
        
        return updatedItem.IsAcknowledged && updatedItem.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProduct(string productId)
    {
        var deletedItem = await _products.DeleteOneAsync(p => p.Id == productId);

        return deletedItem.IsAcknowledged && deletedItem.DeletedCount > 0;
    }

    public async Task<ProductBrand> GetProductBrand(string brandId)
    {
        return await _brands.Find(b => b.Id == brandId).FirstOrDefaultAsync();
    }

    public async Task<ProductType> GetProductType(string typeId)
    {
        return await _types.Find(b => b.Id == typeId).FirstOrDefaultAsync();
    }
}