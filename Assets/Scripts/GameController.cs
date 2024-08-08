using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController
{
    public static void UnlockFoodKitchen(int id)
    {
        ServiceManager.Singleton.GetService<MainService>().UnlockFoodKitchen(id);

        var foodKeyUnlockList = ConfigLoader.GetRecord<FoodUnlockRecord>(id).showKeys;
        var param = new OneParam<List<int>>();
        param.value = foodKeyUnlockList;
        GED.ED.dispatchEvent(EventID.OnUnlockFoodKitchen, param);
    }

    public static void UnlockTable(int id)
    {
        ServiceManager.Singleton.GetService<MainService>().UnlockTable(id);

        var tableKeyUnlockList = ConfigLoader.GetRecord<TableUnlockRecord>(id).showKeys;
        var param = new OneParam<List<int>>();
        param.value = tableKeyUnlockList;
        GED.ED.dispatchEvent(EventID.OnUnlockTable, param);
    }
}
