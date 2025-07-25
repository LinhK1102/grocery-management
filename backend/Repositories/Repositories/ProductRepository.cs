using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility.Common;

namespace Repositories.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _productDao;
        private readonly CategoryDAO _categoryDao;
        private readonly SupplierDAO _supplierDao;
        private readonly IItemRepository _itemRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        public ProductRepository( ProductDAO productDAO, CategoryDAO categoryDao, SupplierDAO supplierDao, IItemRepository itemRepository, IWarehouseRepository warehouseRepository)
        {
            _productDao = productDAO;
            _categoryDao = categoryDao;
            _supplierDao = supplierDao;
            _itemRepository = itemRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<List<Product>> GetAllProduct()
        {
            return await _productDao.GetAllProductAsync();
        }

        public Product GetProductById(int id) => _productDao.GetProductById(id);
        public Product GetProductByName(string productName) => _productDao.GetProductByName(productName);
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
            var savedProduct = await _productDao.AddProductAsync(product);

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


        public Product UpdateProduct(ProductUpdateDto product) => _productDao.UpdateProduct(product);
        public bool DeleteProduct(int id) => _productDao.DeleteProduct(id);

        public Product GetProductByBarcode(string barcode) => _productDao.GetProductByBarcode(barcode);

        public List<Product> GetLowStockProducts(int threshold) => _productDao.GetLowStockProducts(threshold);

        public bool AdjustStock(string barcode, int action, int quantity) => _productDao.AdjustStock(barcode, action, quantity);
        public bool AdjustStock(int productId, int action, int quantity) => _productDao.AdjustStock(productId, action, quantity);

        public List<Product> GetSupplierProductList(int supplierId) => _productDao.GetSupplierProductList(supplierId);
        public List<Product> GetCategoryProductList(int supplierId) => _productDao.GetCategoryProductList(supplierId);

        public async Task<bool> EnsureSeedDataAsync()
        {
            var hasData = await _productDao.GetAllProductAsync();
            if (!hasData.Any())
            {
                var product = new Product
                {
                    ProductName = UtitlityConstant.Product_Default_Name,
                    CategoryId = _categoryDao.GetCategoryByName(UtitlityConstant.Category_Default_Name)?.CategoryId ?? 0,
                    SupplierId = _supplierDao.GetSupplierByName(UtitlityConstant.Supplier_Default_Name)?.SupplierId ?? 0,
                    BarcodeValue = UtitlityConstant.Product_Default_Barcode,
                    UnitPrice = 10000
                };

                var added = await AddProduct(product);
                return added != null;
            }

            return true;
        }

        public IQueryable<Product> GetQueryable()
        {
            return _productDao.GetQueryable(); // giữ nguyên dạng IQueryable để OData xử lý được
        }

    }
}
