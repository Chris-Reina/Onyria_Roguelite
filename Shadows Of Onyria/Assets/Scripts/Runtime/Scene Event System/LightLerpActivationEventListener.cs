using System.Collections;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class LightLerpActivationEventListener : BaseSceneEventListener
{
    [Header("Animation")]
    [SerializeField] private float animationTime = 1f;
    [SerializeField] private bool useCurve = true;
    [SerializeField] private AnimationCurve animationCurve;

    [Header("Flag Setting")]
    [SerializeField] private SceneFlag flag;
    [SerializeField] private bool flagChangeTo = true;
    
    [SerializeField] private List<LightPropHandler> lights;

    private readonly TimerHandler _handler = new TimerHandler();
    
    public override void OnEventTriggered(params object[] parameters)
    {
        if (!CanReact) return;

        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].gameObject.SetActive(true);
            lights[i].SetActiveState(true);
            lights[i].SetIntensityStage(0f);
        }

        TimerManager.SetTimer(_handler, animationTime);
        
        StartCoroutine(LightActivation());
        flag.Value = flagChangeTo;
    }

    private IEnumerator LightActivation()
    {
        while (_handler.IsActive)
        {
            for (int i = 0; i < lights.Count; i++)
            {
                var stage = useCurve ? animationCurve.Evaluate(_handler.Progress) : _handler.Progress;
                lights[i].SetIntensityStage(stage);
            }

            yield return new WaitForEndOfFrame();
        }
        
        for (int i = 0; i < lights.Count; i++)
        {
            lights[i].SetIntensityStage(1f);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }
}
