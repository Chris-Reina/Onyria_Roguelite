using System.Collections;
using TMPro;
using UnityEngine;

namespace DoaT
{
    public class MessageDisplay : MonoBehaviour, IUpdate, IUnloadable
    {
        [SerializeField] private TextMeshProUGUI display;
        [SerializeField] private float remainingTime = 0f;

        private string _message;
        private float _displayTime;
        private Gradient _colorGradient;
        private bool _finished;

        private float Progress => 1 - (remainingTime / _displayTime);

        public void Initialize(string message, float displayTime, Gradient color)
        {
            _message = message;
            _displayTime = displayTime;
            _colorGradient = color;
            ExecutionSystem.AddUpdate(this);
            Messenger.AddChildObject(this);

            display.text = _message;
            display.color = color.Evaluate(0f);
            remainingTime = _displayTime;
        }

        public void OnUpdate()
        {
            if (remainingTime <= 0 && !_finished)
            {
                _finished = true;
                StartCoroutine(SetToUnload());
                return;
            }
            
            display.color = _colorGradient.Evaluate(Progress);
            remainingTime -= Time.deltaTime;
        }

        private IEnumerator SetToUnload()
        {
            yield return new WaitForEndOfFrame();
            Unload();
        }
        
        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, true);
            Messenger.RemoveChildObject(this);
            Destroy(gameObject);
        }
    }
}