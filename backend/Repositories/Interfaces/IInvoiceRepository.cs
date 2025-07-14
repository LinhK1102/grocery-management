using BusinessObjects.Commons;
using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IInvoiceRepository : ISeedableRepository
    {
        IEnumerable<InvoiceItem> GetItemsByInvoiceId(int invoiceId);
        InvoiceItem? GetInvoiceWithDetailsAsync(int id);
        Task<ApiResponse<Invoice>> CreateInvoiceItem(InvoiceItem item);
        string ExportInvoiceToPdf(string content, string fileNameWithoutExt = null);
        Task<List<Invoice>> GetAllAsync();
        Task<Invoice> GetWithDetailsAsync(Invoice invoice);
        Task<ApiResponse<Invoice>> CreateAsync(Invoice invoice);
    }

}
