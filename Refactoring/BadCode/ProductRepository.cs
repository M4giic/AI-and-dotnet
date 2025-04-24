namespace BadCode;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
}

public class ProductRepository
{
    private List<Product> _products = new List<Product>();
    
    public ProductRepository()
    {
        _products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10, IsAvailable = true });
        _products.Add(new Product { Id = 2, Name = "Phone", Price = 499.99m, Stock = 20, IsAvailable = true });
        _products.Add(new Product { Id = 3, Name = "Headphones", Price = 99.99m, Stock = 0, IsAvailable = false });
    }
    
    public List<Product> GetAllProducts()
    {
        return _products;
    }
    
    public void AddProduct(Product product)
    {
        _products.Add(product);
    }
    
    public bool UpdateProduct(Product product)
    {
        for (int i = 0; i < _products.Count; i++)
        {
            if (_products[i].Id == product.Id)
            {
                _products[i] = product;
                
                if (product.Stock > 0)
                {
                    _products[i].IsAvailable = true;
                }
                else
                {
                    _products[i].IsAvailable = false;
                }
                
                return true;
            }
        }
        
        return false;
    }
    
    public void DeleteProduct(int id)
    {
        _products.RemoveAll(p => p.Id == id);
    }
    
    public List<Product> SearchProducts(string name)
    {
        return _products.Where(p => p.Name.Contains(name)).ToList();
    }
    
    public List<Product> GetAvailableProducts()
    {
        return _products.Where(p => p.IsAvailable && p.Stock > 0).ToList();
    }
    
    public decimal GetTotalInventoryValue()
    {
        decimal total = 0;
        foreach (var product in _products)
        {
            total += product.Price * product.Stock;
        }
        
        Console.WriteLine($"Total inventory value: {total}");
        
        return total;
    }
}