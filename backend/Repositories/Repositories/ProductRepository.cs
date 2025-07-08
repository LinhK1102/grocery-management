using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility.Common;

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
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // Ensure CategoryId and SupplierId are valid
            if (product.CategoryId == 0)
                product.CategoryId = _categoryDao.GetOrCreateUncategorizedCategoryId();

            if (product.SupplierId == 0)
                product.SupplierId = _supplierDao.GetOrCreateUnknownSupplierId(product.Supplier);

            // Remove navigation properties to avoid EF tracking issues
            product.Category = null;
            product.Supplier = null;

            // Detach items before saving product
            var detachedItems = product.Items?.ToList() ?? new List<Item>();
            product.Items = null;

            // Save product to DB to generate ProductId
            var savedProduct =  _dao.AddProduct(product);

            // Save items if any
            if (detachedItems.Any())
            {
                savedProduct.Items = detachedItems
                    .Select(item =>
                    {
                        item.ProductId = savedProduct.ProductId;
                        return _itemRepository.CreateItem(item);
                    })
                    .Where(created => created != null)
                    .ToList();
            }

            // Ensure product-warehouse relation is created only if not exists
            if (savedProduct.ProductWarehouses == null || !savedProduct.ProductWarehouses.Any())
            {
                var defaultWarehouse = _warehouseRepository.GetWarehouseByName(UtitlityConstant.Undefined);
                if (defaultWarehouse == null)
                {
                    defaultWarehouse = _warehouseRepository.CreateWarehouse(new Warehouse
                    {
                        WarehouseName = UtitlityConstant.Undefined,
                        WarehouseLocation = UtitlityConstant.Unknown
                    });
                }

                try
                {
                    var createdLink = _warehouseRepository.CreateProductWarehouse(savedProduct, defaultWarehouse);
                    savedProduct.ProductWarehouses = new List<ProductWarehouse> { createdLink };
                }
                catch (Exception ex)
                {
                    // Log or handle duplicate relation if needed
                    Console.WriteLine($"Warehouse link skipped: {ex.Message}");
                }
            }

            return savedProduct;
        }


        public Product UpdateProduct(ProductUpdateDto product) => _dao.UpdateProduct(product);
        public bool DeleteProduct(int id) => _dao.DeleteProduct(id);

        public Product GetProductByBarcode(string barcode) => _dao.GetProductByBarcode(barcode);

        public List<Product> GetLowStockProducts(int threshold) => _dao.GetLowStockProducts(threshold);

        public void AdjustStock(string barcode, string action, int quantity) => _dao.AdjustStock(barcode, action, quantity);

        public List<Product> GetSupplierProductList(int supplierId) => _dao.GetSupplierProductList(supplierId);

    }
}
