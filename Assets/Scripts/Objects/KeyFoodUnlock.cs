using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyFoodUnlock : MonoBehaviour, IPointerClickHandler
{
    public int id { get; set; }
    public void OnPointerClick(PointerEventData eventData)
    {
        //var kitchenPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(id).kitchenPrefab;

        //var kitchenObject = Instantiate(kitchenPrefab);

        GameController.UnlockFoodKitchen(id);

        Destroy(gameObject);
    }
}
