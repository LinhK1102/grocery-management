using WebApplication.Models.Dto;

namespace WebApplication.Helpers
{
    public static class DtoExtensions
    {
        public static ProductUpdateDto ProductToUpdateDto(this ProductDto dto)
        {
            return new ProductUpdateDto
            {
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                UnitsInStock = dto.UnitsInStock,
                UnitPrice = dto.UnitPrice,
                BarcodeValue = dto.BarcodeValue,
                ExpiryDuration = dto.ExpiryDuration
            };
        }

        public static ProductDto ProductToFullDto(this ProductUpdateDto updateDto)
        {
            return new ProductDto
            {
                ProductId = updateDto.ProductId,
                ProductName = updateDto.ProductName,
                CategoryId = updateDto.CategoryId,
                SupplierId = updateDto.SupplierId,
                UnitsInStock = updateDto.UnitsInStock,
                UnitPrice = updateDto.UnitPrice,
                BarcodeValue = updateDto.BarcodeValue,
                ExpiryDuration = updateDto.ExpiryDuration,
                // Khởi tạo rỗng để tránh null reference nếu dùng
                Items = new List<ItemDto>(),
                OrderDetails = new List<OrderDetailDto>(),
                ProductWarehouses = new List<ProductWarehouseDto>()
            };
        }

        public static readonly Dictionary<string, string> Icons = new()
        {
            ["BEVERAGES"] = "fa-mug-hot",
            ["SNACKS"] = "fa-cookie-bite",
            ["DAIRY PRODUCTS"] = "fa-cheese",
            ["FRESH PRODUCE"] = "fa-apple-whole",
            ["BAKERY"] = "fa-bread-slice",
            ["MEAT & POULTRY"] = "fa-drumstick-bite",
            ["FROZEN FOODS"] = "fa-snowflake",
            ["CANNED GOODS"] = "fa-can-food",
            ["GRAINS & CEREALS"] = "fa-seedling",
            ["SPICES & SEASONINGS"] = "fa-pepper-hot",
            ["CONDIMENTS & SAUCES"] = "fa-bottle-droplet",
            ["CLEANING SUPPLIES"] = "fa-pump-soap",
            ["PERSONAL CARE"] = "fa-soap",
            ["HOUSEHOLD ITEMS"] = "fa-broom",
            ["BABY PRODUCTS"] = "fa-baby",
            ["PET SUPPLIES"] = "fa-dog",
            ["HEALTH & WELLNESS"] = "fa-heart-pulse",
            ["INSTANT NOODLES"] = "fa-bowl-rice",
            ["ALCOHOLIC BEVERAGES"] = "fa-wine-glass-alt",
            ["UNCATEGORIZED"] = "fa-box"
        };

        public static string GetIconClass(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) return "fa-box";

            return Icons.TryGetValue(categoryName.ToUpper(), out var icon)
                ? icon
                : "fa-box";
        }

        public static OrderDto ConvertToOrderDto(OrderOrInvoiceDto input, int customerId, int employeeId, int? outletId = null, int? warehouseId = null)
        {
            return new OrderDto
            {
                OrderDate = input.OrderDate,
                CustomerId = customerId,
                EmployeeId = employeeId,
                OutletId = outletId,
                WarehouseId = warehouseId,
                Items = input.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()

            };
        }

    }

}