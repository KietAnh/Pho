
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
        GED.ED.addListener(EventID.OnUnlockArea, OnUnlockArea_UnlockArea);
    }

    private void OnDestroy()
    {
        GED.ED.removeListener(EventID.OnUnlockFoodKitchen, OnUnlockFoodKitchen_UnlockFoodKey);
        GED.ED.removeListener(EventID.OnUnlockTable, OnUnlockTable_UnlockTableKey);
        GED.ED.removeListener(EventID.OnUnlockArea, OnUnlockArea_UnlockArea);

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

        UnlockFoodKey(showKeyFoodUnlockIds);
    }

    private void InitTableUnlock()
    {
        int tableUnlockId = ServiceManager.Singleton.GetService<MainService>().tableUnlockId;
        DevLog.Log("" + tableUnlockId);
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

        UnlockTableKey(showKeyTableUnlockIds);
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

    public void UnlockFoodKey(List<int> ids)
    {
        int areaUnlockId = ServiceManager.Singleton.GetService<MainService>().areaUnlockId;

        for (int i = 0; i < ids.Count; i++)
        {
            if (areaUnlockId >= ConfigLoader.GetRecord<FoodUnlockRecord>(ids[i]).areaUnlockCondition)
            {
                var keyPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(ids[i]).keyPrefab;
                var keyObject = Instantiate(keyPrefab);
                keyObject.GetComponent<KeyFoodUnlock>().id = ids[i];
            }
        }
    }

    public void UnlockTableKey(List<int> ids)
    {
        int areaUnlockId = ServiceManager.Singleton.GetService<MainService>().areaUnlockId;

        for (int i = 0; i < ids.Count; i++)
        {
            if (areaUnlockId >= ConfigLoader.GetRecord<TableUnlockRecord>(ids[i]).areaUnlockCondition)
            {
                var keyPrefab = ConfigLoader.GetRecord<TableUnlockRecord>(ids[i]).keyPrefab;
                var keyObject = Instantiate(keyPrefab);
                keyObject.GetComponent<KeyTableUnlock>().id = ids[i];
            }
        }
    }


    #region event handler

    private void OnUnlockFoodKitchen_UnlockFoodKey(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<List<int>>;

        List<int> foodKeyUnlockList = param.value;
        //int areaUnlockId = ServiceManager.Singleton.GetService<MainService>().areaUnlockId;
        //foreach (var foodKeyId in foodKeyUnlockList)
        //{
        //    if (areaUnlockId >= ConfigLoader.GetRecord<FoodUnlockRecord>(foodKeyId).areaUnlockCondition)
        //    {
        //        var keyPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(foodKeyId).keyPrefab;
        //        var keyObject = Instantiate(keyPrefab);
        //        keyObject.GetComponent<KeyFoodUnlock>().id = foodKeyId;
        //    }
        //}

        UnlockFoodKey(foodKeyUnlockList);
    }

    private void OnUnlockTable_UnlockTableKey(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<List<int>>;

        List<int> tableKeyUnlockList = param.value;
        //int areaUnlockId = ServiceManager.Singleton.GetService<MainService>().areaUnlockId;
        //foreach (var tableKeyId in tableKeyUnlockList)
        //{
        //    if (areaUnlockId >= ConfigLoader.GetRecord<TableUnlockRecord>(tableKeyId).areaUnlockCondition)
        //    {
        //        var keyPrefab = ConfigLoader.GetRecord<TableUnlockRecord>(tableKeyId).keyPrefab;
        //        var keyObject = Instantiate(keyPrefab);
        //        keyObject.GetComponent<KeyTableUnlock>().id = tableKeyId;
        //    }
        //}

        UnlockTableKey(tableKeyUnlockList);
    }

    private void OnUnlockArea_UnlockArea(GameEvent gameEvent)
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

        UnlockFoodKey(showKeyFoodUnlockIds);

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

        UnlockTableKey(showKeyTableUnlockIds);
    }

    #endregion
}

public class OrderInfo
{
    public int slotIndex;
    public int dishIndex;
}
