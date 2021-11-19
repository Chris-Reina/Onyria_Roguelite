using DoaT;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Will let other scene event components operate if the flag value matches the desired outcome (default is true))
/// </summary>
public sealed class SceneFlagCondition : BaseSceneEventCondition
{
    [SerializeField] private SceneFlag flag;
    [SerializeField] private bool valueToMatch;
    public override bool CanOperate => flag.Value == valueToMatch;
}
