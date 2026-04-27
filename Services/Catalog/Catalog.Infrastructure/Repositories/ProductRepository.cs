using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specifications;
using Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<ProductType> _types;
    private readonly IMongoCollection<ProductBrand> _brands;

    public ProductRepository(IOptions<DatabaseConfiguration> configuration)
    {
        var configurations = configuration.Value;
        
        var client = new MongoClient(configurations.ConnectionString);
        var database = client.GetDatabase(configurations.DatabaseName);
        _products = database.GetCollection<Product>(configurations.ProductCollectionName);
        _types = database.GetCollection<ProductType>(configurations.TypeCollectionName);
        _brands = database.GetCollection<ProductBrand>(configurations.BrandCollectionName);
    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpec, CancellationToken cancellationToken)
    {
        var builder = Builders<Product>.Filter;

        var filter = builder.Empty;

        if (!string.IsNullOrEmpty(catalogSpec.Search))
        {
            filter &= builder.Where(x => x.Name.ToLower().Contains(catalogSpec.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(catalogSpec.BrandId))
        {
            filter &= builder.Eq(x => x.Brand.Id, catalogSpec.BrandId);
        }

        if (!string.IsNullOrEmpty(catalogSpec.TypeId))
        {
            filter &= builder.Eq(x => x.Type.Id, catalogSpec.TypeId);
        }

        var count = await _products.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var data = await ApplyDataFilterAsync(catalogSpec, filter,cancellationToken);

        return new Pagination<Product>(catalogSpec.PageIndex, catalogSpec.PageSize, (int)count, data);
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

    private async Task<IReadOnlyCollection<Product>> ApplyDataFilterAsync(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter, CancellationToken cancellationToken)
    {
        var sortDefinition = Builders<Product>.Sort.Ascending(x => x.Name);

        if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
        {
            sortDefinition = catalogSpecParams.Sort switch
            {
                "NameAsc" => Builders<Product>.Sort.Ascending(x => x.Name),
                "NameDesc" => Builders<Product>.Sort.Descending(x => x.Name),
                "DateAsc" => Builders<Product>.Sort.Ascending(x => x.CreatedDate),
                "DateDesc" => Builders<Product>.Sort.Descending(x => x.CreatedDate),
                "priceAsc" => Builders<Product>.Sort.Ascending(x => x.Price),
                "priceDesc" => Builders<Product>.Sort.Descending(x => x.Price),
                _ => Builders<Product>.Sort.Ascending(a => a.Name)
            };
        }

        return await _products
            .Find(filter)
            .Sort(sortDefinition)
            .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
            .Limit(catalogSpecParams.PageSize)
            .ToListAsync(cancellationToken);
    }
}