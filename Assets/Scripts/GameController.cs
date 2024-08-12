using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController
{

    public static MainService mainService => ServiceManager.Singleton.GetService<MainService>();
    public static void UnlockFoodKitchen(int id)
    {
        mainService.UnlockFoodKitchen(id);

        var foodKeyUnlockList = ConfigLoader.GetRecord<FoodUnlockRecord>(id).showKeys;
        var param = new OneParam<List<int>>();
        param.value = foodKeyUnlockList;
        GED.ED.dispatchEvent(EventID.OnUnlockFoodKitchen, param);
    }

    public static void UnlockTable(int id)
    {
        mainService.UnlockTable(id);

        var tableKeyUnlockList = ConfigLoader.GetRecord<TableUnlockRecord>(id).showKeys;
        var param = new OneParam<List<int>>();
        param.value = tableKeyUnlockList;
        GED.ED.dispatchEvent(EventID.OnUnlockTable, param);
    }

    public static void EarnMoney(int amount)
    {
        mainService.EarnMoney(amount);

        GED.ED.dispatchEvent(EventID.OnEarnMoney);

        if (mainService.money >= 100)   //temp, hardcode -> config
        {
            UnlockNewArea(1);
        }
    }

    public static void UnlockNewArea(int id)
    {
        mainService.UnlockArea(id);

        GED.ED.dispatchEvent(EventID.OnUnlockArea);
    }
}
