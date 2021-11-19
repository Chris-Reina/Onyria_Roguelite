using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class PhysicsAnimationData
{
    public string recordedObject;
    public List<PositionalInformation> spatialInformation;

    public PhysicsAnimationData(GameObject o, Vector3 position, Quaternion rotation)
    {
        recordedObject = o.name;
        spatialInformation = new List<PositionalInformation>
            { new PositionalInformation(position.ToSerializable(), rotation.ToSerializable(), 0)};
    }
    
    public void AddData(Vector3 position, Quaternion rotation)
    {
        var deltaPosition = Vector3.Distance(spatialInformation[spatialInformation.Count - 1].position.ToVector3(), position);
        var deltaRotation = Quaternion.Angle(spatialInformation[spatialInformation.Count - 1].rotation.ToQuaternion(), rotation);
        
        spatialInformation.Add(new PositionalInformation(position.ToSerializable(), rotation.ToSerializable(), deltaPosition + deltaRotation));
    }
}