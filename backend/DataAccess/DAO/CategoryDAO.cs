using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace DataAccess.DAO
{
    public class CategoryDAO
    {
        private readonly ApplicationDbContext _context;

        public CategoryDAO(ApplicationDbContext context) => _context = context;

        public List<Category> GetAllCategory() => _context.Categories.ToList();
        public Category GetCategoryById(int id) => _context.Categories.Find(id);
        public Category AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }
        public Category UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return category;
        }
        public bool DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool DeleteCategoryWithDependencyCheck(int categoryId)
        {
            var hasProducts = _context.Products.Any(p => p.CategoryId == categoryId);
            if (hasProducts)
                return false;
            var category = _context.Categories.Find(categoryId);
            if (category == null)
                return false;
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return true;
        }
        public List<Category> SearchCategories(string term)
        {
            return _context.Categories
                .Where(c => c.CategoryName.Contains(term))
                .ToList();
        }
        public List<Category> GetCategoriesByProductId(int productId)
        {
            return _context.Categories
                .Where(c => c.Products.Any(p => p.ProductId == productId))
                .ToList();
        }
        public List<Category> GetCategoriesBySupplierId(int supplierId)
        {
            return _context.Categories
                .Where(c => c.Products.Any(p => p.SupplierId == supplierId))
                .ToList();
        }


        public Category GetCategoryByName(string categoryName)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
        }

        private int? _uncategorizedId;
        public int GetOrCreateUncategorizedCategoryId()
        {
            if (_uncategorizedId.HasValue)
                return _uncategorizedId.Value;

            var existing = GetCategoryByName(UtitlityConstant.Category_Default_Name);
            if (existing != null)
                return (_uncategorizedId = existing.CategoryId).Value;

            var created = AddCategory(new Category { CategoryName = UtitlityConstant.Category_Default_Name });
            if (created == null)
                throw new Exception($"Failed to create '{UtitlityConstant.Category_Default_Name}' category.");

            return (_uncategorizedId = created.CategoryId).Value;
        }


        public bool IsUncategorizedCategoryExists()
        {
            return GetCategoryByName("Uncategorized") != null;
        }

    }
}
