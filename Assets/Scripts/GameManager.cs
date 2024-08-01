using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

    public Transform customerSpawner;
    public GameObject customerPrefab;
    public Transform owner;

    public Transform[] slotList;
    public Transform[] slotPivotList; // point to find path
    public Transform[] standList;
    public Transform[] dishPlaceHolderList;

    private float _genCustomerTimer;
    private float _timeToGenCustomer;
    private Customer[] _customerList;
    private Queue<OrderInfo> _orderQueue;
    public Queue<OrderInfo> OrderQueue => _orderQueue;

    private Queue<int> _customerQueue;
    public Queue<int> CustomerQueue => _customerQueue;

    public int money { get; private set; }

    private void Start()
    {
        _genCustomerTimer = 0;
        _timeToGenCustomer = Random.Range(2f, 5f);
        _customerList = new Customer[slotList.Length];
        _orderQueue = new Queue<OrderInfo>();
        _customerQueue = new Queue<int>();
    }

    private void Update()
    {
        GenerateCustomerTimer();
    }

    private void GenerateCustomerTimer()   // temp rule: ngẫu nhiên 5 - 10s sẽ gen 1 khách
    {
        _genCustomerTimer += Time.deltaTime;
        if (_genCustomerTimer > _timeToGenCustomer)
        {
            int slotIndex = GetSlotIndex();
            Debug.Log("slot index: " + slotIndex);
            if (slotIndex >= 0)
            {
                var customerObject = Instantiate(customerPrefab, customerSpawner.position, customerSpawner.rotation);
                _customerList[slotIndex] = customerObject.GetComponent<Customer>();
                _customerList[slotIndex].SetSlot(slotIndex);
                
            }

            _genCustomerTimer = 0;
            _timeToGenCustomer = Random.Range(2f, 5f);
        }
    }

    private int GetSlotIndex()
    {
        List<int> freeSlots = new List<int>();
        for (int i = 0; i < _customerList.Length; i++)
        {
            if (_customerList[i] == null)
            {
                freeSlots.Add(i);
            }
        }
        if (freeSlots.Count == 0)
            return -1;
        return freeSlots[Random.Range(0, freeSlots.Count)];
    }

    public Customer GetCustomer(int slotIndex)
    {
        return _customerList[slotIndex];
    }

    public void RemoveCustomer(int slotIndex)
    {
        Debug.Log("Remove Customer: " + slotIndex);
        _customerList[slotIndex] = null;
    }
    
    public void RemoveDish(int slotIndex)
    {
        Destroy(dishPlaceHolderList[slotIndex].GetChild(0).gameObject);
    }

    public void EarnMoney(int count)
    {
        money += count;
        uiManager.UpdateMoney();
    }
}

public class OrderInfo
{
    public int slotIndex;
    public int dishIndex;
}
