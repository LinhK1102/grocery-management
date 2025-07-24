using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utility.Common
{
    // Xử lý các list bọc bởi $values hoặc array trực tiếp
    public class ValuesWrapperConverter<T> : JsonConverter<ICollection<T>>
    {
        public override ICollection<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return new List<T>();

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("$values", out var valuesElement))
            {
                return JsonSerializer.Deserialize<List<T>>(valuesElement.GetRawText(), options) ?? new List<T>();
            }

            if (root.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(root.GetRawText(), options) ?? new List<T>();
            }

            return new List<T>();
        }

        public override void Write(Utf8JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$values");
            JsonSerializer.Serialize(writer, value, options);
            writer.WriteEndObject();
        }
    }

    // Factory để tự động gán converter cho List<T> và ICollection<T>
    public class ValuesWrapperConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType) return false;

            var genericDef = typeToConvert.GetGenericTypeDefinition();
            return genericDef == typeof(List<>) || genericDef == typeof(ICollection<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var itemType = typeToConvert.GetGenericArguments()[0];
            var converterType = typeof(ValuesWrapperConverter<>).MakeGenericType(itemType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}
