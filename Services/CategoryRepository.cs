using IShops.Data;
using IShops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private ApplicationDbContext _categoryContext;

        public CategoryRepository(ApplicationDbContext categoryContext)
        {
            _categoryContext = categoryContext;
        }
        public bool CategoryExists(int CategoryId)
        {
            return _categoryContext.Categories.Any(c => c.Id == CategoryId);
        }

        public bool CreateCategory(Category category)
        {
            _categoryContext.AddAsync(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _categoryContext.Remove(category);
            return Save();
        }

        public ICollection<ProductsModel> GetAllProductsForCategory(int CategoryId)
        {
            return _categoryContext.ProductCategory.Where(p => p.CategoryId == CategoryId).Select(b => b.Product ).ToList();

        }

        public ICollection<Category> GetCategories()
        {
            return _categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Category> GetAllCategoriesForAProduct(int ProductId)
        {
            return _categoryContext.ProductCategory.Where(p => p.ProductId == ProductId).Select(c => c.Category).ToList();
        }

        public Category GetCategory(int CategoryId)
        {
            return _categoryContext.Categories.Where(c => c.Id == CategoryId).FirstOrDefault();
        }

        public bool IsDuplicateCategoryName(int CategoryId, string CategoryName)
        {
            var cat = _categoryContext.Categories.Where(c => c.Name.Trim().ToUpper() == CategoryName.Trim().ToUpper()
             && c.Id != CategoryId).FirstOrDefault();

            return cat == null ? false : true;
        }

        public bool Save()
        {
            var saved = _categoryContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _categoryContext.Update(category);
            return Save();
        }
    }
}
