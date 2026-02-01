using System.Collections.Generic;

namespace Game.Shared.Development {

public static class DebugExtensions {
    public static string DebugLog(this IEnumerable<string> source) {
        return string.Join(", ", source);
    }

    public static string DebugLog(this List<int> source) {
        return string.Join(", ", source);
    }

    public static string DebugLog<T>(this List<T> source) {
        return string.Join(", ", source);
    }

    public static string DebugLog<T>(this T[] source) {
        return string.Join(", ", source);
    }

    public static string DebugLog<T1, T2>(this Dictionary<T1, T2> source) {
        string s = "\n";
        foreach (KeyValuePair<T1, T2> kvp in source) {
            s += $"\t{kvp.Key}: {kvp.Value}\n";
        }

        return s;
    }
}

}
