using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.DAO
{
    public class ItemDAO
    {
        private readonly ApplicationDbContext _context;

        public ItemDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Item> GetAllItems()
        {
            return _context.Items.Include(i => i.Product).ToList();
        }

        public Item? GetItemById(string itemId)
        {
            return _context.Items
                .Include(i => i.Product)
                .FirstOrDefault(i => i.ItemId == itemId);
        }

        public List<Item> GetItemsByProductId(int productId)
        {
            return _context.Items
                .Where(i => i.ProductId == productId)
                .Include(i => i.Product)
                .ToList();
        }

        public List<Item> SearchItems(string term)
        {
            return _context.Items
                .Where(i => i.Product.ProductName.Contains(term) || i.BatchCode.Contains(term))
                .Include(i => i.Product)
                .ToList();
        }

        public Item? AddItem(Item item)
        {
            try
            {
                // Gán giá trị nếu thiếu (không tạo dòng mới)
                if (string.IsNullOrWhiteSpace(item.ItemId))
                {
                    item.ItemId = Guid.NewGuid().ToString("N");
                }

                if (string.IsNullOrWhiteSpace(item.BatchCode))
                {
                    item.BatchCode = "BATCH-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
                }

                _context.Items.Add(item);
                _context.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding item: {ex.Message}");
                return null;
            }
        }


        public Item? UpdateItem(Item item)
        {
            try
            {
                _context.Items.Update(item);
                _context.SaveChanges();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item: {ex.Message}");
                return null;
            }
        }

        public bool DeleteItem(string itemId)
        {
            try
            {
                var item = _context.Items.Find(itemId);
                if (item != null)
                {
                    _context.Items.Remove(item);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting item: {ex.Message}");
            }
            return false;
        }
    }
}

