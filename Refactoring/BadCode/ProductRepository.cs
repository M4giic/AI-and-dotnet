namespace BadCode;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return _products.ToList();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be positive", nameof(id));
    
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public async Task<bool> AddAsync(Product product)
    {
        if (_products.Any(p => p.Id == product.Id))
            throw new ArgumentException("Product with this ID already exists");

        _products.Add(product);
        return true;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing == null)
            return false;

        var index = _products.IndexOf(existing);
        _products[index] = product;
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return _products.RemoveAll(p => p.Id == id) > 0;
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Search term cannot be empty");

        return _products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<decimal> GetTotalInventoryValueAsync()
    {
        return _products.Sum(p => p.Price * p.Stock);
    }
}