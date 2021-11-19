using DoaT;
using UnityEngine;

public class FireLightFlicker : MonoBehaviour
{
    public new Light light;
    public FloatRange flickerInterval;
    public FloatRange intensityRange = new FloatRange(9.5f, 11f);
    public float intensityTarget;
    [Range(0.1f, 1f)] public float intensityFlickSpeed = 0.2f; 

    private readonly TimerHandler _handler = new TimerHandler();
    private readonly LocalTimer _localTimer = new LocalTimer();

    private void Awake()
    {
        if (light == null)
            light = GetComponent<Light>();
    }

    private void Update()
    {
        light.intensity = Mathf.Lerp(light.intensity, intensityTarget, intensityFlickSpeed);
        
        if (_handler.IsActive) return;

        _localTimer.SetTimer(_handler, () => intensityTarget = intensityRange.Random(), flickerInterval.Random());
    }

    private void LateUpdate()
    {
        _localTimer.Handle();
    }
}