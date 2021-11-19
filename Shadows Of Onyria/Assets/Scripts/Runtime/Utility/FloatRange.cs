using System;

namespace DoaT
{
    [Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;
        public float relativeMin;
        public float relativeMax;


        public static FloatRange operator +(FloatRange f1, FloatRange f2)
        {
            var temp = new FloatRange(f1);
            temp.min += f2.min;
            temp.max += f2.max;
            temp.relativeMin += f2.relativeMin;
            temp.relativeMax += f2.relativeMax;
            return temp;
        }

        public static FloatRange operator +(FloatRange f1, float f2)
        {
            var temp = new FloatRange(f1);
            temp.min += f2;
            temp.max += f2;
            temp.relativeMin += f2;
            temp.relativeMax += f2;
            return temp;
        }

        public static FloatRange operator -(FloatRange f1, float f2)
        {
            var temp = new FloatRange(f1);
            temp.min -= f2;
            temp.max -= f2;
            temp.relativeMin -= f2;
            temp.relativeMax -= f2;
            return temp;
        }

        public static FloatRange operator *(FloatRange f1, float f2)
        {
            var temp = new FloatRange(f1);
            temp.min *= f2;
            temp.max *= f2;
            temp.relativeMin *= f2;
            temp.relativeMax *= f2;
            return temp;
        }

        public static FloatRange operator /(FloatRange f1, float f2)
        {
            var temp = new FloatRange(f1);
            temp.min /= f2;
            temp.max /= f2;
            temp.relativeMin /= f2;
            temp.relativeMax /= f2;
            return temp;
        }

        public FloatRange(FloatRange copy)
        {
            min = copy.min;
            max = copy.max;
            relativeMin = copy.relativeMin;
            relativeMax = copy.relativeMax;
        }

        public FloatRange(float value = 0.5f)
        {
            min = max = value;
            relativeMin = value - value / 2;
            relativeMax = value + value * 2;
        }

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
            relativeMin = min;
            relativeMax = max;
        }

        public FloatRange(float min, float max, float relativeMin, float relativeMax)
        {
            this.min = min;
            this.max = max;
            this.relativeMin = relativeMin;
            this.relativeMax = relativeMax;
        }

        /// <summary>
        /// Generates a random float between the parameters of the range. min [inclusive] and max [inclusive].
        /// </summary>
        /// <returns></returns>
        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }

        public void SetRange(float minimum, float maximum)
        {
            min = minimum;
            max = maximum;
        }

        public void Multiply(float f)
        {
            min *= f;
            max *= f;
            relativeMin *= f;
            relativeMax *= f;
        }

        public void Divide(float f)
        {
            if (f == 0) return;

            min /= f;
            max /= f;
            relativeMin /= f;
            relativeMax /= f;
        }

    }

}