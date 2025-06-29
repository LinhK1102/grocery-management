using BusinessObjects.Commons;
using BusinessObjects.DTOs;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class InvoiceDAO
    {
        private readonly ApplicationDbContext _context;
        public InvoiceDAO(ApplicationDbContext context) => _context = context;
        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int invoiceId)
        {
            return await _context.Invoice
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<ApiResponse<Invoice>> CreateInvoice(Invoice invoice)
        {
            invoice.CreatedDate = DateTime.Now;

            _context.Add(invoice);
            await _context.SaveChangesAsync();

            // Lấy lại invoice đầy đủ (kèm InvoiceItems và Product)
            var fullInvoice = await GetInvoiceWithDetailsAsync(invoice.InvoiceId);

            if (fullInvoice == null)
            {
                return new ApiResponse<Invoice>
                {
                    Success = false,
                    Message = "Invoice not found after save.",
                    Data = null
                };
            }

            // Tạo DTO để export
            var invoiceData = new InvoiceData
            {
                InvoiceId = fullInvoice.InvoiceId.ToString(),
                CreatedDate = fullInvoice.CreatedDate,
                CustomerName = fullInvoice.CustomerName,
                Items = fullInvoice.InvoiceItems.Select(item => new InvoiceItem
                {
                    ProductId = item.ProductId,
                    Product = item.Product,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            // Tạo nội dung hóa đơn
            string invoiceText = GenerateInvoiceText(invoiceData);

            // Export PDF và lấy đường dẫn
            string filePath = ExportInvoiceToPdf(invoiceText);

            // Cập nhật đường dẫn vào DB
            fullInvoice.InvoiceFilePath = filePath;
            await _context.SaveChangesAsync();

            return new ApiResponse<Invoice>
            {
                Success = true,
                Message = "Invoice created and exported successfully.",
                Data = fullInvoice
            };
        }

        // Tạo nội dung hóa đơn dạng text
        public static string GenerateInvoiceText(InvoiceData invoice)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"INVOICE ID: {invoice.InvoiceId}");
            sb.AppendLine($"Date: {invoice.CreatedDate:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Customer: {invoice.CustomerName}");
            sb.AppendLine(new string('-', 40));

            foreach (var item in invoice.Items)
            {
                sb.AppendLine($"{item.Product?.ProductName ?? "Unknown"} x{item.Quantity} @ {item.UnitPrice:C} = {(item.Quantity * item.UnitPrice):C}");
            }

            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"TOTAL: {invoice.Total:C}");

            return sb.ToString();
        }

        // Export PDF từ nội dung text và trả về đường dẫn
        public static string ExportInvoiceToPdf(string content, string fileNameWithoutExt = null)
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12, XFontStyle.Regular);

            double y = 40;
            foreach (var line in content.Split('\n'))
            {
                gfx.DrawString(line.Trim(), font, XBrushes.Black, new XRect(40, y, page.Width, page.Height), XStringFormats.TopLeft);
                y += 20;
            }

            string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\.."));
            string exportDir = Path.Combine(root, "Exports");
            Directory.CreateDirectory(exportDir);

            string fileName = fileNameWithoutExt ?? $"Invoice_{DateTime.Now:yyyyMMdd_HHmmss}";
            string filePath = Path.Combine(exportDir, fileName + ".pdf");

            doc.Save(filePath);
            return filePath;
        }
    }
}
