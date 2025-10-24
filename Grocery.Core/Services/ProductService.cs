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

        public List<Product> GetAll() => _productRepository.GetAll();

        public Product Add(Product item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Product naam is verplicht.");
            if (item.Stock < 0)
                throw new ArgumentException("Product voorraad kan niet negatief zijn.");
            if (item.Price < 0)
                throw new ArgumentException("Product prijs kan niet negatief zijn.");

            return _productRepository.Add(item);
        }
        

        //public Product Add(Product item)
        //{
        //    throw new NotImplementedException();
        //}

        public Product? Delete(Product item)
        {
            if (item.Id <= 0) return null; 
            return _productRepository.Delete(item);
        }

        public Product? Get(int id)
        {
            if (id <= 0) return null;
            return _productRepository.Get(id);
        }

        public Product? Update(Product item) => _productRepository.Update(item);

        //public Product? Update(Product item)
        //{
        //    return _productRepository.Update(item);
        //}
    }
}
