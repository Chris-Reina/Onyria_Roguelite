using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    private const string Mesh = "Mesh";
    
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    [SerializeField] private List<VisualEffect> vfxGraph;
    [SerializeField] private float refreshRate = 0.1f;

    private readonly HashSet<VisualEffect> _vfxBuckets = new HashSet<VisualEffect>();

    private void Start()
    {
        if (vfxGraph.Count == 0) return;

        foreach (var effect in vfxGraph)
        {
            AddVFXToUpdate(effect);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void AddVFXToUpdate(VisualEffect vfx)
    {
        if (_vfxBuckets.Contains(vfx)) return;
        
        if(_vfxBuckets.Count == 0)
            StartCoroutine(UpdateVfxGraph());
        
        _vfxBuckets.Add(vfx);
    }

    public void RemoveVFXFromUpdate(VisualEffect vfx)
    {
        if (!_vfxBuckets.Contains(vfx)) return;
        
        _vfxBuckets.Remove(vfx);
        
        if(_vfxBuckets.Count == 0)
            StopCoroutine(UpdateVfxGraph());
    }

    private IEnumerator UpdateVfxGraph()
    {
        while (true)
        {
            while (_vfxBuckets.Count == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            
            var m = new Mesh();
            skinnedMesh.BakeMesh(m);
            var vertices = m.vertices;
            var m2 = new Mesh {vertices = vertices};
            for (int i = 0; i < vfxGraph.Count; i++)
            {
                vfxGraph[i].SetMesh(Mesh, m2);
            }
            
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
