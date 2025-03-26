using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClientBlockChain.Util;

public class ObjectToStringConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}
