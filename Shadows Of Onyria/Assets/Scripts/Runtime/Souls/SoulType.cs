using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Souls/Type", fileName = "SoulType_")]
    public class SoulType : ScriptableObject
    {
        public string identifier;
        public Sprite icon;
        [ColorUsage(true, true)] public Color soulEnergyColor;
    }

    public readonly struct SoulTypeData
    {
        public readonly string identifier;
        public readonly Sprite icon;
        public readonly Color soulEnergyColor;

        public SoulTypeData(SoulType type)
        {
            identifier = type.identifier;
            icon = type.icon;
            soulEnergyColor = type.soulEnergyColor;
        }
    }
}