namespace Bolt.Polymorphic.Serializer.Xml;

internal sealed class XmlSerializer(XmlDeserializerHelper deserializerHelper) : IXmlSerializer
{
    public Stream? Serialize<T>(T? source) => XmlSerializerHelper.Serialize(source);

    public T? Deserialize<T>(Stream stream) => deserializerHelper.Deserialize<T>(stream);
}