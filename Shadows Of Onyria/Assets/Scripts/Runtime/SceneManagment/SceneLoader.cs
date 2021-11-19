using System;
using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Game Manager")] 
        [SerializeField] private SceneReference gameManager;
        [SerializeField] private SceneEvent onLoadEvent;
        
        [Header("Completion Scene Events")]
        [SerializeField] private float eventEndingDelay;
        [SerializeField] private SceneEvent sceneEndingEvent;
        [SerializeField] private SceneEvent defeatEvent;

        [Header("Scene References")]
        [SerializeField] private ScenePair currentScene;
        [SerializeField] private ScenePair nextSceneAtWin;
        [SerializeField] private ScenePair nextSceneAtDefeat;

        private SceneLoadData WinLoadData => new SceneLoadData(currentScene, nextSceneAtWin);
        private SceneLoadData LoseLoadData => new SceneLoadData(currentScene, nextSceneAtDefeat);

        private bool _hasCalled;
        
        private void Awake()
        {
            if (sceneEndingEvent == null)
            {
                DebugManager.LogError("There is no scene ending event in the sceneLoader");
                return;
            }
            
            EventManager.Subscribe(sceneEndingEvent, LoadWinSceneEvent);
            if (defeatEvent != null) EventManager.Subscribe(defeatEvent, LoadDefeatSceneEvent);
            
            if (GameManager.Current == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(gameManager, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
        }

        private void Start()
        {
            EventManager.Raise(onLoadEvent);
        }

        private void LoadWinSceneEvent(params object[] parameters)
        {
            if (_hasCalled) return;
            GameManager.SceneLoaderCalled = true;
            InputSystem.DisableInputs();
            _hasCalled = true;
            var delay = Mathf.Max(eventEndingDelay, 0.1f);
            TimerManager.SetTimer(new TimerHandler(), () => GameManager.BeginLoad(WinLoadData), delay);
        }
        private void LoadDefeatSceneEvent(params object[] parameters)
        {
            if (_hasCalled) return;
            GameManager.SceneLoaderCalled = true;
            InputSystem.DisableInputs();
            _hasCalled = true;
            var delay = Mathf.Max(eventEndingDelay, 0.1f);
            TimerManager.SetTimer(new TimerHandler(), () => GameManager.BeginLoad(LoseLoadData), delay);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(sceneEndingEvent, LoadWinSceneEvent);
            if (defeatEvent != null) EventManager.Unsubscribe(defeatEvent, LoadDefeatSceneEvent);
        }

        public void SetLevelAtWin(Stage stage)
        {
            nextSceneAtWin = stage.ToScenePair();
        }
        
        public void SetLevelAtWin(ScenePair pair)
        {
            nextSceneAtWin = pair;
        }
    }
}
