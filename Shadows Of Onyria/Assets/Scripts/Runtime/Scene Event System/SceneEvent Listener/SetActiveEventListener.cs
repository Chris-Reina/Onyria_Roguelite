using DoaT;
using UnityEngine;

public class SetActiveEventListener : BaseSceneEventListener
{
    [SerializeField] private GameObject _object;
    [SerializeField] private bool _setActive;

    public override void OnEventTriggered(params object[] parameters)
    {
        if(CanReact)
            _object.SetActive(_setActive);
    }
}