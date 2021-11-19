using TMPro;
using UnityEngine;

namespace DoaT.Vendor
{
    public class GoldMonitor : MonoBehaviour
    {
        [SerializeField] private ItemInventoryData _data;

        private TextMeshProUGUI _textField;
        
        private void Awake()
        {
            _textField = GetComponent<TextMeshProUGUI>();
            _data.OnGoldValueChanged += OnGoldValueChanged;
            _textField.text = _data.Gold.ToString();
        }

        private void OnGoldValueChanged(int newAmount)
        {
            _textField.text = newAmount.ToString();
        }
    }
}
