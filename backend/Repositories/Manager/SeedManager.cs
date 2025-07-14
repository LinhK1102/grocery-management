using Repositories.Interfaces;
using System.Threading.Tasks;

namespace Repositories.Manager
{
    public class SeedManager
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ISupplierRepository _supplierRepo;
        private readonly IWarehouseRepository _warehouseRepo;
        private readonly IRetailOutletRepository _retailOutletRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IProductRepository _productRepo;
        private readonly IItemRepository _itemRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;

        public SeedManager(
            ICategoryRepository categoryRepo,
            ISupplierRepository supplierRepo,
            IWarehouseRepository warehouseRepo,
            IRetailOutletRepository retailOutletRepo,
            ICustomerRepository customerRepo,
            IEmployeeRepository employeeRepo,
            IProductRepository productRepo,
            IItemRepository itemRepo,
            IInvoiceRepository invoiceRepo,
            IOrderRepository orderRepo,
            IOrderDetailRepository orderDetailRepo
        )
        {
            _categoryRepo = categoryRepo;
            _supplierRepo = supplierRepo;
            _warehouseRepo = warehouseRepo;
            _retailOutletRepo = retailOutletRepo;
            _customerRepo = customerRepo;
            _employeeRepo = employeeRepo;
            _productRepo = productRepo;
            _itemRepo = itemRepo;
            _invoiceRepo = invoiceRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        public async Task SeedAllAsync()
        {
            // Theo thứ tự phụ thuộc khóa ngoại
            await _categoryRepo.EnsureSeedDataAsync();
            await _supplierRepo.EnsureSeedDataAsync();
            await _warehouseRepo.EnsureSeedDataAsync();
            await _retailOutletRepo.EnsureSeedDataAsync();
            await _customerRepo.EnsureSeedDataAsync();
            await _employeeRepo.EnsureSeedDataAsync();

            await _productRepo.EnsureSeedDataAsync();      // Cần Category + Supplier
            await _itemRepo.EnsureSeedDataAsync();         // Cần Product
            //await _invoiceRepo.EnsureSeedDataAsync();      // Cần Customer
            await _orderRepo.EnsureSeedDataAsync();        // Cần Customer + Employee + RetailOutlet + Warehouse
            await _orderDetailRepo.EnsureSeedDataAsync();  // Cần Order + Product
        }
    }
}
