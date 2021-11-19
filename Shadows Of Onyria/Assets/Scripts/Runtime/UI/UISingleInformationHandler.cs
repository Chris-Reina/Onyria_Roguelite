using TMPro;
using UnityEngine;

public class UISingleInformationHandler : UIInformationHandler
{
    [SerializeField] private TextMeshProUGUI _tag;
    [SerializeField] private TextMeshProUGUI _currentValue;
   
    public override void Setup(string attName, int currentValue)
    {
        _tag.text = attName;
        _currentValue.text = currentValue.ToString();
    }
    
    public override void SetValue(int currentValue) => _currentValue.text = currentValue.ToString();
    public override int GetValue() => int.Parse(_currentValue.text);
}