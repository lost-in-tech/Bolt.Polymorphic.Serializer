using System.Collections;
using System.Reflection;
using System.Xml;

namespace Bolt.Polymorphic.Serializer.Xml;

internal static class XmlSerializerHelper
{
    public static Stream? Serialize<T>(T? source)
    {
        if (source == null) return null;

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = ("\t");
        settings.OmitXmlDeclaration = true;

        var stream = new MemoryStream();

        // Create the XmlWriter object and write some content.
        using var writer = XmlWriter.Create(stream, settings);

        var type = typeof(T);

        Serialize(type, type.Name, source, writer);

        writer.Flush();

        stream.Position = 0;

        return stream;
    }


    private static void Serialize(Type type, string elementName, object? value, XmlWriter writer)
    {
        if (value == null) return;

        var typeName = type.Name;

        writer.WriteStartElement(elementName);

        if(typeName != elementName)
        {
            writer.WriteAttributeString("_type", typeName);
        }

        var properties = type.GetProperties().Where(x => x.CanRead);

        var complexProperties = new List<PropertyInfo>();
        foreach (var prop in properties)
        {
            if (TypeHelper.IsSimpleType(prop.PropertyType))
            {
                writer.WriteAttributeString(prop.Name, prop.GetValue(value)?.ToString());
            }
            else
            {
                complexProperties.Add(prop);
            }
        }

        foreach (var prop in complexProperties)
        {
            var propValue = prop.GetValue(value);

            if (propValue is null) continue;

            if (propValue is IEnumerable collection)
            {
                writer.WriteStartElement(prop.Name);

                foreach (var elm in collection)
                {
                    var itemType = elm.GetType();
                    Serialize(itemType, itemType.Name, elm, writer);
                }

                writer.WriteEndElement();
            }
            else
            {
                Serialize(prop.PropertyType.IsInterface ? propValue.GetType() : prop.PropertyType, prop.Name, propValue, writer);
            }
        }

        writer.WriteEndElement();
    }


}