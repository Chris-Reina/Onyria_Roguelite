using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DoaT
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class UITooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerField;
        [SerializeField] private TextMeshProUGUI _contentField;
        [SerializeField] private TextMeshProUGUI _footerField;
        
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private LayoutElement _footerLayoutElement;
        [SerializeField] private RectTransform _rectTransform;

        private const int CHARACTER_WRAP_LIMIT = 50;

        public void SetText(Tuple<string,int> content, Tuple<string,int> header, Tuple<string,int> footer)
        {
            var (headerString, headerLength) = header;
            var (footerString, footerLength) = footer;
            var (contentString, contentLength) = content;

            if (headerLength == 0)
            {
                _headerField.gameObject.SetActive(false);
            }
            else
            {
                _headerField.gameObject.SetActive(true);
                _headerField.text = headerString;
            }
            
            if (footerLength == 0)
            {
                _footerField.gameObject.SetActive(false);
            }
            else
            {
                _footerField.gameObject.SetActive(true);
                _footerField.text = footerString;
            }

            _contentField.text = contentString;

            var anyLengthOverMaximum = headerLength > CHARACTER_WRAP_LIMIT
                                      || contentLength > CHARACTER_WRAP_LIMIT
                                      || footerLength > CHARACTER_WRAP_LIMIT;

            _layoutElement.enabled = anyLengthOverMaximum;

            var lengthFooterElement = _layoutElement.preferredWidth;
            if (!anyLengthOverMaximum)
            {
                lengthFooterElement = Mathf.Max(_rectTransform.rect.width - 1f);
            }

            _footerLayoutElement.preferredWidth = lengthFooterElement;
            
            UpdatePosition();
        }
        public void SetText(string content, string header, string footer)
        {
            if (string.IsNullOrEmpty(header))
            {
                _headerField.gameObject.SetActive(false);
            }
            else
            {
                _headerField.gameObject.SetActive(true);
                _headerField.text = header;
            }
            
            if (string.IsNullOrEmpty(footer))
            {
                _footerField.gameObject.SetActive(false);
            }
            else
            {
                _footerField.gameObject.SetActive(true);
                _footerField.text = footer;
            }

            _contentField.text = content;
            
            var headerLength = _headerField.text.Length;
            var contentLength = _contentField.text.Length;
            var footerLength = _footerField.text.Length;

            _layoutElement.enabled = headerLength > CHARACTER_WRAP_LIMIT
                                  || contentLength > CHARACTER_WRAP_LIMIT
                                  || footerLength > CHARACTER_WRAP_LIMIT;

            // _footerLayoutElement.preferredWidth =
            //     MathUtility.CeiledMax(new[] {headerLength, contentLength}, CHARACTER_WRAP_LIMIT);
            
            UpdatePosition();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying) return;

            var headerLength = _headerField.text.Length;
            var contentLength = _contentField.text.Length;
            var footerLength = _footerField.text.Length;

            var anyLengthOverMaximum = headerLength > CHARACTER_WRAP_LIMIT
                                       || contentLength > CHARACTER_WRAP_LIMIT
                                       || footerLength > CHARACTER_WRAP_LIMIT;

            _layoutElement.enabled = anyLengthOverMaximum;

            var lengthFooterElement = _layoutElement.preferredWidth;
            if (!anyLengthOverMaximum)
            {
                lengthFooterElement = Mathf.Max(_rectTransform.rect.width - 1f);
            }

            _footerLayoutElement.preferredWidth = lengthFooterElement;

        }
#endif

        private void LateUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            var position = Input.mousePosition;
            var rect = _rectTransform.rect;

            var sizeInPivotCoordsX = rect.width / Screen.width;
            var sizeInPivotCoordsY = rect.height / Screen.width;
            var pivotX = position.x / Screen.width;
            var pivotY = position.y / Screen.height;

            _rectTransform.pivot = new Vector2
            {
                x = sizeInPivotCoordsX > 1 - pivotX ? 1 : 0, y = sizeInPivotCoordsY > 1 - pivotY ? 1 : 0
            };
            transform.position = position;
        }
    }

}