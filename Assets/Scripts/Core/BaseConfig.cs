
using UnityEngine;

public abstract class BaseConfig : ScriptableObject
{

    public abstract void CreateRecordMap();
    public abstract BaseRecord GetRecordById(int id);
    public abstract BaseRecord GetRecordByName(string name);
}

public class BaseRecord
{
    public string name;
    public int id;
}