using DoaT.Save;
using UnityEngine;

namespace DoaT.UI
{
    public class UIDeleteSaveFileButton : UIButton
    {
        [SerializeField] private UISaveSlot _parentSlot;

        protected override void Start()
        {
            base.Start();

            OnInteractionSucceeded += DeleteSaveFile;
        }

        private void DeleteSaveFile()
        {
            SaveSystem.DeleteSaveFile(_parentSlot.Slot);
        }
    }
}