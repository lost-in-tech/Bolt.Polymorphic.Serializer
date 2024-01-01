using System.Collections;
using System.Reflection;
using System.Xml;

namespace Bolt.Polymorphic.Serializer.Xml;

internal sealed class XmlDeserializerHelper
{
    private readonly ITypeRegistry _typeRegistry;

    public XmlDeserializerHelper(ITypeRegistry typeRegistry)
    {
        _typeRegistry = typeRegistry;
    }

    public T? Deserialize<T>(Stream stream)
    {
        using var reader = XmlReader.Create(stream);

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                return BuildElement<T>(reader);
            }
        }

        return default;
    }

    private T? BuildElement<T>(XmlReader reader)
    {
        var typeName = reader.GetAttribute("_type") ?? reader.Name;
        var typeData = _typeRegistry.TryGet(typeName) ?? TypeHelper.GetTypeData(typeof(T));
        
        return (T?)BuildObject(typeData, reader);
    }

    private object? BuildArray(TypeData typeData, XmlReader reader)
    {
        if (typeData.CollectionType == null) return null;

        var listType = typeof(List<>);
        var constructedType = listType.MakeGenericType(new[] { typeData.CollectionType.Type });

        var result = Activator.CreateInstance(constructedType) as IList;

        if (result == null) return null;

        var toArray = constructedType.GetMethod("ToArray");


        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement) break;
            if (reader.NodeType == XmlNodeType.Whitespace) continue;
            if (reader.NodeType == XmlNodeType.Element)
            {
                var typeName = reader.GetAttribute("_type") ?? reader.Name;
                var arrayElementTypeData = _typeRegistry.TryGet(typeName);
                var obj = BuildObject(arrayElementTypeData, reader);
                result.Add(obj);
            }
        }


        if (toArray != null)
        {
            return toArray.Invoke(result, null);
        }

        return null;
    }

    private object? BuildObject(TypeData? typeData, XmlReader reader)
    {
        if (typeData == null) return null;

        var typeName = reader.GetAttribute("_type");
        if(!string.IsNullOrWhiteSpace(typeName))
        {
            typeData = _typeRegistry.TryGet(typeName);

            if (typeData == null) return null;
        }


        var result = Activator.CreateInstance(typeData.Type);

        if (typeData.Properties != null)
        {
            foreach (var property in typeData.Properties)
            {
                var att = reader.GetAttribute(property.Key);

                if (att != null)
                {
                    SetPropertyValue(property.Value.PropertyInfo, result, att);
                }
            }
        }

        if (reader.IsEmptyElement) return result;

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement) break;
            if (reader.NodeType == XmlNodeType.Whitespace) continue;
            if (reader.NodeType == XmlNodeType.Element)
            {
                var propData = typeData.TryGetProperty(reader.Name);

                if (propData != null)
                {
                    if (propData.TypeData.IsArray)
                    {
                        var obj = BuildArray(propData.TypeData, reader);

                        propData.PropertyInfo.SetValue(result, obj, null);
                    }
                    else
                    {
                        var obj = BuildObject(propData.TypeData, reader);

                        propData.PropertyInfo.SetValue(result, obj, null);
                    }
                }
            }
        }

        return result;
    }

    private void SetPropertyValue(PropertyInfo prop, object? source, object? value)
    {
        if (value == null) return;

        var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);

        if (prop.PropertyType.IsEnum)
        {
            prop.SetValue(source, Enum.Parse(prop.PropertyType, (string)value));
        }
        else if (nullableType != null && nullableType.IsEnum)
        {
            prop.SetValue(source, Enum.Parse(nullableType, (string)value));
        }


        else if (prop.PropertyType == typeof(int))
        {
            prop.SetValue(source, int.TryParse(value.ToString(), out var result) ? result : 0);
        }
        else if (prop.PropertyType == typeof(int?))
        {
            prop.SetValue(source, int.TryParse(value.ToString(), out var result) ? result : null);
        }


        else if (prop.PropertyType == typeof(long))
        {
            prop.SetValue(source, long.TryParse(value.ToString(), out var result) ? result : 0);
        }
        else if (prop.PropertyType == typeof(long?))
        {
            prop.SetValue(source, long.TryParse(value.ToString(), out var result) ? result : null);
        }


        else if (prop.PropertyType == typeof(decimal))
        {
            prop.SetValue(source, decimal.TryParse(value.ToString(), out var result) ? result : 0);
        }
        else if (prop.PropertyType == typeof(decimal?))
        {
            prop.SetValue(source, decimal.TryParse(value.ToString(), out var result) ? result : null);
        }


        else if (prop.PropertyType == typeof(double))
        {
            prop.SetValue(source, double.TryParse(value.ToString(), out var result) ? result : 0);
        }
        else if (prop.PropertyType == typeof(double?))
        {
            prop.SetValue(source, double.TryParse(value.ToString(), out var result) ? result : null);
        }



        else if (prop.PropertyType == typeof(bool))
        {
            prop.SetValue(source, bool.TryParse(value.ToString(), out var result) ? result : false);
        }
        else if (prop.PropertyType == typeof(bool?))
        {
            prop.SetValue(source, bool.TryParse(value.ToString(), out var result) ? result : null);
        }


        else if (prop.PropertyType == typeof(DateTime))
        {
            prop.SetValue(source, DateTime.TryParse(value.ToString(), out var result) ? result : DateTime.MinValue);
        }
        else if (prop.PropertyType == typeof(DateTime?))
        {
            prop.SetValue(source, DateTime.TryParse(value.ToString(), out var result) ? result : null);
        }

        else if (prop.PropertyType == typeof(float))
        {
            prop.SetValue(source, float.TryParse(value.ToString(), out var result) ? result : 0);
        }
        else if (prop.PropertyType == typeof(float?))
        {
            prop.SetValue(source, float.TryParse(value.ToString(), out var result) ? result : null);
        }


        else if (prop.PropertyType == typeof(short))
        {
            prop.SetValue(source, short.TryParse(value.ToString(), out var result) ? result : 0);
        }
        else if (prop.PropertyType == typeof(short?))
        {
            prop.SetValue(source, short.TryParse(value.ToString(), out var result) ? result : null);
        }


        else
        {
            prop.SetValue(source, value);
        }
    }
}