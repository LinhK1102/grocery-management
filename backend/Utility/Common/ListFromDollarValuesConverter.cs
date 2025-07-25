using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utility.Common
{
    public class ListFromDollarValuesConverter<T> : JsonConverter<List<T>>
    {
        public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);

            // Tạo bản sao option KHÔNG có converter này để tránh vòng lặp
            var newOptions = new JsonSerializerOptions(options);
            var thisConverter = newOptions.Converters.FirstOrDefault(c => c is ListFromDollarValuesConverter<T>);
            if (thisConverter != null)
            {
                newOptions.Converters.Remove(thisConverter);
            }

            // Nếu là object và có $values
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("$values", out var valuesElement))
            {
                return JsonSerializer.Deserialize<List<T>>(valuesElement.GetRawText(), newOptions);
            }

            // Nếu là mảng
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(doc.RootElement.GetRawText(), newOptions);
            }

            return null;
        }



        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options); // không cần custom ghi
        }
    }
}
