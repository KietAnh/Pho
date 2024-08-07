using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageMark : MonoBehaviour
{
    [SerializeField] private GameObject nodeTemplate;
    private List<GameObject> nodeActiveList;
    private int cur = 1;
    private int max = 1;
    public void Init(int number, int curPage)
    {
        nodeActiveList = new List<GameObject>();
        for (int i = 0; i < number; i++)
        {
            var nodeObject = Instantiate(nodeTemplate, transform);
            nodeObject.SetActive(true);
            nodeActiveList.Add(nodeObject.transform.Find("selected").gameObject);
        }
        max = number;
        cur = curPage;
        if (cur > 0 && cur <= max)
        {
            nodeActiveList[cur - 1].SetActive(true);
        }
    }
    public void NextPage()
    {
        if (cur < max)
        {
            nodeActiveList[cur - 1].SetActive(false);
            cur += 1;
            nodeActiveList[cur - 1].SetActive(true);
        }
    }
    public void PrevPage()
    {
        if (cur > 1)
        {
            nodeActiveList[cur - 1].SetActive(false);
            cur -= 1;
            nodeActiveList[cur - 1].SetActive(true);
        }
    }
    public void ClearNodes()
    {
        if (nodeActiveList == null)
            return;
        foreach (var node in nodeActiveList)
        {
            Destroy(node.transform.parent.gameObject);
        }
        nodeActiveList.Clear();
    }
}
