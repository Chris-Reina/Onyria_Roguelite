using DoaT;
using UnityEngine;

public class RunMapSetter : MonoBehaviour
{
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = GetComponent<SceneLoader>();
    }

    private void Start()
    {
        if (SceneContext.IsRunMap)
        {
            _sceneLoader.SetLevelAtWin(PersistentData.RunGenerationManager.GetNextLevelScene());
        }
    }
}
