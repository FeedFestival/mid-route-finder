using System;

namespace Game.Shared.Models.JSON {

public static class JSONExtensions {
    public static T[] AsEnum<T>(this JSONArray array) where T : Enum {
        var result = new T[array.Count];
        for (int i = 0; i < array.Count; i++) {
            result[i] = (T)Enum.ToObject(typeof(T), array[i].AsInt);
        }

        return result;
    }

    public static T AsEnum<T>(this JSONNode node) where T : Enum {
        return (T)Enum.ToObject(typeof(T), node.AsInt);
    }
}

}
