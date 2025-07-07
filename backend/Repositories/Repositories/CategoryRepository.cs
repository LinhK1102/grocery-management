using BusinessObjects.Entities;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }
        public Category GetOrCreateByName(string name)
        {
            var existing = _categoryDAO.GetCategoryByName(name.ToUpper());
            if (existing != null) return null;

            var newCategory = _categoryDAO.AddCategory(new Category { CategoryName = name.ToUpper() });

            return newCategory;
        }
        public List<Category> GetAll()
        {
            var categories = _categoryDAO.GetAllCategory();
            return categories;
        }

        public Category? GetById(int id)
        {
            var category = _categoryDAO.GetCategoryById(id);
            return category == null ? null : category;
        }

        public Category? Update(Category categoryDto)
        {
            var updated = _categoryDAO.UpdateCategory(categoryDto);
            return updated == null ? null : updated;
        }

        private static readonly List<string> _defaultCategories = new()
            {
                "UNCATEGORIZED",
                "BEVERAGES",
                "SNACKS",
                "DAIRY PRODUCTS",
                "FRESH PRODUCE",
                "BAKERY",
                "MEAT & POULTRY",
                "FROZEN FOODS",
                "CANNED GOODS",
                "GRAINS & CEREALS",
                "SPICES & SEASONINGS",
                "CONDIMENTS & SAUCES",
                "CLEANING SUPPLIES",
                "PERSONAL CARE",
                "HOUSEHOLD ITEMS",
                "BABY PRODUCTS",
                "PET SUPPLIES",
                "HEALTH & WELLNESS",
                "INSTANT NOODLES",
                "ALCOHOLIC BEVERAGES"
            };

        public async Task<bool> EnsureDefaultCategoriesAsync()
        {
            bool check = false;
            foreach (var name in _defaultCategories)
            {
                var exists =  _categoryDAO.GetCategoryByName(name.ToUpper());
                if (exists == null)
                {
                    _categoryDAO.AddCategory(new Category { CategoryName = name});
                    check = true;
                }
            }
            return check;
        }
    }
}
