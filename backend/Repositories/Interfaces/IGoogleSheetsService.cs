using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IGoogleSheetsService
    {
        Task<string> CreateSpreadsheetAsync(string title);
        Task<IList<string>> GetHeaderColumnsAsync(string spreadsheetId, string sheetName);
        Task<bool> SheetExistsAsync(string spreadsheetId, string sheetName);
        Task<bool> CreateSheetIfNotExistsAsync(string spreadsheetId, string sheetName);
        Task<bool> WriteDataWithColumnMatchingAsync(string spreadsheetId, string sheetName, IList<IDictionary<string, object>> data);
        Task<bool> WriteDataWithFixedOrderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows);
        Task<bool> AppendDataAfterHeaderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows);
        Task<bool> UpsertDataAsync(string spreadsheetId, string sheetName, string keyColumn, IList<Dictionary<string, object>> newData);

    }


}
