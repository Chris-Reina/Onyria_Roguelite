using UnityEngine;

[CreateAssetMenu(menuName = "Data/Manager Configurations/Item Quality",fileName = "Item Quality Config")]
public class ItemQualityConfig : ScriptableObject
{
    public Color trashColor;
    public Color commonColor;
    public Color uncommonColor;
    public Color rareColor;
    public Color epicColor;
    public Color legendaryColor;
}
