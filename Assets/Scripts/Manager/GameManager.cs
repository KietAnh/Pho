using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;

    public Transform customerSpawner;
    public GameObject customerPrefab;

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

    private void Awake()
    {
        ServiceManager.Singleton.Init();
    }

    private void Start()
    {
        Initialize();

        _genCustomerTimer = 0;
        _timeToGenCustomer = Random.Range(2f, 5f);
        _customerList = new Customer[slotList.Length];
        _orderQueue = new Queue<OrderInfo>();
        _customerQueue = new Queue<int>();

        GED.ED.addListener(EventID.OnUnlockFoodKitchen, OnUnlockFoodKitchen_UnlockFoodKey);
        GED.ED.addListener(EventID.OnUnlockTable, OnUnlockTable_UnlockTableKey);
    }

    private void OnDestroy()
    {
        GED.ED.removeListener(EventID.OnUnlockFoodKitchen, OnUnlockFoodKitchen_UnlockFoodKey);
        GED.ED.removeListener(EventID.OnUnlockTable, OnUnlockTable_UnlockTableKey);

        ServiceManager.Singleton.SaveAllData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ServiceManager.Singleton.SaveAllData();
        }
    }

    private void Initialize()
    {
        InitFoodKitchenUnlock();
        InitTableUnlock();
    }

    private void InitFoodKitchenUnlock()
    {
        int foodUnlockId = ServiceManager.Singleton.GetService<MainService>().foodUnlockId;
        var showKeyFoodUnlockIds = new List<int>();

        if (foodUnlockId == 0)
        {
            showKeyFoodUnlockIds.Add(1);
        }
        else
        {
            showKeyFoodUnlockIds = ConfigLoader.GetRecord<FoodUnlockRecord>(foodUnlockId).showKeys;
        }

        for (int i = 1; i <= foodUnlockId; i++)
        {
            var kitchenPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(i).kitchenPrefab;
            Instantiate(kitchenPrefab);
        }

        for (int i = 0; i < showKeyFoodUnlockIds.Count; i++)
        {
            var keyPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(showKeyFoodUnlockIds[i]).keyPrefab;
            var keyObject = Instantiate(keyPrefab);
            keyObject.GetComponent<KeyFoodUnlock>().id = showKeyFoodUnlockIds[i];
        }
    }

    private void InitTableUnlock()
    {
        int tableUnlockId = ServiceManager.Singleton.GetService<MainService>().tableUnlockId;
        var showKeyTableUnlockIds = new List<int>();

        if (tableUnlockId == 0)
        {
            showKeyTableUnlockIds.Add(1);
        }
        else
        {
            showKeyTableUnlockIds = ConfigLoader.GetRecord<TableUnlockRecord>(tableUnlockId).showKeys;
        }

        for (int i = 1; i <= tableUnlockId; i++)
        {
            var tablePrefab = ConfigLoader.GetRecord<TableUnlockRecord>(i).tablePrefab;
            Instantiate(tablePrefab);
        }

        for (int i = 0; i < showKeyTableUnlockIds.Count; i++)
        {
            var keyPrefab = ConfigLoader.GetRecord<TableUnlockRecord>(showKeyTableUnlockIds[i]).keyPrefab;
            var keyObject = Instantiate(keyPrefab);
            keyObject.GetComponent<KeyTableUnlock>().id = showKeyTableUnlockIds[i];
        }
    }

    private void Update()
    {
        //GenerateCustomerTimer();
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


    #region event handler

    private void OnUnlockFoodKitchen_UnlockFoodKey(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<List<int>>;

        List<int> foodKeyUnlockList = param.value;
        foreach (var foodKeyId in foodKeyUnlockList)
        {
            var keyPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(foodKeyId).keyPrefab;
            var keyObject = Instantiate(keyPrefab);
            keyObject.GetComponent<KeyFoodUnlock>().id = foodKeyId;
        }
    }

    private void OnUnlockTable_UnlockTableKey(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<List<int>>;

        List<int> tableKeyUnlockList = param.value;
        foreach (var tableKeyId in tableKeyUnlockList)
        {
            var keyPrefab = ConfigLoader.GetRecord<TableUnlockRecord>(tableKeyId).keyPrefab;
            var keyObject = Instantiate(keyPrefab);
            keyObject.GetComponent<KeyTableUnlock>().id = tableKeyId;
        }
    }

    #endregion
}

public class OrderInfo
{
    public int slotIndex;
    public int dishIndex;
}
