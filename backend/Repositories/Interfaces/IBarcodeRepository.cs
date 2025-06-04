using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBarcodeRepository
    {
        Task<UpcProductResponse?> GetProductByBarcodeAsync(string barcode);
    }
}
