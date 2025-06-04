using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Commons
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }          // Trạng thái thành công hay không
        public string Message { get; set; } = ""; // Thông báo kèm theo
        public T? Data { get; set; }               // Dữ liệu trả về (có thể null)
    }

}
