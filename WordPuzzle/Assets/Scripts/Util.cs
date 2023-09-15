using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static List<Vector2Int> eightDir = new List<Vector2Int>() { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1) };

    private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
    };

    public static string Serialize<T>(this T source)
    {
        return JsonConvert.SerializeObject(source, SerializerSettings);
    }

    public static T Deserialize<T>(this string source)
    {
        return JsonConvert.DeserializeObject<T>(source, SerializerSettings);
    }

}
