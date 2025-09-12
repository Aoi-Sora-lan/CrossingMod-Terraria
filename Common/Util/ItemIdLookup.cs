using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ID;

namespace CrossingMachine.Common.Util;

public static class ItemIdLookup
{
    private static readonly Dictionary<string, short> nameToId;
    private static readonly Dictionary<short, string> idToName;

    static ItemIdLookup()
    {
        nameToId = new Dictionary<string, short>(StringComparer.Ordinal);
        idToName = new Dictionary<short, string>();
    
        Type type = typeof(ItemID);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(short));
    
        foreach (var field in fields)
        {
            string name = field.Name;
            short value = (short)field.GetValue(null);
        
            nameToId[name] = value;
            idToName[value] = name;
        }
    }
    public static short? GetIdByName(string name)
    {
        return nameToId.TryGetValue(name, out var id) ? id : (short?)null;
    }
    public static string GetNameById(short id)
    {
        return idToName.GetValueOrDefault(id);
    }
    public static IReadOnlyDictionary<string, short> GetAllNameToIdMappings()
    {
        return nameToId;
    }
    public static IReadOnlyDictionary<short, string> GetAllIdToNameMappings()
    {
        return idToName;
    }
}
