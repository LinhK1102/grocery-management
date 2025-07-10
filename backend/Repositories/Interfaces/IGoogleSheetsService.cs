using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IGoogleSheetsService
    {
        // Tạo spreadsheet mới
        Task<string> CreateSpreadsheetAsync(string title);

        // Kiểm tra sheet có tồn tại trong spreadsheet
        Task<bool> SheetExistsAsync(string spreadsheetId, string sheetName);

        // Tạo sheet nếu chưa tồn tại
        Task<bool> CreateSheetIfNotExistsAsync(string spreadsheetId, string sheetName);

        // Lấy danh sách header (dòng đầu tiên)
        Task<IList<string>> GetHeaderColumnsAsync(string spreadsheetId, string sheetName);

        // Ghi dữ liệu dựa theo header có sẵn
        Task<bool> WriteDataWithColumnMatchingAsync(string spreadsheetId, string sheetName, IList<IDictionary<string, object>> data);

        // Ghi dữ liệu theo thứ tự hàng (không quan tâm tên cột)
        Task<bool> WriteDataWithFixedOrderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows);

        // Ghi dữ liệu sau header (gồm cả cập nhật header nếu có)
        Task<bool> AppendDataAfterHeaderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows, IList<object>? headers = null);

        // Ghi đè hoặc thêm dòng mới theo khóa chính
        Task<bool> UpsertDataAsync(string spreadsheetId, string sheetName, string keyColumn, IList<Dictionary<string, object>> newData);
        //Task<bool> UpdateLastRowAsync(string spreadsheetId, string sheetName, IList<object> rowData);
    }

}
