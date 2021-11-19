using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoaT
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _header; 
        [SerializeField, TextArea] private string _content;

        [SerializeField] private float showDelay = 0.1f;

        private bool _coroutineActive = false;
        private Coroutine _currentCoroutine;

        private IEnumerator DelayAndShow()
        {
            _coroutineActive = true;
            yield return new WaitForSeconds(showDelay);

            TooltipSystem.Show(new Tuple<string, int>(_content, _content.Length),
                               new Tuple<string, int>(_header, _header.Length));
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