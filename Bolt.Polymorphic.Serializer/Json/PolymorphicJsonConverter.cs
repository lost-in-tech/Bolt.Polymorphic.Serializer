using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bolt.Polymorphic.Serializer.Json;

internal class PolymorphicJsonConverter<T>(ITypeRegistry typeRegistry) : JsonConverter<T>
{
    private const string TypePropertyName = "_type";
    
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        
        if (!jsonDocument.RootElement.TryGetProperty(TypePropertyName, out var typeProperty))
        {
            throw new JsonException();
        }

        var typePropertyName = typeProperty.GetString();
            
        if (string.IsNullOrWhiteSpace(typePropertyName) == false)
        {
            var type = typeRegistry.TryGet(typePropertyName);
            if (type != null)
            {
                var jsonObject = jsonDocument.RootElement.GetRawText();
                var result = System.Text.Json.JsonSerializer.Deserialize(jsonObject, type.Type, options);

                return result == null ? default : (T)result;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                System.Text.Json.JsonSerializer.Serialize(writer, default(T)?.GetType(), options);
                break;
            default:
                {
                    var type = value.GetType();

                    // Create a JsonObject to hold the serialized object
                    var jsonObject = new JsonObject();

                    // Serialize the implementation type and store it in the JsonObject
                    using (var jsonDocument = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(value, type, options)))
                    {
                        // Add extra property to the JsonObject
                        var typeName = JsonDocument.Parse($"\"{type.Name}\"").RootElement;

                        jsonObject.Add(TypePropertyName, typeName);

                        foreach (var property in jsonDocument.RootElement.EnumerateObject())
                        {
                            jsonObject.Add(property.Name, property.Value);
                        }

                        // Write the JsonObject to the Utf8JsonWriter
                        jsonObject.WriteTo(writer);
                    }

                    break;
                }
        }
    }
}
