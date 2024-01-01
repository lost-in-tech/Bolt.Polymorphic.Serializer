using System.Collections.Concurrent;

namespace Bolt.Polymorphic.Serializer;
internal static class TypeHelper
{
    public static bool IsSimpleType(Type type)
    {
        return type == typeof(string)
            || type == typeof(int)
            || type == typeof(int?)
            || type == typeof(long)
            || type == typeof(long?)
            || type == typeof(float)
            || type == typeof(float?)
            || type == typeof(double)
            || type == typeof(double?)
            || type == typeof(short)
            || type == typeof(short?)
            || type == typeof(bool)
            || type == typeof(bool?)
            || type == typeof(DateTime)
            || type == typeof(DateTime?)
            || type == typeof(DateTimeOffset)
            || type == typeof(DateTimeOffset?)
            || type.IsEnum
            || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
    }


    private static ConcurrentDictionary<Type, TypeData> _store = new();

    public static TypeData GetTypeData(Type type)
    {
        return _store.GetOrAdd(type, BuildTypeData);
    }
    
    private static TypeData BuildTypeData(Type type)
    {
        var isSimpleType = TypeHelper.IsSimpleType(type);

        var properties = new Dictionary<string, PropertyData>();

        if(!isSimpleType)
        {
            var typeProps = type.GetProperties();

            foreach (var property in typeProps)
            {
                properties[property.Name] = new PropertyData
                {
                    TypeData = BuildTypeData(property.PropertyType),
                    PropertyInfo = property
                };
            }
        }

        var collectionType = type.GetElementType();
        return new TypeData
        {
            Type = type,
            IsArray = type.IsArray,
            IsNullable = Nullable.GetUnderlyingType(type) is null ? true : false,
            IsEnum = type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false),
            IsSimpleType = isSimpleType,
            CollectionType = type.IsArray && collectionType != null ? BuildTypeData(collectionType) : null,
            Properties = properties
        };
    }
}
