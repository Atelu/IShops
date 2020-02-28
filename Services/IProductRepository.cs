using IShops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Services
{
    public interface IProductRepository
    {
        ICollection<ProductsModel> GetProducts();
        ProductsModel GetProduct(int ProductId);
    //    ICollection<ProductsModel> GetProductsFromAUser(string UserId);
        bool ProductExists(int ProductId);
        bool IsDuplicateProductName(int ProductId, string ProductName);

        bool CreateProduct(ProductsModel product);
        bool UpdateProduct(ProductsModel product);
        bool DeleteProduct(ProductsModel product);
        bool Save();
    }
}
