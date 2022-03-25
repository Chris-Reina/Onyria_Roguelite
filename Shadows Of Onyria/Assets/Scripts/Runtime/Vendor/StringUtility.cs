using System;
using UnityEngine;

namespace DoaT
{
    public static class StringUtility
    {
        public static string GetExtension(string source)
        {
            var a = source.Split('.');
            return a[a.Length - 1];
        }
        
        public static Tuple<string, int> GetDynamicString(string source, char beginAnchor, char endAnchor, string[] data)
        {
            var str = "";
            var length = -data.Length;
            
            for (int i = 0; i < data.Length; i++)
            {
                length += data[i].Length;
                str = source.Replace($"{beginAnchor}{i}{endAnchor}", $"<color=#03fc9d>{data[i]}</color>");
            }

            return new Tuple<string, int>(str, length);
        }
        
        public static Tuple<string, int> GetDynamicString(string source, char beginAnchor, char endAnchor, string[] data, Color color)
        {
            var str = "";
            var length = -data.Length;

            var hex = ColorUtility.ToHtmlStringRGB(color);
            for (int i = 0; i < data.Length; i++)
            {
                length += data[i].Length;
                str = source.Replace($"{beginAnchor}{i}{endAnchor}", $"<color=#{hex}>{data[i]}</color>");
            }
            return new Tuple<string, int>(str, length);
        }
        
        public static Tuple<string, int> GetDynamicString(string source, char beginAnchor, char endAnchor, string[] data, Color[] colors)
        {
            var str = "";
            var length = -data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                length += data[i].Length;
                str = source.Replace($"{beginAnchor}{i}{endAnchor}",
                    $"<color=#{ColorUtility.ToHtmlStringRGB(colors[i])}>{data[i]}</color>");
            }
            return new Tuple<string, int>(str, length);
        }

        public static string Paint(string source, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{source}</color>";
        }
    }
}