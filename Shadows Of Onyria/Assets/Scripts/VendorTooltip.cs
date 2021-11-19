using System;
using System.Collections;
using DoaT.Vendor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoaT
{
    public class VendorTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Tuple<string, int> _header; 
        private Tuple<string, int> _content;
        private Tuple<string, int> _price;

        [SerializeField] private float showDelay = 0.05f;

        private bool _coroutineActive = false;
        private Coroutine _currentCoroutine;

        public void SetTooltip(Tuple<string,int> content, 
                               Tuple<string,int> header = null, 
                               Tuple<string,int> price = null)
        {
            _header = header;
            _content = content;
            _price = price;
        }
        
        public void SetTooltip(Item item)
        {
            var auxPrice = item.buyAmount.ToString();
            
            _header = new Tuple<string, int>(item.header, item.header.Length);
            _content = item.GetContent();
            _price = new Tuple<string, int>(auxPrice,auxPrice.Length);
        }

        private IEnumerator DelayAndShow()
        {
            _coroutineActive = true;
            yield return new WaitForSeconds(showDelay);

            TooltipSystem.Show(_content, _header, _price);
            _currentCoroutine = null;
            _coroutineActive = false;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_coroutineActive) return;
            
            _currentCoroutine = StartCoroutine(DelayAndShow());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_coroutineActive)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
                _coroutineActive = false;
                return;
            }
            TooltipSystem.Hide();
        }
    }
}