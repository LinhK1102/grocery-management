using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Repositories.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ItemDAO _dao;
        public ItemRepository(ItemDAO itemDAO)
        {
            _dao = itemDAO;
        }
        public List<Item> GetAllItems() => _dao.GetAllItems();
        public Item GetItemById(string id) => _dao.GetItemById(id);
        public Item GetItemByName(string id) => _dao.GetItemByBatchCode(id);
        public Item CreateItem(Item item)
        {
           return _dao.AddItem(item);
        }
        public Item UpdateItem(Item item)
        {
            return _dao.UpdateItem(item);
        }
        public bool DeleteItem(string id)
        {
            return _dao.DeleteItem(id);
        }
        public List<Item> SearchItems(string term)
        {
            return _dao.SearchItems(term);
        }
        public List<Item> GetItemsByProductId(int productId)
        {
            return _dao.GetItemsByProductId(productId);
        }

        public async Task EnsureDefaultItemAsync()
        {
            if (GetItemByName(UtitlityConstant.Undefined) == null)
            {
                CreateItem(new Item
                {
                    BatchCode = UtitlityConstant.Undefined,
                    Barcode = "0000000000000",
                    Quantity = 0
                });
            }
        }

    }
}
