using UnityEngine;

namespace DoaT
{
    public static class AudioUtility
    {
        public static float DecibelsToLinear(float db)
        {
            return Mathf.Clamp(Mathf.Pow(10, db / 20), -80f, 0f);
        }

        public static float LinearToDecibels(float linear)
        {
            return Mathf.Clamp(20 * Mathf.Log10(linear), -80f, 0f);
        }
    }
}