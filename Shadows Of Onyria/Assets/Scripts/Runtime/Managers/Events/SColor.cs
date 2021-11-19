using UnityEngine;

namespace DoaT
{
    [System.Serializable]
    public readonly struct SColor
    {
        private const byte MAX_BYTE_FOR_OVEREXPOSED_COLOR = 191;

        public readonly float r;
        public readonly float g;
        public readonly float b;
        public readonly float a;
        public readonly float intensity;

        public bool IsDefault => r == 0f && g == 0f && b == 0f && a == 0f && intensity == 0f;
        
        public SColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
            var scaleFactor = MAX_BYTE_FOR_OVEREXPOSED_COLOR / color.maxColorComponent;
            intensity = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);
        }
        public SColor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
            intensity = 1f;
        }
        public SColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            intensity = 1f;
        }
        public SColor(float r, float g, float b, float a, float intensity)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.intensity = intensity;
        }

        public Color ToColor(bool incorporateIntensity = true)
        {
            var factor = incorporateIntensity ? Mathf.Pow(2, intensity) : Mathf.Pow(0.5f, intensity);
            
            return new Color(r * factor, g * factor, b * factor, a);
        }

        public Vector4 ToVector4(bool incorporateIntensity = true)
        {
            var factor = incorporateIntensity ? Mathf.Pow(2, intensity) : Mathf.Pow(0.5f, intensity);

            return new Vector4(r * factor, g * factor, b * factor, a);
        }
    }
}