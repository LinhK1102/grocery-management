using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.Interfaces;
using System.Net.Http.Headers;
using Utility.Common;

namespace Repositories.Services
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly string _credentialsPath;
        private readonly SheetsService _sheetsService;

        public GoogleSheetsService(IConfiguration config)
        {
            _credentialsPath = config["GoogleSheet:CredentialsPath"]!;

            var credential = GoogleCredential
                .FromFile(_credentialsPath)
                .CreateScoped(UtitlityConstant.Google_Sheet_Scopes);

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Grocery Export Sheets"
            });
        }

        public async Task<string> CreateSpreadsheetAsync(string title)
        {
            Console.WriteLine($"[CreateSpreadsheet] Creating spreadsheet: {title}");
            var spreadsheet = new Spreadsheet
            {
                Properties = new SpreadsheetProperties { Title = title }
            };
            var createRequest = _sheetsService.Spreadsheets.Create(spreadsheet);
            var response = await createRequest.ExecuteAsync();
            Console.WriteLine($"[CreateSpreadsheet] Created with ID: {response.SpreadsheetId}");
            return response.SpreadsheetId;
        }

        public async Task<IList<string>> GetHeaderColumnsAsync(string spreadsheetId, string sheetName)
        {
            Console.WriteLine($"[GetHeaderColumns] Spreadsheet: {spreadsheetId}, Sheet: {sheetName}");
            var range = $"{sheetName}!1:1";
            var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            var headers = response.Values?.FirstOrDefault()?.Select(v => v.ToString()).ToList() ?? new List<string>();
            Console.WriteLine($"[GetHeaderColumns] Headers: {string.Join(", ", headers)}");
            return headers;
        }

        public async Task<bool> SheetExistsAsync(string spreadsheetId, string sheetName)
        {
            Console.WriteLine($"[SheetExists] Spreadsheet: {spreadsheetId}, Checking sheet: {sheetName}");
            var sheet = await _sheetsService.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var exists = sheet.Sheets.Any(s => s.Properties.Title == sheetName);
            Console.WriteLine($"[SheetExists] Exists: {exists}");
            return exists;
        }

        public async Task<bool> CreateSheetIfNotExistsAsync(string spreadsheetId, string sheetName)
        {
            if (await SheetExistsAsync(spreadsheetId, sheetName)) return true;

            Console.WriteLine($"[CreateSheetIfNotExists] Creating sheet: {sheetName}");
            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties { Title = sheetName }
                }
            };

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> { addSheetRequest }
            };

            var request = _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId);
            var response = await request.ExecuteAsync();
            var created = response.Replies.Any();
            Console.WriteLine($"[CreateSheetIfNotExists] Created: {created}");
            return created;
        }

        public async Task<bool> WriteDataWithColumnMatchingAsync(string spreadsheetId, string sheetName, IList<IDictionary<string, object>> data)
        {
            Console.WriteLine($"[WriteDataWithColumnMatching] Spreadsheet: {spreadsheetId}, Sheet: {sheetName}");
            var headers = await GetHeaderColumnsAsync(spreadsheetId, sheetName);
            var rows = new List<IList<object>>();
            foreach (var row in data)
            {
                var line = new List<object>();
                foreach (var h in headers)
                {
                    line.Add(row.ContainsKey(h) ? row[h] : "");
                }
                rows.Add(line);
            }

            return await AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows, headers.Cast<object>().ToList());
        }

        public async Task<bool> WriteDataWithFixedOrderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows)
        {
            Console.WriteLine($"[WriteDataWithFixedOrder] Spreadsheet: {spreadsheetId}, Sheet: {sheetName}");
            var range = $"{sheetName}!A2";
            var valueRange = new ValueRange
            {
                Range = range,
                MajorDimension = "ROWS",
                Values = rows
            };

            var request = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var response = await request.ExecuteAsync();
            Console.WriteLine($"[WriteDataWithFixedOrder] Rows updated: {response.UpdatedRows}");
            return response.UpdatedRows > 0;
        }

        public async Task<bool> AppendDataAfterHeaderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows, IList<object>? headers = null)
        {
            Console.WriteLine($"[AppendDataAfterHeader] Spreadsheet: {spreadsheetId}, Sheet: {sheetName}");

            if (headers != null && headers.Count > 0)
            {
                Console.WriteLine("[AppendDataAfterHeader] Updating headers");
                var formattedHeaders = headers.Select(FormatValue).ToList();

                var headerRange = $"{sheetName}!A1";
                var headerValue = new ValueRange
                {
                    Values = new List<IList<object>> { formattedHeaders },
                    MajorDimension = "ROWS"
                };

                var updateRequest = _sheetsService.Spreadsheets.Values.Update(headerValue, spreadsheetId, headerRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                await updateRequest.ExecuteAsync();
            }

            // Áp dụng FormatValue cho mọi phần tử trong rows
            var formattedRows = rows
                .Select(row => row.Select(FormatValue).ToList<object>())
                .ToList<IList<object>>();

            var range = $"{sheetName}";
            var valueRange = new ValueRange
            {
                Values = formattedRows,
                MajorDimension = "ROWS"
            };

            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

            var response = await appendRequest.ExecuteAsync();
            Console.WriteLine($"[AppendDataAfterHeader] Rows appended: {response.Updates.UpdatedRows}");

            return response.Updates.UpdatedRows > 0;
        }

        private object FormatValue(object? value)
        {
            if (value == null) return "";
            if (value is string || value.GetType().IsPrimitive) return value;
            if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
            return JsonConvert.SerializeObject(value); // fallback cho object phức tạp
        }

        public async Task<bool> UpsertDataAsync(string spreadsheetId, string sheetName, string keyColumn, IList<Dictionary<string, object>> newData)
        {
            Console.WriteLine($"[UpsertData] Spreadsheet: {spreadsheetId}, Sheet: {sheetName}, Key: {keyColumn}");
            var headers = await GetHeaderColumnsAsync(spreadsheetId, sheetName);
            var readRequest = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, sheetName);
            var readResponse = await readRequest.ExecuteAsync();

            var existing = new Dictionary<string, int>();
            int keyIndex = headers.IndexOf(keyColumn);
            if (keyIndex < 0)
            {
                Console.WriteLine("[UpsertData] Key column not found");
                return false;
            }

            int rowIndex = 1;
            foreach (var row in readResponse.Values ?? new List<IList<object>>())
            {
                if (row.Count > keyIndex)
                    existing[row[keyIndex].ToString()] = rowIndex;
                rowIndex++;
            }

            foreach (var data in newData)
            {
                string key = data[keyColumn]?.ToString();
                var row = headers.Select(h => data.ContainsKey(h) ? data[h] : "").ToList<object>();

                var valueRange = new ValueRange
                {
                    Values = new List<IList<object>> { row },
                    MajorDimension = "ROWS"
                };

                if (existing.ContainsKey(key))
                {
                    Console.WriteLine($"[UpsertData] Updating row for key: {key}");
                    var updateRange = $"{sheetName}!A{existing[key] + 1}";
                    var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, updateRange);
                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                    await updateRequest.ExecuteAsync();
                }
                else
                {
                    Console.WriteLine($"[UpsertData] Appending row for key: {key}");
                    var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, spreadsheetId, sheetName);
                    appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                    appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
                    await appendRequest.ExecuteAsync();
                }
            }

            return true;
        }
    }
}
