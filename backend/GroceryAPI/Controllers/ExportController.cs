using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Services;
using System.Globalization;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IGoogleSheetsService _sheets;
        private readonly GoogleDriveService _driveService;

        public ExportController(
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            IGoogleSheetsService sheets,
            GoogleDriveService driveService)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _sheets = sheets;
            _driveService = driveService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> ExportProducts([FromQuery] string? spreadsheetId, [FromQuery] string groupBy = "category", [FromQuery] string writeMode = "append")
        {
            spreadsheetId ??= await _sheets.CreateSpreadsheetAsync("Products Export");
            if (string.IsNullOrEmpty(spreadsheetId))
                return StatusCode(500, SystemStatus.Fail("Failed to create spreadsheet."));

            var products = await _productRepo.GetAllProduct();
            if (!products.Any())
                return NotFound(SystemStatus.Fail("No product data found"));

            var grouped = products.GroupBy(p => groupBy == "category" ? p.Category?.CategoryName ?? "Uncategorized" : "All");

            foreach (var group in grouped)
            {
                var sheetName = group.Key.Length > 30 ? group.Key[..30] : group.Key;
                await _sheets.CreateSheetIfNotExistsAsync(spreadsheetId, sheetName);

                var rows = group.Select(p => new List<object>
                {
                    p.ProductId,
                    p.ProductName,
                    p.UnitPrice,
                    p.UnitsInStock,
                    p.BarcodeValue
                }).Cast<IList<object>>().ToList();

                var headers = new List<object> { "ProductId", "ProductName", "UnitPrice", "UnitsInStock", "BarcodeValue" };

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
                    await _sheets.AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows, headers);
                }
            }

            var sheetUrl = $"{UtitlityConstant.Google_Sheet_Spreadsheet_Url}{spreadsheetId}";
            return Ok(SystemStatus.Success(new { spreadsheetId, url = sheetUrl }, "Products exported"));
        }

        [HttpPost("orders")]
        public async Task<IActionResult> ExportOrders([FromQuery] string? spreadsheetId, [FromQuery] string groupBy = "month")
        {
            spreadsheetId ??= await _sheets.CreateSpreadsheetAsync("Orders Export");

            var orders = _orderRepo.GetAllOrders();
            if (!orders.Any())
                return NotFound(SystemStatus.Fail("No order data found"));

            var grouped = orders.GroupBy(o => groupBy == "month" ? o.OrderDate.ToString("yyyy-MM") : "All");

            foreach (var group in grouped)
            {
                var sheetName = group.Key;
                await _sheets.CreateSheetIfNotExistsAsync(spreadsheetId, sheetName);

                var rows = group.Select(o => new List<object>
                {
                    o.OrderId,
                    o.CustomerId,
                    o.OrderDate.ToString("yyyy-MM-dd"),
                    o.OrderDetails.Sum(i => i.UnitPriceAtTimeOfSale * i.Quantity).ToString("C", CultureInfo.GetCultureInfo("vi-VN"))
                }).Cast<IList<object>>().ToList();

                var headers = new List<object> { "OrderId", "CustomerId", "OrderDate", "TotalAmount" };
                await _sheets.AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows, headers);
            }

            var spreadsheetUrl = $"{UtitlityConstant.Google_Sheet_Spreadsheet_Url}{spreadsheetId}";
            return Ok(SystemStatus.Success(new { spreadsheetUrl }, "Orders exported"));
        }

        [HttpPost("sheet/create")]
        public async Task<IActionResult> CreateSheet([FromQuery] string? folderId, [FromQuery] string? sheetName)
        {
            folderId ??= UtitlityConstant.Google_Drive_FolderId;
            sheetName ??= UtitlityConstant.Google_Sheet_Auto_Name;

            var sheetId = await _driveService.CreateSheetInFolderAsync(folderId, sheetName);

            return Ok(SystemStatus.Success(new
            {
                sheetID = sheetId,
                sheetUrl = $"{UtitlityConstant.Google_Sheet_Spreadsheet_Url}{sheetId}"
            }, "Create successfully"));
        }

        [HttpPost("sheet/write")]
        public async Task<IActionResult> UpdateOrAppendData(
            [FromQuery] string spreadsheetId,
            [FromQuery] string sheetName,
            [FromQuery] string? keyColumn,
            [FromBody] List<Dictionary<string, object>> data)
        {
            if (string.IsNullOrEmpty(spreadsheetId) || string.IsNullOrEmpty(sheetName))
                return BadRequest(SystemStatus.Fail("Missing spreadsheetId or sheetName"));

            await _sheets.CreateSheetIfNotExistsAsync(spreadsheetId, sheetName);

            if (!string.IsNullOrEmpty(keyColumn))
            {
                var success = await _sheets.UpsertDataAsync(spreadsheetId, sheetName, keyColumn, data);
                if (!success)
                    return StatusCode(500, SystemStatus.Fail("Failed to upsert data"));
            }
            else
            {
                var headers = data.FirstOrDefault()?.Keys.Cast<object>().ToList();
                if (headers == null)
                    return BadRequest(SystemStatus.Fail("No data provided"));

                var rows = data.Select(row => row.Values.Cast<object>().ToList()).Cast<IList<object>>().ToList();
                await _sheets.AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows, headers);
            }

            return Ok(SystemStatus.Success(new
            {
                sheetUrl = $"{UtitlityConstant.Google_Sheet_Spreadsheet_Url}{spreadsheetId}"
            }, "Data written to sheet"));
        }

        //[HttpPost("update-last-row")]
        //public async Task<IActionResult> UpdateLastRow([FromBody] UpdateSheetRequest request)
        //{
        //    var result = await _sheets.UpdateLastRowAsync(request.SpreadsheetId, request.SheetName, request.RowData);

        //    return Ok(new
        //    {
        //        success = result,
        //        message = result ? "Updated successfully" : "Update failed"
        //    });
        //}
    }

    public class UpdateSheetRequest
    {
        public string SpreadsheetId { get; set; } = null!;
        public string SheetName { get; set; } = "Sheet1";
        public List<object> RowData { get; set; } = new();
    }
}
