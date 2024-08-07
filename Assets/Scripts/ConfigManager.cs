using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class ConfigManager : SingletonBehaviour<ConfigManager>
{
    public List<BaseConfig> configList;
    private Dictionary<Type, BaseConfig> _configMap = new Dictionary<Type, BaseConfig>();
    public void Init()
    {
        foreach (var config in configList)
        {
            string configTypeStr = config.GetType().ToString();
            string recordTypeStr = configTypeStr.Remove(configTypeStr.IndexOf("Config")) + "Record";
            Type recordType = Assembly.GetExecutingAssembly().GetType(recordTypeStr);
            config.CreateRecordMap();
            _configMap.Add(recordType, config);
        }
    }

    public BaseConfig GetConfigAsset<T>() where T : BaseRecord
    {
        if (_configMap.ContainsKey(typeof(T)))
        {
            return _configMap[typeof(T)];
        }
        else
        {
            DevLog.Err("asset have not loaded yet + " + typeof(T));
            return null;
        }
    }

    public T GetConfigRecord<T>(int id) where T : BaseRecord
    {
        if (_configMap.ContainsKey(typeof(T)))
        {
            return _configMap[typeof(T)].GetRecordById(id) as T;
        }
        else
        {
            return null;
        }
    }

    public T GetConfigRecord<T>(string name) where T : BaseRecord
    {
        if (_configMap.ContainsKey(typeof(T)))
        {
            return _configMap[typeof(T)].GetRecordByName(name) as T;
        }
        else
        {
            DevLog.Err("config is invalid >> " + typeof(T));
            return null;
        }
    }
}

public static class ConfigLoader
{
    public static T GetRecord<T>(int id) where T : BaseRecord
    {
        return ConfigManager.Singleton.GetConfigRecord<T>(id);
    }

    public static T GetRecord<T>(string name) where T : BaseRecord
    {
        return ConfigManager.Singleton.GetConfigRecord<T>(name);
    }

    public static BaseConfig GetConfig<T>() where T : BaseRecord
    {
        return ConfigManager.Singleton.GetConfigAsset<T>();
    }
}