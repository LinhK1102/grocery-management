using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Repositories.Services
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly string _credentialsPath;

        public GoogleSheetsService(IConfiguration config)
        {
            _credentialsPath = config["GoogleSheet:CredentialsPath"];
        }

        private async Task<string> GetAccessTokenAsync()
        {
            GoogleCredential credential = await GoogleCredential
                .FromFileAsync(_credentialsPath, CancellationToken.None);

            // Scope cần thiết cho Sheets và Drive
            credential = credential.CreateScoped(UtitlityConstant.Google_Sheet_Scopes);

            // Lấy access token để gọi Google API
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            return token;
        }

        public async Task<string> CreateSpreadsheetAsync(string title)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            var url = "https://www.googleapis.com/drive/v3/files";

            var requestBody = new
            {
                name = title,
                mimeType = UtitlityConstant.Google_Drive_mimeType,
                parents = new[] { UtitlityConstant.Google_Drive_FolderId } // ID thư mục Drive
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[Google API] Failed to create spreadsheet: {response.StatusCode} - {responseContent}");
                return null!;
            }

            dynamic json = JsonConvert.DeserializeObject(responseContent);
            return json.id;
        }



        private async Task<HttpClient> CreateAuthorizedHttpClientAsync()
        {
            var token = await GetAccessTokenAsync();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IList<string>> GetHeaderColumnsAsync(string spreadsheetId, string sheetName)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            var url = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{sheetName}!1:1";
            var response = await http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content);

            var headers = new List<string>();
            foreach (var h in json.values[0]) headers.Add((string)h);
            return headers;
        }

        public async Task<bool> SheetExistsAsync(string spreadsheetId, string sheetName)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            var url = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}";
            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[Google API] Error checking spreadsheet: {response.StatusCode} - {errorText}");
                return false; // hoặc throw nếu cần
            }

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode || !content.Trim().StartsWith("{"))
            {
                Console.WriteLine($"[Google API] Error checking spreadsheet: {response.StatusCode} - {content}");
                return false;
            }

            dynamic json = JsonConvert.DeserializeObject(content);

            foreach (var sheet in json.sheets)
            {
                if ((string)sheet.properties.title == sheetName) return true;
            }
            return false;
        }

        private async Task<string?> GetApiJsonAsync(string url)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[GoogleSheets API] Failed: {response.StatusCode} - {error}");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> CreateSheetIfNotExistsAsync(string spreadsheetId, string sheetName)
        {
            if (await SheetExistsAsync(spreadsheetId, sheetName)) return true;

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            var url = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}:batchUpdate";
            var requestBody = new
            {
                requests = new[]
                {
                new
                {
                    addSheet = new
                    {
                        properties = new { title = sheetName }
                    }
                }
            }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(url, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> WriteDataWithColumnMatchingAsync(string spreadsheetId,
        string sheetName, IList<IDictionary<string, object>> data)
        {
            var headers = await GetHeaderColumnsAsync(spreadsheetId, sheetName);

            var rows = new List<IList<object>>();
            foreach (var row in data)
            {
                var line = new List<object>();
                foreach (var header in headers)
                {
                    line.Add(row.ContainsKey(header) ? row[header] : "");
                }
                rows.Add(line);
            }

            return await AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows);
        }


        Task<bool> IGoogleSheetsService.WriteDataWithColumnMatchingAsync(string spreadsheetId,
                string sheetName, IList<IDictionary<string, object>> data)
        {
            var headersTask = GetHeaderColumnsAsync(spreadsheetId, sheetName);
            headersTask.Wait(); // dùng sync vì interface không cho async
            var headers = headersTask.Result;

            var rows = new List<IList<object>>();
            foreach (var row in data)
            {
                var line = new List<object>();
                foreach (var header in headers)
                {
                    line.Add(row.ContainsKey(header) ? row[header] : "");
                }
                rows.Add(line);
            }

            return AppendDataAfterHeaderAsync(spreadsheetId, sheetName, rows);
        }


        public async Task<bool> WriteDataWithFixedOrderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            var range = $"{sheetName}!A2";
            var url = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{range}?valueInputOption=RAW";

            var requestBody = new
            {
                range,
                majorDimension = UtitlityConstant.Google_Sheet_Rows,
                values = rows
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await http.PutAsync(url, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AppendDataAfterHeaderAsync(string spreadsheetId, string sheetName, IList<IList<object>> rows)
        {
            using var http = await CreateAuthorizedHttpClientAsync();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());

            var url = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{sheetName}!A1:append?valueInputOption=RAW";
            var requestBody = new
            {
                values = rows,
                majorDimension = UtitlityConstant.Google_Sheet_Rows
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await http.PostAsync(url, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpsertDataAsync(string spreadsheetId, string sheetName, string keyColumn, IList<Dictionary<string, object>> newData)
        {
            var headers = await GetHeaderColumnsAsync(spreadsheetId, sheetName);
            var existing = new Dictionary<string, int>();

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
            var readUrl = $"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{sheetName}";
            var readResp = await http.GetAsync(readUrl);
            var readJson = JsonConvert.DeserializeObject<dynamic>(await readResp.Content.ReadAsStringAsync());

            int keyIndex = headers.IndexOf(keyColumn);
            if (keyIndex < 0) return false;

            int rowIndex = 1;
            foreach (var row in readJson.values)
            {
                if (row.Count > keyIndex)
                    existing[(string)row[keyIndex]] = rowIndex;
                rowIndex++;
            }

            foreach (var data in newData)
            {
                string key = data[keyColumn]?.ToString();
                var row = headers.Select(h => data.ContainsKey(h) ? data[h] : "").ToList();

                string range = existing.ContainsKey(key)
                    ? $"{sheetName}!A{existing[key] + 1}"
                    : $"{sheetName}!A1:append";

                var reqBody = new
                {
                    range,
                    majorDimension = UtitlityConstant.Google_Sheet_Rows,
                    values = new[] { row }
                };

                var content = new StringContent(JsonConvert.SerializeObject(reqBody), Encoding.UTF8, "application/json");
                if (existing.ContainsKey(key))
                    await http.PutAsync($"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{range}?valueInputOption=RAW", content);
                else
                    await http.PostAsync($"{UtitlityConstant.Google_Sheet_SheetsBaseUrl}/{spreadsheetId}/values/{range}?valueInputOption=RAW&insertDataOption=INSERT_ROWS", content);
            }

            return true;
        }
    }

}
