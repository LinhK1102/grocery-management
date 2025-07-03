using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utility.Common
{
    public class ValuesWrapperConverter<T> : JsonConverter<ICollection<T>>
    {
        public override ICollection<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return new List<T>();

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // Nếu là object có $values
            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("$values", out var valuesElement))
            {
                return JsonSerializer.Deserialize<List<T>>(valuesElement.GetRawText(), options) ?? new List<T>();
            }

            // Nếu là array trực tiếp
            if (root.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options) ?? new List<T>();
            }

            return new List<T>();
        }

        public override void Write(Utf8JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
        {
            // Ghi lại theo cấu trúc: { "$values": [...] }
            writer.WriteStartObject();
            writer.WritePropertyName("$values");
            JsonSerializer.Serialize(writer, value, options);
            writer.WriteEndObject();
        }
    }


}
