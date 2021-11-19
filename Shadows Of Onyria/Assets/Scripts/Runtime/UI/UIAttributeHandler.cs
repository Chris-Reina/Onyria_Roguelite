using System;
using TMPro;
using UnityEngine;

public class UIAttributeHandler : MonoBehaviour
{
   public string separator = "=>";
   
   [SerializeField] private UIAttributeButton _addButton;
   [SerializeField] private UIAttributeButton _removeButton;
   [SerializeField] private TextMeshProUGUI _tag;
   [SerializeField] private TextMeshProUGUI _currentValue;
   [SerializeField] private TextMeshProUGUI _separator;
   [SerializeField] private TextMeshProUGUI _possibleValue;

   public event Action OnAdd;
   public event Action OnRemove;

   private void Awake()
   {
      _addButton.OnButtonClicked += OnAddEvent;
      _removeButton.OnButtonClicked += OnRemoveEvent;
   }

   public void Setup(string attName, int currentValue, int possibleValue)
   {
      _tag.text = attName;
      _currentValue.text = currentValue.ToString();
      _possibleValue.text = possibleValue.ToString();

   }
   public void SetValues(int currentValue, int possibleValue)
   {
      _currentValue.text = currentValue.ToString();
      _possibleValue.text = possibleValue.ToString();
   }

   private void OnAddEvent()
   {
      OnAdd?.Invoke();
   }

   private void OnRemoveEvent()
   {
      OnRemove?.Invoke();
   }

   public int GetCurrentValue()
   {
      return int.Parse(_currentValue.text);
   }
   public int GetPossibleValue()
   {
      return int.Parse(_possibleValue.text);
   }
}