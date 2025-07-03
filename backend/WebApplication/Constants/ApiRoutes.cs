namespace WebApplication.Constants
{
    public static class ApiRoutes
    {
        public static class Product
        {
            public const string GetAll = "/api/products/get-all";
            public const string GetById = "/api/products/get-by-id/{0}";
            public const string Create = "/api/products/create";
            public const string Update = "/api/products/update/{0}";
            public const string Delete = "/api/products/delete/{0}";
            public const string LowStock = "/api/products/low-stock";
            public const string Scan = "/api/products/scan/{0}";
            public const string ScanAdjustStock = "/api/products/scan-adjust-stock";
            public const string GetBySupplierId = "/api/products/supplier-products/{0}";
        }

        public static class Employee
        {
            public const string GetAll = "/api/employees";
            public const string GetById = "/api/employees/{0}";
            public const string Create = "/api/employees";
            public const string Update = "/api/employees/{0}";
            public const string Delete = "/api/employees/{0}";
            public const string TopSellers = "/api/employees/top-sellers";
        }

        public static class Auth
        {
            public const string Login = "/api/Auth/login";
            public const string Register = "/api/Auth/register";
        }

        public static class Barcode
        {
            public const string Search = "/api/barcode/search/{0}";
            public const string Scan = "/api/barcode/scan/{0}";
        }

        public static class Customers
        {
            public const string Search = "/api/customer/search?searchTerm={0}";
            public const string HighDiscount = "/api/customer/high-discount?minDiscount={0}";
            public const string UpdateDiscount = "/api/customer/update-discount?customerId={0}&newRate={1}&maxLimit={2}";
        }

        public static class Orders
        {
            public const string GetAll = "/api/orders/get-all";
            public const string GetById = "/api/orders/get-by-id/{0}";
            public const string Create = "/api/orders/create";
            public const string Update = "/api/orders/update/{0}";
            public const string Delete = "/api/orders/delete/{0}";
            public const string Search = "/api/orders/search?keyword={0}";
        }

        public static class OrderDetails
        {
            public const string GetByOrderId = "/api/orderdetail/get-all-by-order-id/{0}";
            public const string Create = "/api/orderdetail/create";
            public const string Update = "/api/orderdetail/update/{0}";
            public const string Delete = "/api/orderdetail/delete/{0}";
        }

        public static class RetailOutlets
        {
            public const string GetAll = "/api/retail-outlets/get-all";
            public const string Create = "/api/retail-outlets/create";
            public const string GetById = "/api/retail-outlets/get-by-id/{0}";
            public const string Update = "/api/retail-outlets/update/{0}";
            public const string Delete = "/api/retail-outlets/delete/{0}";
        }

        public static class Suppliers
        {
            public const string GetAll = "/api/suppliers/get-all";
            public const string GetById = "/api/suppliers/get-by-id/{0}";
            public const string Create = "/api/suppliers/create";
            public const string Update = "/api/suppliers/update/{0}";
            public const string Delete = "/api/suppliers/delete/{0}";
        }

        public static class Warehouse
        {
            public const string GetAll = "/api/warehouses/get-all";
            public const string GetById = "/api/warehouses/get-by-id/{0}";
            public const string Create = "/api/warehouses/create";
            public const string Update = "/api/warehouses/update/{0}";
            public const string Delete = "/api/warehouses/delete/{0}";
        }
    }
}
