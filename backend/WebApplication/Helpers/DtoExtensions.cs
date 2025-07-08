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
    }
}
