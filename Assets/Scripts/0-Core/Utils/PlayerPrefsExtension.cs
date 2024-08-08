using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PlayerPrefsExtension
{
    public static List<string> GetListString(string savePlace, string defaultValue)
    {
        string str = PlayerPrefs.GetString(savePlace, defaultValue);
        List<string> res = new List<string>(str.Split(','));
        return res;
    }

    public static void SetListString(string savePlace, List<string> value)
    {
        string res = "";
        foreach (string str in value)
        {
            res += str + ",";
        }
        if (value.Count > 0)
            res = res.Remove(res.Length - 1, 1);
        PlayerPrefs.SetString(savePlace, res);
    }

    public static Dictionary<string, int> GetDictionary(string place, string defaultValue)
    {
        string str = PlayerPrefs.GetString(place, defaultValue);
        var dic = new Dictionary<string, int>();
        if (str == "")
            return dic;

        List<string> strList = new List<string>(str.Split(';'));

        foreach (var element in strList)
        {
            string key = element.Split(':')[0];
            int value = Int32.Parse(element.Split(':')[1]);
            dic.Add(key, value);
        }

        return dic;
    }
    public static void SetDictionary(string place, Dictionary<string, int> dic)
    {
        string res = "";
        if (dic == null)
            return;
        foreach (KeyValuePair<string, int> kvp in dic)
        {
            res += kvp.Key + ":" + kvp.Value + ";";
        }
        if (res != "")
            res = res.Remove(res.Length - 1, 1);
        PlayerPrefs.SetString(place, res);
    }
    
    public static bool GetBool(string place, bool defaultValue)
    {
        int intDefaultValue = defaultValue ? 1 : 0;
        return PlayerPrefs.GetInt(place, intDefaultValue) == 1 ? true : false;
    }
    public static void SetBool(string place, bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(place, intValue);
    }
}