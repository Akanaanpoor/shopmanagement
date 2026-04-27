using System.Text.Json;
using Catalog.Core.Entities;
using Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data;

public class DataBaseSeeder
{
    public static async Task SeedAsync(IOptions<DatabaseConfiguration> configuration)
    {
        var configurations = configuration.Value;
        
        var client = new MongoClient(configurations.ConnectionString);
        var database = client.GetDatabase(configurations.DatabaseName);
        
        var products = database.GetCollection<Product>(configurations.ProductCollectionName);
        var types = database.GetCollection<ProductType>(configurations.TypeCollectionName);
        var brands = database.GetCollection<ProductBrand>(configurations.BrandCollectionName);
        
        var seedBasePath = Path.Combine("Data", "SeedData");
        
        //seed Brand
        List<ProductBrand> brandList = new();
        
        if ((await brands.CountDocumentsAsync(_ => true) == 0))
        {
            var brandData = await File.ReadAllTextAsync(Path.Combine(seedBasePath, "BrandData.json"));
            
            brandList = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
            
            await brands.InsertManyAsync(brandList);
        }
        else
        {
            brandList = await brands.Find(_ => true).ToListAsync();
        }
        
        //seed Type
        List<ProductType> typeList = new();
        
        if ((await types.CountDocumentsAsync(_ => true) == 0))
        {
            var typeData = await File.ReadAllTextAsync(Path.Combine(seedBasePath, "TypeData.json"));
            
            typeList = JsonSerializer.Deserialize<List<ProductType>>(typeData);
            
            await types.InsertManyAsync(typeList);
        }
        else
        {
            typeList = await types.Find(_ => true).ToListAsync();
        }
        //seed Product
        List<Product> productList = new();
        
        if ((await products.CountDocumentsAsync(_ => true) == 0))
        {
            var productData = await File.ReadAllTextAsync(Path.Combine(seedBasePath, "ProductData.json"));
            
            productList = JsonSerializer.Deserialize<List<Product>>(productData);

            foreach (var product in productList)
            {
                product.Id = null;
                if (product.CreatedDate == default)
                    product.CreatedDate = DateTime.UtcNow;
            }
            
            await products.InsertManyAsync(productList);
        }
    }
}