using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product Add(Product item)
        {
            // Eenvoudige validatie (optioneel, maar verstandig)
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Productnaam mag niet leeg zijn.");

            if (item.Stock < 0)
                throw new ArgumentException("Voorraad mag niet negatief zijn.");

            if (item.Price < 0)
                throw new ArgumentException("Prijs mag niet negatief zijn.");

            // Product toevoegen aan database via repository
            return _productRepository.Add(item);
        }

        public Product? Delete(Product item)
        {
            throw new NotImplementedException();
        }

        public Product? Get(int id)
        {
            throw new NotImplementedException();
        }

        public Product? Update(Product item)
        {
            return _productRepository.Update(item);
        }
    }
}
