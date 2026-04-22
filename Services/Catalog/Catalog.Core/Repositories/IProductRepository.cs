using Catalog.Core.Entities;
using Catalog.Core.Specifications;

namespace Catalog.Core.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProducts();
    
    Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams);
    
    Task<Product> GetProductById(string productId);
    
    Task<IEnumerable<Product>> GetProductByName(string productName);
    
    Task<IEnumerable<Product>> GetProductByBrand(string brandName);
    
    Task<Product> AddProduct(Product product);
    
    Task<bool> UpdateProduct(Product product);
    
    Task<bool> DeleteProduct(string productId);
    
    Task<ProductBrand> GetProductBrand(string brandId);
    
    Task<ProductType> GetProductType(string typeId) ;
}