using Microsoft.AspNetCore.Mvc;
using Utility.Common;
using Repositories.Interfaces;
using System.Text;
using Repositories.Services;
using System.Globalization;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IGoogleSheetsService _sheets;

        public ExportController(
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            IGoogleSheetsService sheets)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _sheets = sheets;
        }

        [HttpPost("products")]
        public async Task<IActionResult> ExportProducts([FromQuery] string? spreadsheetId, [FromQuery] string groupBy = "category", [FromQuery] string writeMode = "append")
        {
            if (string.IsNullOrEmpty(spreadsheetId))
            {
                spreadsheetId = await _sheets.CreateSpreadsheetAsync("Products Export");

                if (string.IsNullOrEmpty(spreadsheetId))
                    return StatusCode(500, SystemStatus.Fail("Failed to create spreadsheet."));
            }


            var products = _productRepo.GetAllProduct();
            if (!products.Any()) return NotFound(SystemStatus.Fail("No product data found"));

            var grouped = products.GroupBy(p => groupBy == "category" ? p.Category?.CategoryName ?? "Uncategorized" : "All");

            foreach (var group in grouped)
            {
                var sheetName = group.Key.Length > 30 ? group.Key.Substring(0, 30) : group.Key;
                await _sheets.CreateSheetIfNotExistsAsync(spreadsheetId, sheetName);

                if (writeMode == "upsert")
                {
                    var dictData = group.Select(p => new Dictionary<string, object>
                    {
                        ["ProductId"] = p.ProductId,
                        ["ProductName"] = p.ProductName,
                        ["UnitPrice"] = p.UnitPrice,
                        ["UnitsInStock"] = p.UnitsInStock,
                        ["BarcodeValue"] = p.BarcodeValue
                    }).ToList();

                    await _sheets.UpsertDataAsync(spreadsheetId, sheetName, "ProductId", dictData);
                }
                else
                {
                    var rows = new List<IList<object>>();
                    foreach (var p in group)
                    {
                        rows.Add(new List<object> { p.ProductId, p.ProductName, p.UnitPrice, p.UnitsInStock, p.BarcodeValue });
                    }
                    await _sheets.AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows);
                }
            }
            if (string.IsNullOrEmpty(spreadsheetId))
            {
                spreadsheetId = await _sheets.CreateSpreadsheetAsync("Products Export");

                if (string.IsNullOrEmpty(spreadsheetId))
                    return StatusCode(500, SystemStatus.Fail("Failed to create spreadsheet."));
            }

            var sheetUrl = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}";
            return Ok(SystemStatus.Success(new { spreadsheetId, url = sheetUrl }, "Products exported"));
        }

        [HttpPost("orders")]
        public async Task<IActionResult> ExportOrders([FromQuery] string? spreadsheetId, [FromQuery] string groupBy = "month")
        {
            if (string.IsNullOrEmpty(spreadsheetId))
            {
                spreadsheetId = await _sheets.CreateSpreadsheetAsync("Orders Export");
            }

            var orders = _orderRepo.GetAllOrders();
            if (!orders.Any()) return NotFound(SystemStatus.Fail("No order data found"));

            var grouped = orders.GroupBy(o => groupBy == "month" ? o.OrderDate.ToString("yyyy-MM") : "All");

            foreach (var group in grouped)
            {
                var sheetName = group.Key;
                await _sheets.CreateSheetIfNotExistsAsync(spreadsheetId, sheetName);

                var rows = new List<IList<object>>();
                foreach (var o in group)
                {
                    var total = o.OrderDetails.Sum(item => item.UnitPriceAtTimeOfSale * item.Quantity);
                    rows.Add(new List<object> { o.OrderId, o.CustomerId, o.OrderDate.ToString("yyyy-MM-dd"),
                        total.ToString("C", CultureInfo.GetCultureInfo("vi-VN"))});
                }
                await _sheets.AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows);
            }

            return Ok(SystemStatus.Success(new { spreadsheetId }, "Orders exported"));
        }
    }
}

