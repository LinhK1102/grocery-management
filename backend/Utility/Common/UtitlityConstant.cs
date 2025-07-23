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
        public static readonly string N_A = "N/A";

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

        //Employee default
        public static readonly string Employee_Default_Name = "Unknown Employee";
        public static readonly string Employee_Email = "unknown@employee.com";

        // Retail Outlet default
        public static readonly string Retail_Outlet_Default_Name = "Unknown Retail Outlet";
        public static readonly string Retail_Outlet_Default_Address = "Unknown Address";

        // Category default
        public static readonly string Category_Default_Name = "Uncategorized";
        public static readonly List<string> Default_Categories = new()
        {
            "UNCATEGORIZED",
            "BEVERAGES",
            "SNACKS",
            "DAIRY PRODUCTS",
            "FRESH PRODUCE",
            "BAKERY",
            "MEAT & POULTRY",
            "FROZEN FOODS",
            "CANNED GOODS",
            "GRAINS & CEREALS",
            "SPICES & SEASONINGS"
        };
        // Item default
        public static readonly string Item_Default_Name = "Unknown Item";
        public static readonly string Item_Default_Description = "No description available";
        public static readonly decimal Item_Default_Price = 0.0m;
        public static readonly int Item_Default_Quantity = 0;

        // Order default
        public static readonly string Order_Default_Status = "Pending";
        public static readonly string Order_Default_Payment_Method = "Cash";
        public static readonly string Order_Default_Shipping_Address = "Unknown Address";

        //Customer default
        public static readonly string Customer_Default_Name = "Unknown Customer";
        public static readonly string Customer_Type_Unknown = "Unknown";
        public static readonly string Customer_Type_Identified = "Identified";

        //Product default
        public static readonly string Product_Default_Name = "Unknown Product";
        public static readonly string Product_Default_Barcode = "0000000000000";

        //Supplier default
        public static readonly string Supplier_Default_Name = "Unknown Supplier";
        public static readonly string Supplier_Default_Address = "Unknown Address";
        public static readonly string Supplier_Default_Email = "suplier.123@gmail.com";
        public static readonly string Supplier_Default_Phone = "0000000000";
        // Warehouse default
        public static readonly string Warehouse_Default_Name = "Unknown Warehouse";
        public static readonly string Warehouse_Default_Location = "Unknown Location";
        // ProductWarehouse default

        //Invoice
        public static readonly string Invoice_File_Path = "invoices/test_invoice.pdf";

        public static readonly List<string> UnitPriceRanges = new()
        {
            "0-50", "51-100", "101-200", "201-500", "500-"
        };

        public static readonly List<string> StockQuantityRanges = new()
        {
            "0-10", "11-50", "51-100", "101-500", "500-"
        };

    }
}
