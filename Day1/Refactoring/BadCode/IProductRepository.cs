namespace BadCode;

public interface IProductRepository
{
    List<Product> GetAllProducts();
    void AddProduct(Product product);
    bool UpdateProduct(Product product);
    void DeleteProduct(int id);
    List<Product> SearchProducts(string name);
    List<Product> GetAvailableProducts();
    decimal GetTotalInventoryValue();
}
