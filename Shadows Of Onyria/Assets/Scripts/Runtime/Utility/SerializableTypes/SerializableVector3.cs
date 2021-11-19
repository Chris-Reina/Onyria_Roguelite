using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class for a Vector3.
/// </summary>
[Serializable]
public struct SerializableVector3 : IEquatable<SerializableVector3>
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public bool Equals(SerializableVector3 other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
    }

    public override bool Equals(object obj)
    {
        return obj is SerializableVector3 other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ z.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(SerializableVector3 left, SerializableVector3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SerializableVector3 left, SerializableVector3 right)
    {
        return !left.Equals(right);
    }
}