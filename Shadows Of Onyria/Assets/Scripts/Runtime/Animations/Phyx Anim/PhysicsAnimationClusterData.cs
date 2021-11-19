using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Animation/Physics Animation Cluster", fileName = "Physics Animation Cluster Data")]
public class PhysicsAnimationClusterData : ScriptableObject
{
    public bool record;
    public float count;
    public float doneCount;
    
    private readonly HashSet<GameObject> _objects = new HashSet<GameObject>();
    private readonly HashSet<GameObject> _doneObjects = new HashSet<GameObject>();
    private readonly Dictionary<GameObject, PhysicsAnimationData> _data = new Dictionary<GameObject, PhysicsAnimationData>();
    public List<PhysicsAnimationData> information = new List<PhysicsAnimationData>();

    public bool AddObjectToRecord(GameObject obj)
    {
        if (!record) return false;
        if (_objects.Contains(obj))return false;

        count++;
        _objects.Add(obj);
        return true;
    }
    
    public void AddFinishedObject(GameObject obj)
    {
        if (!record) return;
        if (_doneObjects.Contains(obj)) return;
        
        doneCount++;
        _doneObjects.Add(obj);
        if (_objects.Count == _doneObjects.Count)
        {
            GenerateList();
        }
    }
    
    public void AddData(GameObject obj, Vector3 position, Quaternion rotation)
    {
        if (_data.ContainsKey(obj))
        {
            _data[obj].AddData(position, rotation);
            return;
        }
        
        _data.Add(obj, new PhysicsAnimationData(obj, position, rotation));
    }

    private void GenerateList()
    {
        information = _data.Select(x => x.Value).ToList();
        _objects.Clear();
        _doneObjects.Clear();
        _data.Clear();
    }

    public void CleanupAnimations()
    {
        for (var i = 0; i < information.Count; i++)
        {
            information[i].spatialInformation.Reverse();
            information[i].spatialInformation =
                information[i].spatialInformation.SkipWhile(x => x.deltaSpeed < 0.001f).ToList();
            information[i].spatialInformation.Reverse();
        }

        information = information.OrderBy(x => x.recordedObject).ToList();
    }

    public void ClearAnimations()
    {
        _objects.Clear();
        _doneObjects.Clear();
        _data.Clear();
        information.Clear();
        count = 0;
        doneCount = 0;
    }

    public PhysicsAnimationData GetAnimationData(string gameObjectName)
    {
        // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
        return information.Where(x => x.recordedObject.Equals(gameObjectName)).FirstOrDefault();
    }
}