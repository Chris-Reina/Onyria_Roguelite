using System;
using UnityEngine;

namespace DoaT.UI
{
    public class ConfirmationWindow : MonoBehaviour
    {
        private static ConfirmationWindow Current { get; set; }

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

        public static bool Create(string confirmationMessage, Action callback) => Current.CreateImpl(confirmationMessage, callback);
        private bool CreateImpl(string confirmationMessage, Action callback)
        {
            //TODO: Implement
            return false;
        }
    }
}
