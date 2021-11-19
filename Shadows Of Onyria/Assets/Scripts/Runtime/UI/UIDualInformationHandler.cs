using TMPro;
using UnityEngine;

public class UIDualInformationHandler : UIInformationHandler
{
    [SerializeField] private TextMeshProUGUI _tag;
    [SerializeField] private TextMeshProUGUI _currentValue;
    [SerializeField] private TextMeshProUGUI _possibleValue;
   
    public override void Setup(string attName, int currentValue)
    {
        _tag.text = attName;
        _currentValue.text = currentValue.ToString();
        _possibleValue.text = currentValue.ToString();
    }

    public override int GetValue() => int.Parse(_currentValue.text);
    public int GetPossibleValue() => int.Parse(_possibleValue.text);

    public override void SetValue(int currentValue) => _currentValue.text = currentValue.ToString();
    public void SetPossibleValue(int possibleValue) => _possibleValue.text = possibleValue.ToString();
    
    
}