namespace Catalog.Core.Specifications;

public class Pagination<T> where T : class
{
    public Pagination()
    {
    }
    
    public Pagination(int index, int size, int count, IReadOnlyCollection<T> data)
    {
        Index = index;
        Size = size;
        Count = count;
        Data = data;
    }

    public IReadOnlyCollection<T> Data { get; set; }

    public int Count { get; set; }

    public int Size { get; set; }

    public int Index { get; set; }
}