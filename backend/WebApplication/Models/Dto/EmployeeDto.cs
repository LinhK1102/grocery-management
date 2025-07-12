using System.Text.Json.Serialization;

namespace WebApplication.Models.Dto
{
    public class EmployeeDto
    {
        [JsonPropertyName("employeeId")]
        public int EmployeeId { get; set; }

        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; }

        [JsonPropertyName("employeeEmail")]
        public string EmployeeEmail { get; set; }

        [JsonPropertyName("retailOutletId")]
        public int RetailOutletId { get; set; }
    }

}
