using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class FileInformation
    {
        public string FileName { get; set; } = string.Empty;    // Tên file (có hoặc không có đuôi mở rộng)
        public string FilePath { get; set; } = string.Empty;    // Đường dẫn tương đối hoặc tuyệt đối
        public string FileUrl { get; set; } = string.Empty;     // URL nếu được truy cập qua web
        public string FileType { get; set; } = string.Empty;    // Loại file (PDF, PNG, EXCEL, ...)
        public long FileSize { get; set; }                      // Kích thước file (bytes)
        public DateTime CreatedAt { get; set; }                 // Thời điểm tạo file
    }

}
