using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Data;
using System.IO;

namespace Repositories.Repositories
{
    public class GoogleSheetsRepository
    {
        private readonly SheetsService _sheetsService;
        private readonly string _spreadsheetId;

        public GoogleSheetsRepository(string credentialsPath, string spreadsheetId)
        {
            _spreadsheetId = spreadsheetId;

            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "ASP.NET Export",
            });
        }

        public void WriteData(string sheetName, List<IList<object>> data)
        {
            var range = $"{sheetName}!A1";
            var valueRange = new ValueRange { Values = data };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            updateRequest.Execute();
        }
    }
}
