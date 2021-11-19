using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class UISoulPanelManager : MonoBehaviour
{
    public UISoul prefab;
    public SoulInventoryData inventoryAsset;

    private IGroupController _parent;
    private readonly HashSet<UISoul> _buckets = new HashSet<UISoul>();

    private void Awake()
    {
        _parent = GetComponentInParent<IGroupController>();
        _parent.OnUpdateUI += RedrawUI;
        _parent.OnHideUI += ReleaseUIAssets;
        _parent.OnShowUI += DrawUI;
    }

    private void ReleaseUIAssets() //TODO: Implement Pool
    {
        //DebugManager.LogWarning("RELEASE");
        
        foreach (var bucket in _buckets)
        {
            Destroy(bucket.gameObject);
        }
        
        _buckets.Clear();
    }

    private void DrawUI() //TODO: Implement Pool
    {
        //DebugManager.LogWarning("Draw");
        
        foreach (var soul in inventoryAsset.obtainedSouls)
        {
            var temp = Instantiate(prefab, transform);
            _buckets.Add(temp);
            temp.Setup(soul);
        }
    }
    
    private void RedrawUI() 
    {
        //DebugManager.LogWarning("RedrawUI");
        ReleaseUIAssets();
        DrawUI();
    }
}
