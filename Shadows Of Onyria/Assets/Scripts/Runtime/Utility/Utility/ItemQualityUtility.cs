using System;
using DoaT;
using UnityEngine;

public class ItemQualityUtility : Manager<ItemQualityConfig>
{ 
    public Color GetColor(ItemQuality quality)
    {
        switch (quality)
        {
            case ItemQuality.Trash:
                return _config.trashColor;
            case ItemQuality.Common:
                return _config.commonColor;
            case ItemQuality.Uncommon:
                return _config.uncommonColor;
            case ItemQuality.Rare:
                return _config.rareColor;
            case ItemQuality.Epic:
                return _config.epicColor;
            case ItemQuality.Legendary:
                return _config.legendaryColor;
            default:
                throw new ArgumentOutOfRangeException(nameof(quality), quality, null);
        }
    }
}