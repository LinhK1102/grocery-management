using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mapper
{
    public static class ProductMapper
    {
        public static Product ToProductEntity(ProductUpdateDto dto)
        {
            if (dto == null) return null;

            return new Product
            {
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                UnitsInStock = dto.UnitsInStock,
                UnitPrice = dto.UnitPrice,
                BarcodeValue = dto.BarcodeValue,
                ExpiryDuration = dto.ExpiryDuration,

                // KHÔNG ánh xạ Items, Category, Supplier, Order, ProductWarehouses, OrderDetails ở đây
            };
        }

        public static void ToUpdateProductEntity(Product entity, ProductUpdateDto dto)
        {
            if (entity == null || dto == null) return;

            entity.ProductName = dto.ProductName;
            entity.CategoryId = dto.CategoryId;
            entity.SupplierId = dto.SupplierId;
            entity.UnitsInStock = dto.UnitsInStock;
            entity.UnitPrice = dto.UnitPrice;
            entity.BarcodeValue = dto.BarcodeValue;
            entity.ExpiryDuration = dto.ExpiryDuration;
        }
    }
}
