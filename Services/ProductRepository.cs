using IShops.Data;
using IShops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Services
{
    public class ProductRepository : IProductRepository
    {
        private ApplicationDbContext _productContext;

        public ProductRepository(ApplicationDbContext productContext)
        {
            _productContext = productContext;
        }

        public bool CreateProduct(ProductsModel product)
        {
            _productContext.AddAsync(product);
            return Save();
        }

        public bool DeleteProduct(ProductsModel product)
        {
            _productContext.Remove(product);
            return Save();
        }

        public ProductsModel GetProduct(int ProductId)
        {
            return _productContext.Products.Where(c => c.Id == ProductId).FirstOrDefault();
        }

        public ICollection<ProductsModel> GetProducts()
        {
            return _productContext.Products.OrderBy(c => c.Name).ToList();
        }

        public ICollection<ProductsModel> GetProductsFromAUser(string UserId)
        {
            return _productContext.Products.Where(c => c.UserInfo.Id == UserId).ToList();

        }

        public bool IsDuplicateProductName(int ProductId, string ProductName)
        {
            var product = _productContext.Products.Where(c => c.Name.Trim().ToUpper() == ProductName.Trim().ToUpper()
            && c.Id != ProductId).FirstOrDefault();

            return product == null ? false : true;

        }

        public bool ProductExists(int ProductId)
        {
            return _productContext.Products.Any(c => c.Id == ProductId);
        }

        public bool Save()
        {
            var saved = _productContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateProduct(ProductsModel product)
        {
            _productContext.Update(product);
            return Save();
        }
    }
}
