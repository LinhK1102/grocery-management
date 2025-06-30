using BusinessObjects.Commons;
using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBarcodeRepository
    {
        Task<UpcProductResponse?> GetProductInfoFromApiAsync(string barcode);
        Task<ApiResponse<Product>> GetOrCreateProductByBarcodeAsync(string barcode);
    }
}
