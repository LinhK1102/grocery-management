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
    public class ItemRepository(ItemDAO itemDAO, Lazy<IProductRepository> productRepository) : IItemRepository
    {
        private readonly ItemDAO _dao = itemDAO;
        private readonly Lazy<IProductRepository> _productRepository = productRepository;

        public List<Item> GetAllItems() => _dao.GetAllItems();
        public Item GetItemById(string id) => _dao.GetItemById(id);
        public Item GetItemByName(string id) => _dao.GetItemByBatchCode(id);
        public Item CreateItem(Item item)
        {
            if (item.ImportedDate == null)
                item.ImportedDate = DateTime.UtcNow;

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

        public async Task<bool> EnsureSeedDataAsync()
        {
            if ( GetItemByName(UtitlityConstant.Undefined) == null)
            {
                var product = _productRepository.Value.GetProductByName(UtitlityConstant.Product_Default_Name);
                if ( product?.ProductId != 0)
                  return  CreateItem(new Item
                    {
                      ProductId = product.ProductId,
                        BatchCode = UtitlityConstant.Undefined,
                        Barcode = UtitlityConstant.Product_Default_Barcode,
                        Quantity = 0
                    }) != null;
            }
            return true;
        }

        public Item GetItemByBatchCode(string batchCode)
        {
            return _dao.GetItemByBatchCode(batchCode);
        }
    }
}
