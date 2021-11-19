using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace DoaT
{
    public class UISoundSlider : MonoBehaviour
    {
        public Slider slider;
        public AudioMixer mixer;
        public string exposedParameterName;

        private void Start()
        {
            //mixer.GetFloat(exposedParameterName, out var value);
            //slider.value = AudioUtility.DecibelsToLinear(value);
        }
        
        public void OnSliderValueChanged(float value)
        {
            mixer.SetFloat(exposedParameterName, AudioUtility.LinearToDecibels(value));
        }
    }
}
