using UnityEngine;

namespace DoaT.UI
{
    public class UIButtonSFX : MonoBehaviour
    {
        [SerializeField] private UIButton _target;

        [SerializeField] private AudioCue _selectionCue;
        [SerializeField] private AudioCue _confirmationCue;
        [SerializeField] private AudioCue _failureCue;

        private void Awake()
        {
            if (_target != null) return;
            
            _target = GetComponent<UIButton>();

            if (_target == null)
            {
                DebugManager.LogError("No target found.");
            }
        }

        private void Start()
        {
            _target.OnSelectionStateChange += OnSelection;
            _target.OnInteraction += OnInteraction;
        }

        private void OnSelection(bool selected)
        {
            if (!selected || _selectionCue == null) return;

            AudioSystem.PlayCue(_selectionCue);
        }

        private void OnInteraction(bool interactable)
        {
        
            if (interactable && _confirmationCue != null)
                AudioSystem.PlayCue(_confirmationCue);
            else if(!interactable && _failureCue != null)
                AudioSystem.PlayCue(_failureCue);
        }
    }
}
