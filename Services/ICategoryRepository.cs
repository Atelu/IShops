using IShops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int CategoryId);
        ICollection<Category> GetAllCategoriesForAProduct(int ProductId);
        ICollection<ProductsModel> GetAllProductsForCategory(int CategoryId);
        bool CategoryExists(int CategoryId);
        bool IsDuplicateCategoryName(int CategoryId, string CategoryName);

        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
