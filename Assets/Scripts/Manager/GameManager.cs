
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : SingletonBehaviour<GameManager>
{
    public UIManager uiManager;

    public Transform customerSpawner;
    public GameObject customerPrefab;

    public Owner owner;

    public GameObject treeContainer;

    //public Transform[] slotList;
    //public Transform[] slotPivotList; // point to find path
    //public Transform[] standList;
    //public Transform[] dishPlaceHolderList;

    private List<Table> _tableList = new List<Table>();
    private List<Kitchen> _kitchenList = new List<Kitchen>();

    private float _genCustomerTimer;
    private float _timeToGenCustomer;
    private Customer[] _customerList;   // list customer đang có chỗ ngồi, id là id của slot
    private Queue<OrderInfo> _orderQueue;
    public Queue<OrderInfo> OrderQueue => _orderQueue;

    private Queue<int> _customerQueue;
    public Queue<int> CustomerQueue => _customerQueue;


    protected override void Awake()
    {
        base.Awake();

        ServiceManager.Singleton.Init();
    }

    private void Start()
    {
        Initialize();

        _genCustomerTimer = 0;
        _timeToGenCustomer = Random.Range(2f, 5f);
        int slotCount = 0;
        for (int i = 0; i < _tableList.Count; i++)
        {
            slotCount += _tableList[i].slotCount;
        }
        _customerList = new Customer[slotCount];
        _orderQueue = new Queue<OrderInfo>();
        _customerQueue = new Queue<int>();

        GED.ED.addListener(EventID.OnUnlockFoodKitchen, OnUnlockFoodKitchen_UnlockFoodKey);
        GED.ED.addListener(EventID.OnUnlockTable, OnUnlockTable_UnlockTable);
        GED.ED.addListener(EventID.OnUnlockArea, OnUnlockArea_UnlockArea);

        GED.ED.addListener(EventID.OnUnlockNotWalkableObject, OnUnlockNotWalkableObject_RescanPathFinderGraph);
        GED.ED.addListener(EventID.OnCleanTree, OnCleanTree_RescanPathFinderGraph);
    }

    protected override void OnDestroy()
    {
        GED.ED.removeListener(EventID.OnUnlockFoodKitchen, OnUnlockFoodKitchen_UnlockFoodKey);
        GED.ED.removeListener(EventID.OnUnlockTable, OnUnlockTable_UnlockTable);
        GED.ED.removeListener(EventID.OnUnlockArea, OnUnlockArea_UnlockArea);

        GED.ED.removeListener(EventID.OnUnlockNotWalkableObject, OnUnlockNotWalkableObject_RescanPathFinderGraph);
        GED.ED.removeListener(EventID.OnCleanTree, OnCleanTree_RescanPathFinderGraph);

        ServiceManager.Singleton.SaveAllData();

        base.OnDestroy();
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
        InitTree();

        ScanPathFinderGraph();
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
            var kitchenObject = Instantiate(kitchenPrefab);
            _kitchenList.Add(kitchenObject.GetComponent<Kitchen>());
        }

        if (_kitchenList.Count > 0)
        {
            owner.kitchenTrans = _kitchenList[0].transform; // temp
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
            var tableObject = Instantiate(tablePrefab);
            _tableList.Add(tableObject.GetComponent<Table>());
        }

        UnlockTableKey(showKeyTableUnlockIds);
    }

    private void InitTree()   // init async ?
    {
        var treeCleanIdList = ServiceManager.Singleton.GetService<MainService>().treeCleanIdList;
        
        if (treeCleanIdList.Count == 0)
        {
            Instantiate(treeContainer);
        }
        else
        {
            var newTreeContainer = new GameObject("tree-container");
            for (int i = 0; i < treeContainer.transform.childCount; i++)
            {
                var child = treeContainer.transform.GetChild(i);
                if (child.CompareTag("tree") && !treeCleanIdList.Contains(i))
                {
                    var treePrefab = child;
                    var treeObject = Instantiate(treePrefab, newTreeContainer.transform);
                    treeObject.name = "tree-" + i;
                }
                
            }
        }
    }

    private void ScanPathFinderGraph()
    {
        TimeRecordUtil.Begin("Scan PathFinder Graph"); // 0.25s -> hơi lâu -> scan async
        AstarPath.active.Scan();
        TimeRecordUtil.End();
    }

    private void RescanPathFinderGraph(Collider2D collider)
    {
        TimeRecordUtil.Begin("Rescan PathFinder Graph");
        AstarPath.active.UpdateGraphs(collider.bounds);
        TimeRecordUtil.End();
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
            DevLog.Log("slot index: " + slotIndex);
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

    public (int, int) ConvertGlobalSlotIndexToTableIndexAndLocalSlotIndex(int slotIndex) // check exception kỹ
    {
        for (int i = 0; i < _tableList.Count; i++)
        {
            if (slotIndex - _tableList[i].slotCount < 0)
            {
                return (i, slotIndex);
            }
            slotIndex -= _tableList[i].slotCount;
        }
        return (-1, -1);
    }

    public Transform GetSlot(int slotIndex)
    {
        var (tableIndex, localSlotIndex) = ConvertGlobalSlotIndexToTableIndexAndLocalSlotIndex(slotIndex);
        if (tableIndex >= 0 && localSlotIndex >= 0)
        {
            return _tableList[tableIndex].slotList[localSlotIndex];
        }
        return null;
    }

    public Transform GetSlotPivot(int slotIndex)  
    {
        var tuple = ConvertGlobalSlotIndexToTableIndexAndLocalSlotIndex(slotIndex);
        if (tuple.Item1 >= 0 && tuple.Item2 >=0)
        {
            int tableIndex = tuple.Item1;
            int localSlotIndex = tuple.Item2;
            return _tableList[tableIndex].slotPivotList[localSlotIndex];
        }
        else
        {
            return null;
        }
    }

    public Transform GetStand(int slotIndex)
    {
        var (tableIndex, localSlotIndex) = ConvertGlobalSlotIndexToTableIndexAndLocalSlotIndex(slotIndex);
        if (tableIndex >= 0 && localSlotIndex >= 0)
        {
            return _tableList[tableIndex].standList[localSlotIndex];
        }
        return null;
    }

    public Transform GetDishPlaceHolder(int slotIndex)
    {
        var (tableIndex, localSlotIndex) = ConvertGlobalSlotIndexToTableIndexAndLocalSlotIndex(slotIndex);
        if (tableIndex >= 0 && localSlotIndex >= 0)
        {
            return _tableList[tableIndex].dishPlaceHolderList[localSlotIndex];
        }
        return null;
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
        Transform dishPlaceHolder = GetDishPlaceHolder(slotIndex);
        Destroy(dishPlaceHolder.GetChild(0).gameObject);
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
        var param = gameEvent.Data as OneParam<int>;
        int id = param.value1;

        var foodKeyUnlockList = ConfigLoader.GetRecord<FoodUnlockRecord>(id).showKeys;
        var kitchenPrefab = ConfigLoader.GetRecord<FoodUnlockRecord>(id).kitchenPrefab;
        var kitchenObject = Instantiate(kitchenPrefab);
        var kitchen = kitchenObject.GetComponent<Kitchen>();
        _kitchenList.Add(kitchen);

        if (_kitchenList.Count > 0 && owner.kitchenTrans == null)
        {
            owner.kitchenTrans = _kitchenList[0].transform; // temp
        }

        RescanPathFinderGraph(kitchenObject.GetComponent<Collider2D>());

        UnlockFoodKey(foodKeyUnlockList);
    }

    private void OnUnlockTable_UnlockTable(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<int>;
        int id = param.value1;

        var tableKeyUnlockList = ConfigLoader.GetRecord<TableUnlockRecord>(id).showKeys;
        var tablePrefab = ConfigLoader.GetRecord<TableUnlockRecord>(id).tablePrefab;
        var tableObject = Instantiate(tablePrefab);
        var table = tableObject.GetComponent<Table>();
        _tableList.Add(table);
        Array.Resize(ref _customerList, _customerList.Length + table.slotCount);

        RescanPathFinderGraph(tableObject.GetComponent<Collider2D>());

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

    private void OnUnlockNotWalkableObject_RescanPathFinderGraph(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<Collider2D>;
        RescanPathFinderGraph(param.value1);
    }
    private void OnCleanTree_RescanPathFinderGraph(GameEvent gameEvent)
    {
        var param = gameEvent.Data as OneParam<Collider2D>;
        RescanPathFinderGraph(param.value1);
    }

    #endregion
}

public class OrderInfo
{
    public int slotIndex;
    public int dishIndex;
}
