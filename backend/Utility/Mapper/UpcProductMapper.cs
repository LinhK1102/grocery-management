using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Utility.Mapper
{
    public static class UpcProductMapper
    {
        public static Product ToProductEntity(this UpcProductResponse response)
        {
            return new Product
            {
                ProductName = Convention.ToUpperOrNA(response.Title),
                BarcodeValue = Convention.ToUpperOrNA(response.Barcode),
                UnitPrice = decimal.TryParse(response.Msrp, out var price) ? price : 0,
                UnitsInStock = 0,
                ExpiryDuration = DateTime.Now.AddMonths(6), // default giả định 6 tháng

                // Gán tạm Category object
                Category = new Category
                {
                    CategoryName = Convention.ToUpperOrNA(response.Category)
                },
                CategoryId = 0, // placeholder để xử lý sau khi kiểm tra DB

                // Gán Supplier object
                Supplier = new Supplier
                {
                    SupplierName = Convention.ToUpperOrNA(response.Manufacturer),
                    SupplierEmail = "N/A",
                    SupplierPhoneNumber = "N/A"
                },
                SupplierId = 0, // placeholder
                Items = new List<Item>(),
                OrderDetails = new List<OrderDetail>(),
                ProductWarehouses = new List<ProductWarehouse>()
            };
        }


        public static Item ToItemEntity(this UpcProductResponse response, int productId)
        {
            return new Item
            {
                ProductId = productId,
                Barcode = response.Barcode,
                Quantity = 1,
                ImportedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                ManufactureDate = DateTime.Now
            };
        }
    }
}
