
using UnityEngine;

public static class DevLog
{
    public static bool isDebug = true;
    public static void Log(string text)
    {
        if (isDebug)
            Debug.Log("dev log >> " + text);
    }

    public static void Warn(string text)
    {
        if (isDebug)
            Debug.LogWarning("dev log >> " + text);
    }

    public static void Err(string text)
    {
        if (isDebug)
            Debug.LogError("dev log >> " + text);
    }
}
