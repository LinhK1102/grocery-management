using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Mapper
{
    public static class ApiResponseMapper
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Success")
         => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<IEnumerable<T>> SuccessList<T>(IEnumerable<T> data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail<T>(string message = "Operation failed", T? data = default)
            => new() { Success = false, Message = message, Data = data };

        public static ApiResponse<object> Fail(string message = "Operation failed")
            => new() { Success = false, Message = message, Data = null };
    }
}
