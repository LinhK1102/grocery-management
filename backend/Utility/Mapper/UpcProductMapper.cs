using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mapper
{
    public static class UpcProductMapper
    {
        public static Product ToProductEntity(this UpcProductResponse response)
        {
            return new Product
            {
                ProductName = response.Title,
                BarcodeValue = response.Barcode,
                UnitPrice = decimal.TryParse(response.Msrp, out var price) ? price : 0,
                CategoryId = 1,
                SupplierId = 1,
                UnitsInStock = 0
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
