using System.Text.Json;

namespace Bolt.Polymorphic.Serializer.Json;

internal sealed class JsonSerializer(IJsonOptionsProvider optionsProvider)
    : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = optionsProvider.Get();

    public string Serialize<T>(T value)
    {
        return System.Text.Json.JsonSerializer.Serialize(value, _options);
    }

    public T? Deserialize<T>(string value)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(value, _options);
    }

    public ValueTask<T?> Deserialize<T>(Stream source, CancellationToken cancellationToken)
    {
        return System.Text.Json.JsonSerializer.DeserializeAsync<T>(source, _options, cancellationToken);
    }
}