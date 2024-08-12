using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainService : BaseService
{
    public int money { get; private set; }
    public int foodUnlockId { get; private set; }
    public int tableUnlockId { get; private set; }
    public int areaUnlockId { get; private set; }
    public override void LoadData()
    {
        money = DataIO.ReadInt("Money", 0);
        foodUnlockId = DataIO.ReadInt("FoodUnlockId", 0);
        tableUnlockId = DataIO.ReadInt("TableUnlockId", 0);
        areaUnlockId = DataIO.ReadInt("AreaUnlockId", 0);
    }


    public override void SaveData()
    {
        DataIO.WriteInt("Money", money);
        DataIO.WriteInt("FoodUnlockId", foodUnlockId);
        DataIO.WriteInt("TableUnlockId", tableUnlockId);
        DataIO.WriteInt("AreaUnlockId", areaUnlockId);
    }
    public override void ClearCache()
    {
        money = 0;
        foodUnlockId = 0;
        tableUnlockId = 0;
        areaUnlockId = 0;
    }
    public override void ReloadData()
    {
        ClearCache();
        LoadData();
    }

    public void EarnMoney(int amount)
    {
        money += amount;
    }

    public void UnlockFoodKitchen(int id)
    {
        foodUnlockId = id;
    }

    public void UnlockTable(int id)
    {
        tableUnlockId = id;
    }

    public void UnlockArea(int id)
    {
        areaUnlockId = id;
    }
}