using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class RigidbodyAnimationEndPosition : MonoBehaviour
{
    public PhysicsAnimationClusterData dataHolder;
    public List<Transform> transforms;
    private readonly Dictionary<string,PositionalInformation> _positions = new Dictionary<string, PositionalInformation>();
    
    public void GetTransforms()
    {
        transforms = GetComponentsInChildren<Transform>().ToList();
        transforms.Remove(transform);
    }

    public void Set()
    {
        _positions.Clear();
        
        foreach (var info in dataHolder.information)
        {
            var s = info.recordedObject;
            var p = info.spatialInformation[info.spatialInformation.Count - 1];
            _positions.Add(s, p);
        }

        foreach (var t in transforms.Where(t => _positions.ContainsKey(t.name)))
        {
            t.position = _positions[t.name].position.ToVector3();
            t.rotation = _positions[t.name].rotation.ToQuaternion();
        }
    }
}
