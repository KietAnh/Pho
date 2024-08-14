using DG.Tweening;
using Pathfinding;
using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    public GameManager gameManager { get; set; }
    public Seeker seeker;
    public float speed;
    public TextMeshProUGUI textState;

    private CustomerState _state;
    private int _slotIndex;
    private Transform _slotTrans;
    private int _currentWaypoint;
    private Path _path;
    private void Start()
    {
        ChangeState(CustomerState.Comming);
        gameManager = GameObject.FindObjectOfType<GameManager>();

        //Transform target = gameManager.slotPivotList[_slotIndex];
        Transform target = gameManager.GetSlotPivot(_slotIndex);
        seeker.StartPath(transform.position, target.position, OnPathComputeComplete);

        _slotTrans = gameManager.GetSlot(_slotIndex);
    }
    public void SetSlot(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    private void Update()
    {

    }

    public void OnPathComputeComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
            if (_state == CustomerState.Leaving)
            {
                float distance = Vector3.Distance(transform.position, _slotTrans.position);
                transform.DOMove(_slotTrans.position, distance / speed).OnComplete(() =>
                {
                    GoToNextWaypoint();
                });
            }
            else
            {
                GoToNextWaypoint();
            }
        }
    }

    public void GoToNextWaypoint()
    {
        if (_path == null)
            return;

        if (_currentWaypoint < _path.vectorPath.Count)
        {
            float distance = Vector3.Distance(transform.position, _path.vectorPath[_currentWaypoint]);
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
        if (_state == CustomerState.Comming)
        {
            float distance = Vector3.Distance(transform.position, _slotTrans.position);
            transform.DOMove(_slotTrans.position, distance / speed).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameManager.CustomerQueue.Enqueue(_slotIndex);
                ChangeState(CustomerState.Waiting);
            });
        }
        else if (_state == CustomerState.Leaving)
        {
            Destroy(gameObject);
        }
    }

    

    public void ChangeState(CustomerState state)
    {
        _state = state;
        textState.text = _state.ToString();
        switch (_state)
        {
            case CustomerState.Comming:
            case CustomerState.Leaving:
                textState.transform.parent.gameObject.SetActive(false);
                break;
            case CustomerState.Waiting:
                textState.transform.parent.gameObject.SetActive(true);
                break;
            case CustomerState.Eating:
                textState.transform.parent.gameObject.SetActive(true);
                DOVirtual.DelayedCall(3f, () =>
                {
                    GameController.EarnMoney(10);
                    ChangeState(CustomerState.Leaving);
                    Vector3 currentPosition = transform.position;
                    gameManager.RemoveCustomer(_slotIndex);
                    gameManager.RemoveDish(_slotIndex);

                    seeker.StartPath(_slotTrans.position, gameManager.customerSpawner.position, OnPathComputeComplete);

                });
                break;
        }
    }
}

public enum CustomerState { Comming, Waiting, Eating, Leaving}
