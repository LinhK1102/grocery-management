using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public class UtitlityConstant
    {
        //data in database
        public static readonly string Undefined = "UNDEFINDED";
        public static readonly string Unknown = "UNKNOWN";

        //Item actions
        public static readonly int Item_Action_Sell = 0;
        public static readonly int Item_Action_Restock = 1;

        // Google Sheets API
        public static readonly string[] Google_Sheet_Scopes = new[]
        {
        "https://www.googleapis.com/auth/spreadsheets",
        "https://www.googleapis.com/auth/drive"
    };

        public static readonly string Google_Sheet_Credentials_Path = "grocery-465318-4fa9a5b2782c.json";
        public static readonly string Google_Sheet_SheetsBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets";
        public static readonly string Google_Sheet_ValueInputOption = "RAW";
        public static readonly string Google_Sheet_MajorDimension = "ROWS";
        public static readonly string Google_Sheet_DefaultRange = "A:Z";

        // Sheet default naming
        public static string Google_Sheet_Auto_Name => DateTime.Now.ToString("yyyyMMdd_HHmm");

        // Google Drive
        public static readonly string Google_Drive_FolderId = "1RaxF7HrdmMIk1l39E06dDLHj6EJWcqDq";
        public static readonly string Google_Drive_MimeType_Sheet = "application/vnd.google-apps.spreadsheet";
        public static readonly string[] Google_Drive_Metadata_Scopes = new[]
        {
        "https://www.googleapis.com/auth/drive.metadata.readonly"
    };

        // Public link format
        public static readonly string Google_Sheet_Spreadsheet_Url = "https://docs.google.com/spreadsheets/d/";

        // Optional: For project grouping/foldering
        public static readonly string Folder_Grocery_Api_Id = "1RaxF7HrdmMIk1l39E06dDLHj6EJWcqDq";
    }
}
