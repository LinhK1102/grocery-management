using BusinessObjects.Commons;
using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
      public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDAO _invoiceDAO;

        public InvoiceRepository(InvoiceDAO invoiceDAO)
        {
            _invoiceDAO = invoiceDAO;
        }

        public Task<List<Invoice>> GetAllAsync() => _invoiceDAO.GetAllInvoicesAsync();

        //public Task<Invoice?> GetByIdAsync(int id) => _invoiceDAO.GetByIdAsync(id);

        public Task<Invoice?> GetWithDetailsAsync(int id) => _invoiceDAO.GetInvoiceWithDetailsAsync(id);

        public Task<ApiResponse<Invoice>> CreateAsync(Invoice invoice) => _invoiceDAO.CreateInvoice(invoice);

        public async Task<List<InvoiceItem>> GetItemsByInvoiceId(int invoiceId)
        {
            return await _invoiceDAO.GetItemsByInvoiceIdAsync(invoiceId);
        }

        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int id)
        {
            return await _invoiceDAO.GetInvoiceWithDetailsAsync(id);
        }

        public async Task<ApiResponse<InvoiceItem>> CreateInvoiceItem(InvoiceItem item)
        {
            return await _invoiceDAO.CreateInvoiceItemAsync(item);
        }

        public async Task<ApiResponse<FileInformation>> ExportInvoiceToPdf(string content, string fileNameWithoutExt)
        {
            string filePath = _invoiceDAO.ExportInvoiceToPdf(content, fileNameWithoutExt);

            var fileInfo = new FileInformation
            {
                FileName = fileNameWithoutExt,
                FilePath = filePath
            };

            return new ApiResponse<FileInformation>
            {
                Success = true,
                Message = "File generated successfully",
                Data = fileInfo
            };
        }

        public async Task<Invoice> GetWithDetailsAsync(Invoice invoice)
        {
          return await _invoiceDAO.GetInvoiceWithDetailsAsync(invoice.InvoiceId);
        }

        IEnumerable<InvoiceItem> IInvoiceRepository.GetItemsByInvoiceId(int invoiceId)
        {
            throw new NotImplementedException();
        }

        InvoiceItem? IInvoiceRepository.GetInvoiceWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<ApiResponse<Invoice>> IInvoiceRepository.CreateInvoiceItem(InvoiceItem item)
        {
            throw new NotImplementedException();
        }

        string IInvoiceRepository.ExportInvoiceToPdf(string content, string fileNameWithoutExt)
        {
            throw new NotImplementedException();
        }

    }
}
