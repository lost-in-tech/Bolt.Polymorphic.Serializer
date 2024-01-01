using System.Reflection;

namespace Bolt.Polymorphic.Serializer;

internal interface ITypeRegistry
{
    TypeData? TryGet(string typeName);
    TypeData? TryGet(Type type);
    Type[] TypesRegistered { get; }
}

internal sealed class TypeRegistry : ITypeRegistry
{
    private readonly Dictionary<string, TypeData> _typeStoreByName = new();
    private readonly Dictionary<Type, TypeData> _typeStoreByType = new();
    private readonly List<Type> _typesScanned = new List<Type>();

    public TypeData? TryGet(string typeName)
    {
        return _typeStoreByName.GetValueOrDefault(typeName);
    }

    public TypeData? TryGet(Type type)
    {
        return _typeStoreByType.GetValueOrDefault(type);
    }

    public Type[] TypesRegistered => _typesScanned.ToArray();

    internal void Register(Assembly assembly, Type[] typesToScan)
    {
        _typesScanned.AddRange(typesToScan);
        
        foreach(var type in assembly.GetTypes())
        {
            if(type.IsClass)
            {
                if(typesToScan.Any(t => type.IsAssignableTo(t)))
                {
                    var typeData = TypeHelper.GetTypeData(type);

                    _typeStoreByName[type.Name] = typeData;
                    _typeStoreByType[type] = typeData;
                }
            }
        }
    }

    
}

public record TypeData
{
    public required Type Type { get; init; }
    public bool IsSimpleType { get; init; }
    public bool IsNullable { get; init; }
    public bool IsEnum { get; init; }
    public bool IsArray { get; init; }
    public TypeData? CollectionType { get; init; }
    public required Dictionary<string, PropertyData> Properties { get; init; }
    public PropertyData? TryGetProperty(string name) => Properties.GetValueOrDefault(name);

}

public record PropertyData
{
    public required PropertyInfo PropertyInfo { get; init; }
    public required TypeData TypeData { get; init; }
}
