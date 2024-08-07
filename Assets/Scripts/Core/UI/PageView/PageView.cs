using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Reflection;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class PageView : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] protected RectTransform _content;
        [SerializeField] protected GameObject _itemTemplate;
        [SerializeField] protected GameObject _gridTemplate;
        [SerializeField] protected PageMark _pageMark;
        [SerializeField] protected Button _btnPrev;
        [SerializeField] protected Button _btnNext;
        [SerializeField] protected int maxItemsPerPage = 9;
        [SerializeField] protected float percentThreshold = 0.25f;
        [SerializeField] protected float easing = 0.5f;
        [SerializeField] protected bool canDrag = true;
        [SerializeField] protected string _itemTypeStr;     // class name of page item
        public int totalPages { get; set; }
        public int totalItems { get; set; }
        public int currentPage { get; set; }

        private int _pageWidth;
        private Vector2 _originPosition;
        protected Vector2 position;

        protected List<PageItem> itemList;
        protected List<GameObject> gridPool;

        public Action<PageItem, int> ItemShowHandler;
        public Action<PageItem, int> ItemHideHandler;

        public bool isInitialized = false;

        public Transform firstItem
        {
            get
            {
                return itemList[0].trans;
            }
        }

        void Awake()
        {
            //pageMark = transform.Find("page_mark").GetComponent<PageMark>();
            //content = transform.Find("view_port/content").GetComponent<RectTransform>();
            //itemTemplate = transform.Find("view_port/item").gameObject;
            //gridTemplate = transform.Find("view_port/page").gameObject;
            _pageWidth = (int)GetComponent<RectTransform>().sizeDelta.x;
            _originPosition = _content.anchoredPosition;

            if (_btnPrev != null)
            {
                _btnPrev.onClick.AddListener(OnClickPrev);
            }
            if (_btnNext != null)
            {
                _btnNext.onClick.AddListener(OnClickNext);
            }
        }

        void OnEnable()
        {
            if (isInitialized)
            {
                InitView();
            }
        }

        public virtual void Init(int nItems, int maxItemsPerPage, int curPage = 0, bool fillPage = false)
        {
            isInitialized = true;
            position = _originPosition;
            _content.anchoredPosition = position;
            if (nItems > 0)
            {
                this.maxItemsPerPage = maxItemsPerPage;
                totalPages = (nItems - 1) / maxItemsPerPage + 1;
                if (fillPage)
                {
                    totalItems = totalPages * maxItemsPerPage;
                }
                else
                {
                    totalItems = nItems;
                }
                currentPage = curPage;
                if (_pageMark != null)
                {
                    _pageMark.Init(totalPages, currentPage);
                }
            }
            else
            {
                Debug.LogError("KIET LOG >> PageView Init >> Number of items = 0");
            }

            SpawnItems();
            InitView();
        }
        private void SpawnItems()
        {
            itemList = new List<PageItem>();
            gridPool = new List<GameObject>();
            for (int i = 0; i < totalPages; i++)
            {
                GameObject gridObject = Instantiate(_gridTemplate, _content.transform);
                gridObject.SetActive(true);
                var grid = gridObject.transform.Find("grid").gameObject;
                if (i == currentPage) 
                    grid.SetActive(true);
                gridPool.Add(grid);
                int nItemsCurPage = i == totalPages - 1 ? totalItems % maxItemsPerPage : maxItemsPerPage;
                if (nItemsCurPage == 0)
                    nItemsCurPage = maxItemsPerPage;
                for (int j = 0; j < nItemsCurPage; j++)
                {
                    var itemObject = Instantiate(_itemTemplate, grid.transform);
                    itemObject.SetActive(true);
                    Type itemType = Assembly.GetExecutingAssembly().GetType(_itemTypeStr);
                    var item = Activator.CreateInstance(itemType) as PageItem;
                    item.trans = itemObject.transform;
                    item.Init();
                    itemList.Add(item);
                }
            }
        }
        public void InitView()
        {
            RefreshView(currentPage);
            CheckShowArrowButton();
        }
        public void RefreshView(int index = 0)
        {
            for (int i = index*maxItemsPerPage; i < (index + 1)*maxItemsPerPage && i < itemList.Count; i++)
            {
                ItemShowHandler(itemList[i], i);
            }
        }
        public void HideView()
        {
            OnLeaveOldPage(currentPage);
        }
        private void OnLeaveOldPage(int index)
        {
            for (int i = index * maxItemsPerPage; i < (index + 1) * maxItemsPerPage && i < itemList.Count; i++)
            {
                ItemHideHandler(itemList[i], i);
            }
        }
        private void OnClickPrev()
        {
            PrevPage();
            gridPool[currentPage + 1].SetActive(false);
            gridPool[currentPage].SetActive(true);
            RefreshView(currentPage);
        }
        private void OnClickNext()
        {
            NextPage();
            gridPool[currentPage - 1].SetActive(false);
            gridPool[currentPage].SetActive(true);
            RefreshView(currentPage);
        }
        public void PrevPage()
        {
            currentPage -= 1;
            if (_pageMark != null)
                _pageMark.PrevPage();
            CheckShowArrowButton();
            OnLeaveOldPage(currentPage + 1);
        }
        public void NextPage()
        {
            currentPage += 1;
            if (_pageMark != null)
                _pageMark.NextPage();
            CheckShowArrowButton();
            OnLeaveOldPage(currentPage - 1);
        }
        public void CheckShowArrowButton()
        {
            if (_btnNext != null)
            {
                if (currentPage == totalPages - 1)
                {
                    _btnNext.gameObject.SetActive(false);
                }
                else
                {
                    _btnNext.gameObject.SetActive(true);
                }
            }
            if (_btnPrev != null)
            {
                if (currentPage == 0)
                {
                    _btnPrev.gameObject.SetActive(false);
                }
                else
                {
                    _btnPrev.gameObject.SetActive(true);
                }
            }
        }
        public void ClearItems()
        {
            for (int i = 0; i < _content.transform.childCount; i++)
            {
                Destroy(_content.transform.GetChild(i).gameObject);
            }
            if (gridPool != null)
                gridPool.Clear();
            if (itemList != null)
                itemList.Clear();
            if (_pageMark != null)
                _pageMark.ClearNodes();
        }
        private void OnDisable()
        {
            HideView();
        }

        private void OnDestroy()
        {
            ClearItems();

            if (_btnPrev != null)
            {
                _btnPrev.onClick.RemoveListener(PrevPage);
            }
            if (_btnNext != null)
            {
                _btnNext.onClick.RemoveListener(NextPage);
            }
        }

        #region Handle input
        public void OnDrag(PointerEventData data)
        {
            if (!canDrag) return;
            float difference = data.pressPosition.x - data.position.x;
            _content.anchoredPosition = position - new Vector2(difference, 0);
            if (difference < 0)
            {
                if (currentPage > 0)
                {
                    if (!gridPool[currentPage - 1].activeSelf)
                    {
                        gridPool[currentPage - 1].SetActive(true);
                        RefreshView(currentPage - 1);
                    }
                }
            }
            else if (difference > 0)
            {
                if (currentPage < totalPages - 1)
                {
                    if (!gridPool[currentPage + 1].activeSelf)
                    {
                        gridPool[currentPage + 1].SetActive(true);
                        RefreshView(currentPage + 1);
                    }
                }
            }
        }
        public void OnEndDrag(PointerEventData data)
        {
            if (!canDrag) return;
            float percentage = (data.pressPosition.x - data.position.x) / _pageWidth;
            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector3 newPosition = position;
                if (percentage > 0 && currentPage < totalPages - 1)
                {
                    newPosition += new Vector3(-_pageWidth, 0, 0);
                    NextPage();
                }
                else if (percentage < 0 && currentPage > 0)
                {
                    newPosition += new Vector3(_pageWidth, 0, 0);
                    PrevPage();
                }
                StartCoroutine(SmoothMove(_content.anchoredPosition, newPosition, easing));
                position = newPosition;
            }
            else
            {
                StartCoroutine(SmoothMove(_content.anchoredPosition, position, easing));
            }
        }
        IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
        {
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                _content.anchoredPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            if (endpos.x > startpos.x && currentPage < totalPages - 1)
            {
                gridPool[currentPage + 1].SetActive(false);
                
            }
            else if (currentPage > 0)
            {
                gridPool[currentPage - 1].SetActive(false);
            }
            gridPool[currentPage].SetActive(true);
        }
        #endregion

    }
}

