using DoaT;
using TMPro;
using UnityEngine;

public class UIZoneMarker : MonoBehaviour
{
    public TextMeshProUGUI zoneNameField;
    
    void Start()
    {
        if (SceneContext.IsRunMap)
        {
            zoneNameField.text = PersistentData.RunGenerationManager.GetZoneName();
        }
        else
        {
            if (World.Current == null)
            {
                Destroy(gameObject);
                return;
            }

            zoneNameField.text = World.ZoneName;
        }
    }
}
