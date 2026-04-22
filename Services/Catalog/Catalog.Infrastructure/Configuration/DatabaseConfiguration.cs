namespace Catalog.Infrastructure.Configuration;

public class DatabaseConfiguration
{
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string ProductCollectionName { get; set; }

    public string BrandCollectionName { get; set; }

    public string TypeCollectionName { get; set; }
}