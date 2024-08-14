using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : SingletonBehaviour<AreaManager>
{
    public ContactFilter2D notWalkableFilter;
    public List<Transform> areaList = new List<Transform>();

    private void Start()
    {
        GED.ED.addListener(EventID.OnUnlockArea, OnUnlockArea_CleanTree);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GED.ED.removeListener(EventID.OnUnlockArea, OnUnlockArea_CleanTree);
    }

    #region event

    private void OnUnlockArea_CleanTree(GameEvent gameEvent)
    {
        int areaUnlockId = ServiceManager.Singleton.GetService<MainService>().areaUnlockId;

        var overlapResults = new List<Collider2D>();
        areaList[areaUnlockId - 1].GetComponent<Collider2D>().OverlapCollider(notWalkableFilter, overlapResults);
        foreach (var collider in overlapResults)
        {
            var tree = collider.transform.parent;
            if (tree.CompareTag("tree"))
            {
                GameController.CleanTree(tree.GetSiblingIndex(), collider);

                Destroy(tree.gameObject);
            }
        }
    }

    #endregion
}
