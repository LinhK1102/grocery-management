using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _dao;
        private readonly CategoryDAO _categoryDao;
        private readonly SupplierDAO _supplierDao;
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        public ProductRepository(ApplicationDbContext context, CategoryDAO categoryDao, SupplierDAO supplierDao, IItemRepository itemRepository, IWarehouseRepository warehouseRepository)
        {
            _dao = new ProductDAO(context);
            _categoryDao = categoryDao;
            _supplierDao = supplierDao;
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
        }

        public List<Product> GetAllProduct() => _dao.GetAllProduct();
        public Product GetProductById(int id) => _dao.GetProductById(id);
        public async Task<Product> AddProduct(Product product)
        {
            if (product.CategoryId == 0)
                product.CategoryId = _categoryDao.GetOrCreateUncategorizedCategoryId();

            if (product.SupplierId == 0)
                product.SupplierId = _supplierDao.GetOrCreateUnknownSupplierId(product.Supplier);

            if (product.Items == null && product.Items.Count <= 0)
                product.Items = product.Items
                                .Select(item =>
                                {
                                    item.ProductId = product.ProductId;
                                    return _itemRepository.CreateItem(item);
                                })
                                .Where(added => added != null)
                                .ToList();

            if (!(product.ProductWarehouses?.Any() ?? false))
            {
                var created = _warehouseRepository.CreateProductWarehouse(product, new Warehouse { WarehouseId = 0 });
                product.ProductWarehouses = created != null
                    ? new List<ProductWarehouse> { created }
                    : [];
            }


            var status = _dao.AddProduct(product);
            return product;
        }
        public Product UpdateProduct(Product product) => _dao.UpdateProduct(product);
        public bool DeleteProduct(int id) => _dao.DeleteProduct(id);

        public Product GetProductByBarcode(string barcode) => _dao.GetProductByBarcode(barcode);

        public List<Product> GetLowStockProducts(int threshold) => _dao.GetLowStockProducts(threshold);

        public void AdjustStock(string barcode, string action, int quantity) => _dao.AdjustStock(barcode, action, quantity);

        public List<Product> GetSupplierProductList(int supplierId) => _dao.GetSupplierProductList(supplierId);

        Product IProductRepository.AddProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
