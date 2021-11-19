using UnityEngine;
using UnityEngine.UI;

namespace DoaT
{
    public class UIPointer : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public CursorGameSelection selection;
        public Image cursorImage;
        public Image cursorMaskImage;

        private Vector3 _mPos;

        [SerializeField] private Sprite CursorIdle = default;
        [SerializeField] private Sprite CursorPressed = default;

        private void Awake()
        {
            Cursor.visible = false;

            if (!canvasGroup)
                canvasGroup = GetComponentInParent<CanvasGroup>();

            cursorMaskImage.gameObject.SetActive(false);
        }

        void Update()
        {
            //cursorMaskImage.gameObject.SetActive(selection.raycastResult is EntityRaycastResult);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                cursorImage.sprite = CursorPressed;
                //cursorMaskImage.sprite = CursorPressed;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                cursorImage.sprite = CursorIdle;
                //cursorMaskImage.sprite = CursorIdle;
            }

            _mPos = Input.mousePosition;
        }

        private void LateUpdate()
        {
            transform.position = _mPos;
        }
    }

}