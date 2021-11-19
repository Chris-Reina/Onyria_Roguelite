using UnityEngine;

namespace DoaT.UI
{
    public class UISelectionBackground : MonoBehaviour
    {
        [SerializeField] private UIButton _target;
        [SerializeField] private GameObject SelectionBackground;

        private void Awake()
        {
            if (_target == null)
                _target = GetComponent<UIButton>();

            if (_target == null)
                Debug.LogWarning("A UI feedback doesn't have a target to attach to.");
        }

        private void Start()
        {
            _target.OnSelectionStateChange += ManageBackground;
        }

        private void ManageBackground(bool state)
        {
            SelectionBackground.SetActive(state);
        }
    }
}
