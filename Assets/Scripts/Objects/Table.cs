
using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform[] slotList;
    public Transform[] slotPivotList; // point to find path
    public Transform[] standList;
    public Transform[] dishPlaceHolderList;

    public int slotCount => slotList.Length;
}
