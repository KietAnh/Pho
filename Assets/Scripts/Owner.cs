
using UnityEngine;
using DG.Tweening;
using Pathfinding;

public class Owner : MonoBehaviour
{
    public GameManager gameManager;
    public Transform kitchenTrans;
    public Transform dishPlaceHolder;
    public GameObject[] dishPrefabList;
    public Seeker seeker;
    public float speed;

    private string[] _menu = new string[] { "PhoBo", "PhoGa"};

    private OwnerState _state;
    private OwnerState _nextState;
    private Transform _trans;

    private Vector3 _target;
    private int _nextCustomer;
    private OrderInfo _cookOrder;
    private GameObject _dishObject;

    private Path _path;
    private int _currentWaypoint;

    private void Start()
    {
        _state = OwnerState.Idle;
        _trans = transform;
    }

    private void Update()
    {
        if (gameManager.CustomerQueue.Count > 0 && _state == OwnerState.Idle)
        {
            _nextCustomer = gameManager.CustomerQueue.Dequeue();
            var nextCustomerPos = gameManager.standList[_nextCustomer].position;
            _target = nextCustomerPos;
            _state = OwnerState.Moving;
            _nextState = OwnerState.Order;
            seeker.StartPath(transform.position, _target, OnPathComputeComplete);
        }
        else if (gameManager.OrderQueue.Count > 0 && _state == OwnerState.Idle)
        {
            _state = OwnerState.Moving;
            _nextState = OwnerState.Cooking;
            _target = kitchenTrans.position;
            seeker.StartPath(transform.position, _target, OnPathComputeComplete);
        }
    }

    public void OnPathComputeComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
            GoToNextWaypoint();
        }
    }

    public void GoToNextWaypoint()
    {
        if (_path == null)
            return;

        if (_currentWaypoint < _path.vectorPath.Count)
        {
            float distance = Vector3.Distance(_trans.position, _path.vectorPath[_currentWaypoint]);
            transform.DOMove(_path.vectorPath[_currentWaypoint], distance / speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                _currentWaypoint += 1;
                GoToNextWaypoint();
            });
        }
        else
        {
            OnReachEndOfPath();
        }
    }

    public void OnReachEndOfPath()
    {
        if (_state != OwnerState.Moving)
            return;

        _state = _nextState;
        if (_state == OwnerState.Order)
        {
            Debug.Log("Order");
            var nextOrder = new OrderInfo();
            nextOrder.slotIndex = _nextCustomer;
            nextOrder.dishIndex = Random.Range(0, _menu.Length);
            gameManager.OrderQueue.Enqueue(nextOrder);
            DOVirtual.DelayedCall(0.1f, () =>
            {
                _state = OwnerState.Idle;
            });
        }
        else if (_state == OwnerState.Cooking)
        {
            _cookOrder = gameManager.OrderQueue.Dequeue();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                var cookForCustomerPos = gameManager.standList[_cookOrder.slotIndex].position;
                var cookDishPrefab = dishPrefabList[_cookOrder.dishIndex];
                _dishObject = Instantiate(cookDishPrefab, dishPlaceHolder);
                _state = OwnerState.Moving;
                _nextState = OwnerState.Serving;
                _target = cookForCustomerPos;
                seeker.StartPath(transform.position, _target, OnPathComputeComplete);
            });
        }
        else if (_state == OwnerState.Serving)
        {
            Debug.Log("serving");
            _dishObject.transform.SetParent(gameManager.dishPlaceHolderList[_cookOrder.slotIndex], false);
            gameManager.GetCustomer(_cookOrder.slotIndex)?.ChangeState(CustomerState.Eating);
            DOVirtual.DelayedCall(0.1f, () =>
            {
                _state = OwnerState.Idle;
            });
        }
    }
}

public enum OwnerState { Idle, Moving, Order, Cooking, Serving}
