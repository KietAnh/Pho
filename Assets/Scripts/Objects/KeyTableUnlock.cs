using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyTableUnlock : MonoBehaviour, IPointerClickHandler
{
    public int id { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        var tablePrefab = ConfigLoader.GetRecord<TableUnlockRecord>(id).tablePrefab;

        Instantiate(tablePrefab);

        GameController.UnlockTable(id);

        Destroy(gameObject);
    }
}
