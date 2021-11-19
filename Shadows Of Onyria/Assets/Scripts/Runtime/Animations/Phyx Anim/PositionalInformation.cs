using System;

[Serializable]
public struct PositionalInformation
{
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public float deltaSpeed;

    public PositionalInformation(SerializableVector3 position, SerializableQuaternion rotation, float deltaSpeed)
    {
        this.position = position;
        this.rotation = rotation;
        this.deltaSpeed = deltaSpeed;
    }
}
