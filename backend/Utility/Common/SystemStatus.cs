using BusinessObjects.Commons;
using Utility.Common;

namespace Utility.Common
{
    public static class SystemStatus
    {
        public static ApiResponse<T> Success<T>(T data, string message = "") =>
            new ApiResponse<T> { Success = true, Message = message, Data = data };

        public static ApiResponse<string> Success(string message) =>
            new ApiResponse<string> { Success = true, Message = message, Data = null };

        public static ApiResponse<string> Fail(string message) =>
            new ApiResponse<string> { Success = false, Message = message, Data = null };
    }
}
