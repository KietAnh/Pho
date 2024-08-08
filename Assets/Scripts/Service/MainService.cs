using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainService : BaseService
{
    public int foodUnlockId { get; private set; }
    public int tableUnlockId { get; private set; }
    public override void LoadData()
    {
        foodUnlockId = DataIO.ReadInt("FoodUnlockId", 0);
        tableUnlockId = DataIO.ReadInt("TableUnlockId", 0);
    }


    public override void SaveData()
    {
        DataIO.WriteInt("FoodUnlockId", foodUnlockId);
        DataIO.WriteInt("TableUnlockId", foodUnlockId);
    }
    public override void ClearCache()
    {
        foodUnlockId = 0;
        tableUnlockId = 0;
    }
    public override void ReloadData()
    {
        ClearCache();
        LoadData();
    }

    public void UnlockFoodKitchen(int id)
    {
        foodUnlockId = id;
    }

    public void UnlockTable(int id)
    {
        tableUnlockId = id;
    }
}