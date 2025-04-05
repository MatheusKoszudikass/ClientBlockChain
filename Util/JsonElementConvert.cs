using System.Reflection;
using System.Text.Json;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;

namespace ClientBlockChain.Util;

public static class JsonElementConvert
{
    public static object ConvertToObject(JsonElement jsonElement)
    {
        return IdentifierTypeToProcesss(jsonElement);
    }

    private static object IdentifierTypeToProcesss(JsonElement jsonElement)
    {
        if (JsonMatchesType<ClientMine>(jsonElement))
            return jsonElement.Deserialize<ClientMine>()!;

        if (JsonMatchesType<LogEntry>(jsonElement))
            return jsonElement.Deserialize<LogEntry>()!;

        if (JsonMatchesType<ClientCommandMine>(jsonElement))
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
                return (ClientCommandMine)jsonElement.GetInt32();

            throw new InvalidOperationException("Expected a number for enum deserialization.");
        }

        if (JsonMatchesType<ClientCommandLog>(jsonElement))
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
                return (ClientCommandLog)jsonElement.GetInt32();

            throw new InvalidOperationException("Expected a number for enum deserialization.");
        }

        if (jsonElement.ValueKind == JsonValueKind.String)
            return jsonElement.GetString()!;

        throw new ArgumentException("Unsupported data type", nameof(jsonElement));
    }

    private static bool JsonMatchesType<T>(JsonElement element)
    {
        if (typeof(T).IsEnum)
        {
            if(element.ValueKind == JsonValueKind.Number)
            {
                return Enum.IsDefined(typeof(T), element.GetInt32());
            }

            if(element.ValueKind == JsonValueKind.String)
            {
                return Enum.TryParse(typeof(T), element.GetString(), out _);
            }
            
            return false;
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        var propertyJson = element.EnumerateObject().Select(p => p.Name).ToHashSet();
        var propertyClass = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name).ToHashSet();

        return propertyJson.SetEquals(propertyClass);
    }
}