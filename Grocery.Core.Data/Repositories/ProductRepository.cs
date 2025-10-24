using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> products;

        public ProductRepository()
        {
            products = new List<Product>
            {
                new Product(1, "Melk", 300, new DateOnly(2025, 9, 25), 0.95m),
                new Product(2, "Kaas", 100, new DateOnly(2025, 9, 30), 7.98m),
                new Product(3, "Brood", 400, new DateOnly(2025, 9, 12), 2.19m),
                new Product(4, "Cornflakes", 0, new DateOnly(2025, 12, 31), 1.48m)
            };
        }

        public List<Product> GetAll() => products;

        public Product? Get(int id) => products.FirstOrDefault(p => p.Id == id);

        public Product Add(Product item)
        {
            int nextId = products.Any() ? products.Max(p => p.Id) + 1 : 1;
            item.Id = nextId;
            products.Add(item);
            return item;
        }

        public Product? Delete(Product item)
        {
            var product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product == null) return null;

            products.Remove(product);
            return product;
        }

        public Product? Update(Product item)
        {
            var product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product == null) return null;

            product.Name = item.Name;
            product.Stock = item.Stock;          // aangepast
            product.ShelfLife = item.ShelfLife;  // aangepast
            product.Price = item.Price;

            return product;
        }
    }
}
