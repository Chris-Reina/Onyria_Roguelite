using TMPro;
using UnityEngine;

namespace DoaT.UI
{
    public class UIButtonTextDisplay : MonoBehaviour
    {
        [SerializeField] private UIButton _target;
        [SerializeField] private TextMeshProUGUI _display;

        [SerializeField] private Color _enabled;
        [SerializeField] private Color _disabled;

        private void Awake()
        {
            if (_target == null)
                _target = GetComponent<UIButton>();

            if (_target == null)
                Debug.LogWarning("A UI feedback doesn't have a target to attach to.");
        }

        private void Start()
        {
            if (_target == null) return;

            ManageDisplay(_target.interactable);
            _target.OnInteractableStateChange += ManageDisplay;
        }

        private void ManageDisplay(bool state)
        {
            _display.color = state ? _enabled : _disabled;
        }
    }
}
