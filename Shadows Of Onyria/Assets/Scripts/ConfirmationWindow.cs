using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT.UI
{
    //TODO: Expand to allow for different configurations of modal windows.
    public class ConfirmationWindow : MonoBehaviour
    {
        private static ConfirmationWindow Current { get; set; }

        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private Action _confirmActionCallback;

        private void Awake()
        {
            if (Current == null)
                Current = this;
            else if (Current != this)
            {
                Destroy(this);
                return;
            }
        }

        private void Start()
        {
            _confirmButton.onClick.AddListener(ConfirmationCallbackAdapter);
            _cancelButton.onClick.AddListener(Close);
        }

        private void ConfirmationCallbackAdapter() => _confirmActionCallback?.Invoke();
        private void Open()
        {
            _group.Activate();
        }
        private void Close()
        {
            _group.Deactivate();
        }

        public static void Display(string confirmationMessage, Action callback) => Current.DisplayImpl(confirmationMessage, callback);
        private void DisplayImpl(string confirmationMessage, Action callback)
        {
            _message.text = confirmationMessage;
            _confirmActionCallback = callback;

            Open();
        }
    }
}
