using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net;

public static class DataIO
{
    #region read


    public static int ReadInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static float ReadFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static string ReadString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static bool ReadBool(string key, bool defaultValue)
    {
        return PlayerPrefsExtension.GetBool(key, defaultValue);
    }

    public static T ReadObject<T>(string key, T defaultValue)
    {
        string jsonStr = ReadString(key, "");
        T value = JsonUtility.FromJson<T>(jsonStr);
        if (value == null)
            return defaultValue;
        return value;
    }

    public static Dictionary<K, V> ReadDictionary<K, V>(string key, Dictionary<K, V> defaultValue)
    {
        string jsonStr = ReadString(key, "");
        var customDic = JsonUtility.FromJson<CustomDictionary<K, V>>(jsonStr);
        if (customDic == null)
            return defaultValue;
        return customDic.GetDic();
    }

    public static List<T> ReadList<T>(string key, List<T> defaultValue)
    {
        string jsonStr = ReadString(key, "");
        var customList = JsonUtility.FromJson<CustomList<T>>(jsonStr);
        if (customList == null)
            return defaultValue;
        return customList.GetList();
    }

    #endregion

    #region write

    public static void WriteInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void WriteFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void WriteString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void WriteBool(string key, bool value)
    {
        PlayerPrefsExtension.SetBool(key, value);
    }

    public static void WriteObject<T>(string key, T value)
    {
        string jsonStr = JsonUtility.ToJson(value);
        WriteString(key, jsonStr);
    }

    public static void WriteDictionary<K, V>(string key, Dictionary<K, V> dic)
    {
        var customDic = new CustomDictionary<K, V>(dic);
        string jsonStr = JsonUtility.ToJson(customDic);
        WriteString(key, jsonStr);
    }

    public static void WriteList<T>(string key, List<T> list)
    {
        var customList = new CustomList<T>(list);
        string jsonStr = JsonUtility.ToJson(customList);
        WriteString(key, jsonStr);
    }

    #endregion
}

/// <summary>
/// Custom Dictionary để có thể đọc, ghi file json
/// </summary>
[System.Serializable]
public class CustomDictionary<K, V>
{
    public List<K> keys;
    public List<V> values;

    public Dictionary<K, V> GetDic()
    {
        Dictionary<K, V> dic = new Dictionary<K, V>();
        for (int i = 0; i < keys.Count; i++)
        {
            if (!dic.ContainsKey(keys[i]))
            {
                dic.Add(keys[i], values[i]);
            }
        }
        return dic;
    }

    public CustomDictionary(Dictionary<K, V> dic)
    {
        keys = dic.Keys.ToList<K>();
        values = dic.Values.ToList<V>();
    }
}

/// <summary>
/// Custom List để có thể đọc, ghi file json
/// </summary>
[System.Serializable]
public class CustomList<T>
{
    public List<T> list;

    public CustomList(List<T> list)
    {
        this.list = list;
    }

    public List<T> GetList()
    {
        return list;
    }
}

public static class PREF_KEY
{
    public const string CurLevel = "CurLevel";
    public const string IsFirstPlay = "FirstPlay";

    public const string BackCount = "BackCount";
    public const string HelpCount = "HelpCount";
    public const string ResetCount = "ResetCount";

    public const string SoundOn = "SoundOn";
    public const string MusicOn = "MusicOn";
    public const string VibraOn = "VibraOn";

    public const string NoAds = "NoAds";

    public const string NextGuide = "NextGuide";

    public const string Version = "Version";
}