using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TableUnlockConfig", menuName = "Config/TableUnlockConfig", order = 1)]
public class TableUnlockConfig : BaseConfig
{
    public List<TableUnlockRecord> recordList;

    public Dictionary<int, TableUnlockRecord> recordMap;
    public Dictionary<string, TableUnlockRecord> recordMapByName;

    public override void CreateRecordMap()
    {
        recordMap = new Dictionary<int, TableUnlockRecord>();
        recordMapByName = new Dictionary<string, TableUnlockRecord>();
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
public class TableUnlockRecord : BaseRecord
{
    public GameObject keyPrefab;
    public GameObject tablePrefab;
    public List<int> showKeys;
}