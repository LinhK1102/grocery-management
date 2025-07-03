using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IItemRepository
    {
        List<Item> GetAllItems();
        Item GetItemById(string id);
        Item CreateItem(Item item);
        Item UpdateItem(Item item);
        bool DeleteItem(string id);
        List<Item> SearchItems(string term);
        List<Item> GetItemsByProductId(int productId);
    }
}
