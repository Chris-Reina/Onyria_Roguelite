using System.Collections;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class DebugSceneFlagTrigger : MonoBehaviour
{
    public SceneFlag flag;
    public bool changeTo = true;
    
    public void Trigger()
    {
        flag.Value = changeTo;
    }
}
