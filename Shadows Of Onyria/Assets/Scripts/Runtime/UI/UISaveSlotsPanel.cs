using UnityEngine;

public class UISaveSlotsPanel : MonoBehaviour
{
    //TODO: Implement

    [SerializeField] private CanvasGroup _saveSlotsGroup;

    private void Awake()
    {
        if (_saveSlotsGroup == null)
        {
            _saveSlotsGroup = GetComponent<CanvasGroup>();
            
            if (_saveSlotsGroup == null)
                _saveSlotsGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    
}

public interface ISaveSlotInteraction
{
    
}

public class CreateSaveInteraction : ISaveSlotInteraction
{
    
}

public class LoadSaveInteraction : ISaveSlotInteraction
{
    
}
