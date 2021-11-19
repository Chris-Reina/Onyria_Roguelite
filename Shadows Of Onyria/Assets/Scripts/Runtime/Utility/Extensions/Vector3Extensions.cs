using System;
using UnityEngine;

public static class Vector3Extensions
{
   public static Vector3 Clone(this Vector3 old)
   {
      return new Vector3(old.x, old.y, old.z);
   }
   
   public static Vector3 SetValues(this Vector3 thisVector, Vector3 reference)
   {
      thisVector.x = reference.x;
      thisVector.y = reference.y;
      thisVector.z = reference.z;
      return thisVector;
   }
   public static Vector3 SetX(this Vector3 thisVector, float newX)
   {
      thisVector.x = newX;
      return thisVector;
   }
   public static Vector3 SetY(this Vector3 thisVector, float newY)
   {
      thisVector.y = newY;
      return thisVector;
   }
   public static Vector3 SetZ(this Vector3 thisVector, float newZ)
   {
      thisVector.z = newZ;
      return thisVector;
   }

   public static float ProjectedDistance(this Vector3 thisVector, Vector3 comparedVector)
   {
      thisVector.SetY(0);
      comparedVector.SetY(0);
      return Vector3.Distance(thisVector, comparedVector);
   }

   public static SerializableVector3 ToSerializable(this Vector3 thisVector)
   {
      return new SerializableVector3(thisVector.x, thisVector.y, thisVector.z);
   }

   public static float GetMean(this Vector3 thisVector)
   {
      var accum = thisVector.x + thisVector.y + thisVector.z;
      return accum / 3;
   }

   public static Vector4 ToVector4(this Vector3 thisVector)
   {
      return new Vector4(thisVector.x, thisVector.y, thisVector.z);
   }

   public static bool Any(this Vector3 thisVector, Func<float, bool> condition)
   {
      return condition(thisVector.x) || condition(thisVector.y) || condition(thisVector.z);
   }
}
