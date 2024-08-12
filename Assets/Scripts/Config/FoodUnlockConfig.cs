using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodUnlockConfig", menuName = "Config/FoodUnlockConfig", order = 1)]
public class FoodUnlockConfig : BaseConfig
{
    public List<FoodUnlockRecord> recordList;

    public Dictionary<int, FoodUnlockRecord> recordMap;
    public Dictionary<string, FoodUnlockRecord> recordMapByName;

    public override void CreateRecordMap()
    {
        recordMap = new Dictionary<int, FoodUnlockRecord>();
        recordMapByName = new Dictionary<string, FoodUnlockRecord>();
        foreach (var record in recordList)
        {
            if (!recordMap.ContainsKey(record.id))
            {
                recordMap.Add(record.id, record);
            }
            if (!recordMapByName.ContainsKey(record.name))
            {
                recordMapByName.Add(record.name, record);
            }
        }
    }
    public override BaseRecord GetRecordById(int id)
    {
        if (recordMap.ContainsKey(id))
            return recordMap[id];
        else
        {
            DevLog.Log("id is invalid: " + id);
            return null;
        }
    }

    public override BaseRecord GetRecordByName(string name)
    {
        if (recordMapByName.ContainsKey(name))
            return recordMapByName[name];
        else
        {
            DevLog.Log("name is invalid: " + name);
            return null;
        }
    }
}

[Serializable]
public class FoodUnlockRecord : BaseRecord
{
    public FoodType type;
    public GameObject keyPrefab;
    public GameObject kitchenPrefab;
    public int areaUnlockCondition;
    public List<int> showKeys;
}

public enum FoodType
{
    TomChienXu,
    Sushi,
}