using BusinessObjects.Entities;

namespace Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> EnsureDefaultCategoriesAsync();
        List<Category> GetAll();
        Category? GetById(int id);
        Category GetOrCreateByName(string name);
        Category? Update(Category category);

    }
}
