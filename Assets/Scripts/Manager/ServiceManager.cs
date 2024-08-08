using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServiceManager : SingletonTemplate<ServiceManager>
{
    private Dictionary<Type, BaseService> _serviceMap = new Dictionary<Type, BaseService>();

    //protected override void Awake()
    //{
    //    base.Awake();
    //    Init();
    //}

    public void Init()
    {
        //_serviceMap.Add(typeof(PlayerService), new PlayerService());
        // add service here...
        _serviceMap.Add(typeof(MainService), new MainService());

        //LoadAllData();
        PreloadData();
    }
    public void PreloadData()
    {
        //_serviceMap[typeof(PlayerService)].LoadData();
        // load service here...
        _serviceMap[typeof(MainService)].LoadData();
    }
    public T GetService<T>() where T : BaseService
    {
        Type type = typeof(T);
        if (_serviceMap.ContainsKey(type))
        {
            return _serviceMap[type] as T;
        }
        return null;
    }

    public void LoadAllData()
    {
        foreach (var pair in _serviceMap)
        {
            pair.Value.LoadData();
        }
    }

    public void SaveAllData()
    {
        foreach (var pair in _serviceMap)
        {
            pair.Value.SaveData();
        }
    }

    public void ReloadData()
    {
        foreach (var pair in _serviceMap)
        {
            pair.Value.ReloadData();
        }
    }
}
