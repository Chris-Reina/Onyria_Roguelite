using System;
using UnityEngine;

/// <summary>
/// Serializable class for a Quaternion.
/// </summary>
[Serializable]
public struct SerializableQuaternion : IEquatable<SerializableQuaternion>
{
    public float w;
    public float x;
    public float y;
    public float z;

    public SerializableQuaternion(Quaternion quaternion)
    {
        w = quaternion.w;
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
    
    public bool Equals(SerializableQuaternion other)
    {
        return w.Equals(other.w) && x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
    }

    public override bool Equals(object obj)
    {
        return obj is SerializableQuaternion other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = w.GetHashCode();
            hashCode = (hashCode * 397) ^ x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ z.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(SerializableQuaternion left, SerializableQuaternion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SerializableQuaternion left, SerializableQuaternion right)
    {
        return !left.Equals(right);
    }
}
