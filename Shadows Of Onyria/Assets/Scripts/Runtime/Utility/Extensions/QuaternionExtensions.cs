using UnityEngine;

public static class QuaternionExtensions
{
    public static SerializableQuaternion ToSerializable(this Quaternion thisQuaternion)
    {
        return new SerializableQuaternion(thisQuaternion);
    }
}
