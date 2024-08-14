
using UnityEngine;

public static class TimeRecordUtil
{
    public static float timeBegin;
    public static string context;
    public static void Begin(string str)
    {
        context = str;
        timeBegin = Time.realtimeSinceStartup;
    }

    public static void End()
    {
        DevLog.Log(context + " cost time: " + (Time.realtimeSinceStartup - timeBegin));
    }
}
