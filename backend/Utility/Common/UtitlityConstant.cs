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

        //Google Sheets
        public static readonly string Google_Sheet_Credentials_Path = "grocery-465318-4fa9a5b2782c.json";
        public static readonly string Google_Sheet_SheetsBaseUrl = "https://sheets.googleapis.com/v4/spreadsheets";
        public static readonly string Google_Sheet_Rows = "ROWS";
        public static readonly string[] Google_Sheet_Scopes = new[]{ "https://www.googleapis.com/auth/spreadsheets", "https://www.googleapis.com/auth/drive" };
        public static readonly string Google_Drive_FolderId = "1RaxF7HrdmMIk1l39E06dDLHj6EJWcqDq";
        public static readonly string Google_Drive_mimeType = "application/vnd.google-apps.spreadsheet";
        public static readonly string[] Scopes = new[]{"https://www.googleapis.com/auth/drive.metadata.readonly"};


    }
}
