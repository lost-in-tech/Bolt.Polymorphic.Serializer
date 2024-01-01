namespace Bolt.Polymorphic.Serializer.Xml;

public interface IXmlSerializer
{
    Stream? Serialize<T>(T? source);
    T? Deserialize<T>(Stream stream);
}

public static class XmlSerializerExtensions
{
    public static async Task<string?> Serialize<T>(this IXmlSerializer serializer, T? source, CancellationToken ct)
    {
        if (source == null) return null;
        await using var ms = serializer.Serialize(source);
        if (ms == null) return null;
        using var sr = new StreamReader(ms);
        return await sr.ReadToEndAsync(ct);
    }

    public static T? Deserialize<T>(this IXmlSerializer serializer, string source)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        writer.Write(source);
        writer.Flush();
        stream.Position = 0;
        var result = serializer.Deserialize<T>(stream);
        return result;
    }
}