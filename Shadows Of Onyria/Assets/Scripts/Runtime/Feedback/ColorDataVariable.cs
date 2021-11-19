using UnityEngine;

namespace DoaT
{
    [System.Serializable, CreateAssetMenu(menuName = "Data/Data Variable/Color", fileName = "DataColor_")]
    public class ColorDataVariable : DataVariable<Color>
    {
        [SerializeField, ColorUsage(true, true)] private Color _color;

        public override Color Value => _color;
        public SColor SerializableColor => new SColor(_color);
    }
}