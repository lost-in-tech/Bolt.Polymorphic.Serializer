namespace Bolt.Polymorphic.Serializer.Json;

public interface IJsonSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string value);
    ValueTask<T?> Deserialize<T>(Stream source, CancellationToken cancellationToken = default);
}