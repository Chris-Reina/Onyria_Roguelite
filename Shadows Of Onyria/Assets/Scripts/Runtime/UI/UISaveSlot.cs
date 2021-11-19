using System;
using DoaT.Save;
using TMPro;
using UnityEngine;

namespace DoaT.UI
{
    public class UISaveSlot : MonoBehaviour
    {
        [SerializeField] private int _slot;

        [SerializeField] private TextMeshProUGUI _slotNameText;
        
        [SerializeField] private CanvasGroup _dataFoundGroup;
        [SerializeField] private CanvasGroup _dataMissingGroup;

        [SerializeField] private TextMeshProUGUI _playerLevel;
        [SerializeField] private TextMeshProUGUI _saveDate;
        [SerializeField] private TextMeshProUGUI _hoursPlayed;
        [SerializeField] private TextMeshProUGUI _zone;
        
        public int Slot => _slot;

        private void Start()
        {
            FillSaveSlotData();
        }

        private void FillSaveSlotData()
        {
            _slotNameText.text = $"SLOT {_slot}";

            if (SaveSystem.GetSavedMetaData(_slot, out var metadata))
            {
                _dataFoundGroup.Activate();
                _dataMissingGroup.Deactivate();

                _playerLevel.text = metadata.CharacterLevel.ToString();

                _saveDate.text = metadata.SaveTime.ToString("MM/dd/yyyy hh:mm tt");

                var time = TimeSpan.FromSeconds(metadata.PlayedTime);
                _hoursPlayed.text = time.ToString(@"hh\:mm");

                _zone.text = metadata.GameZone;
            }
            else
            {
                _dataFoundGroup.Deactivate();
                _dataMissingGroup.Activate();
            }
        }
    }
}
