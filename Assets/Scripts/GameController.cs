using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController
{

    public static MainService mainService => ServiceManager.Singleton.GetService<MainService>();

    public static void EarnMoney(int amount)
    {
        mainService.EarnMoney(amount);

        GED.ED.dispatchEvent(EventID.OnEarnMoney);

        if (mainService.money >= 100)   //temp, hardcode -> config
        {
            UnlockNewArea(1);
        }
    }
    public static void UnlockFoodKitchen(int id)
    {
        mainService.UnlockFoodKitchen(id);

        //var foodKeyUnlockList = ConfigLoader.GetRecord<FoodUnlockRecord>(id).showKeys;

        GED.ED.dispatchEvent(EventID.OnUnlockFoodKitchen, new OneParam<int>(id));
        //GED.ED.dispatchEvent(EventID.OnUnlockNotWalkableObject, new OneParam<Collider2D>(collider));
    }

    public static void UnlockTable(int id)
    {
        mainService.UnlockTable(id);
        //var tableKeyUnlockList = ConfigLoader.GetRecord<TableUnlockRecord>(id).showKeys;

        GED.ED.dispatchEvent(EventID.OnUnlockTable, new OneParam<int>(id));
        //GED.ED.dispatchEvent(EventID.OnUnlockNotWalkableObject, new OneParam<Collider2D>(collider));
    }

    public static void UnlockNewArea(int id)
    {
        if (mainService.areaUnlockId >= id)
            return;

        mainService.UnlockArea(id);

        GED.ED.dispatchEvent(EventID.OnUnlockArea);
    }

    public static void CleanTree(int id, Collider2D collider)
    {
        mainService.CleanTree(id);

        GED.ED.dispatchEvent(EventID.OnCleanTree, new OneParam<Collider2D>(collider));
    }
}
