using System;
using System.Collections;
using System.Collections.Generic;
using DoaT.Inputs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace DoaT
{
    public class GameManager : MonoBehaviour
    {
        public GameState _gameState = new GameState();
        
        public static GameManager Current { get; private set; }
        public static bool IsLoading { get; private set; }
        public static bool CanOpenPauseMenu => !IsLoading && !SceneLoaderCalled;

        public static bool SceneLoaderCalled = false;
        
        public event Action OnLoadingComplete;

        [Header("Scene References")]
        [SerializeField] private SceneReference _gameManager;
        [SerializeField] private SceneReference _mainMenu;
        [SerializeField] private SceneReference _uiIfNotMainMenu;
        
        [Header("First MainMenu Screen")]
        [SerializeField] private float _mainMenuRevealTime;
        [SerializeField] private CanvasGroup _globalCanvasGroup;

        [Header("Global")]
        [SerializeField] private SceneFlagProfile _persistentFlags;
        
        [Header("Loading Screen")]
        [SerializeField] private CanvasGroup _loadingCanvasGroup;
        [SerializeField] private Image _progressMark;
        [SerializeField] private float _loadingRevealHideTime;
        
        private void Awake()
        {
            var loadMainMenu = World.Current == null;
            
            if (Current == null)
            {
                Current = this;
                StartCoroutine(loadMainMenu ? LoadMainMenu() : FadeOut());
            }
            else if (Current != this)
            {
                Destroy(gameObject);
            }
        }

        public static void BeginLoad(SceneLoadData levelData)
        {
            if (IsLoading) return;
            
            if (Current == null)
                throw new NullReferenceException();
            
            IsLoading = true;
            Current.BeginLoadInternal(levelData);
        }

        private void BeginLoadInternal(SceneLoadData levelData)
        {
            EventManager.Raise(GameEvents.OnSceneUnload);
            World.Clear();
            InputSystem.Clear();

            StartCoroutine(LoadScene(levelData));
        }

        private IEnumerator FadeOut()
        {
            if(_uiIfNotMainMenu != null) SceneManager.LoadSceneAsync(_uiIfNotMainMenu, LoadSceneMode.Additive);

            var current = Initializer.Current;
            current.BeginInitialization();
            var isInitializingDone = false;
            
            while (!isInitializingDone) //INITIALIZATION MANAGER % 
            {
                var initializationProgress = 0f;
                isInitializingDone = true;

                if (current.IsDone)
                {
                    initializationProgress = 1f;
                }
                else
                {
                    initializationProgress = current.Progress;
                    isInitializingDone = false;
                }

                _progressMark.fillAmount = (1 + initializationProgress) / 2;

                yield return null;
            } 
            
            while (_globalCanvasGroup.alpha > 0f)
            {
                var value = _globalCanvasGroup.alpha;
                _globalCanvasGroup.alpha = Mathf.Clamp01(value - Time.deltaTime * _loadingRevealHideTime);

                yield return null;
            }
            
            _progressMark.fillAmount = 0f;
            OnLoadingComplete?.Invoke();
            InputSystem.EnableInputs();
        }

        private IEnumerator LoadSaveFile(SceneLoadData levelData)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_gameManager));
            
            yield return ShowBackground();
            yield return ShowLoadingIcon();
            
            var operations = new List<AsyncOperation>();
            
            if (levelData.CurrentUiScene != null) operations.Add(SceneManager.UnloadSceneAsync(levelData.CurrentUiScene));
            operations.Add(SceneManager.UnloadSceneAsync(levelData.CurrentScene));
            operations.Add( SceneManager.LoadSceneAsync(levelData.NextScene, LoadSceneMode.Additive));
            if(levelData.NextUiScene != null) operations.Add(SceneManager.LoadSceneAsync(levelData.NextUiScene, LoadSceneMode.Additive));

            yield return SceneLoadImpl(operations);

            _progressMark.fillAmount = 0.5f;
            var current = Initializer.Current;

            while (current == null)
            {
                yield return null;
                current = Initializer.Current;
            }

            yield return InitializerImpl(current);
            
            yield return HideLoadingIcon();
            yield return HideBackground();

            _progressMark.fillAmount = 0f;
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelData.NextScene));
            OnLoadingComplete?.Invoke();
            IsLoading = false;
            SceneLoaderCalled = false;
        }

        private IEnumerator InitializerImpl(Initializer current)
        {
            current.BeginInitialization();
            var isInitializingDone = false;
            
            while (!isInitializingDone) //INITIALIZATION MANAGER % 
            {
                var initializationProgress = 0f;
                isInitializingDone = true;

                if (current.IsDone)
                {
                    initializationProgress = 1f;
                }
                else
                {
                    initializationProgress = current.Progress;
                    isInitializingDone = false;
                }

                _progressMark.fillAmount = (1 + initializationProgress) / 2;

                yield return null;
            } 

            _progressMark.fillAmount = 1f;
        }
        private IEnumerator SceneLoadImpl(List<AsyncOperation> operations)
        {
            var isLoadingDone = false;

            while (!isLoadingDone) //LOADING SCENE
            {
                var loadingLoadProgress = 0f;
                isLoadingDone = true;

                for (int i = 0; i < operations.Count; i++)
                {
                    if (operations[i] == null || operations[i].isDone)
                    {
                        loadingLoadProgress += 1;
                        continue;
                    }

                    loadingLoadProgress += operations[i].progress / 0.9f;
                    isLoadingDone = false;
                }

                _progressMark.fillAmount = (loadingLoadProgress / operations.Count) / 2;
                yield return null;
            }
        }
        private IEnumerator ShowLoadingIcon()
        {
            while (_loadingCanvasGroup.alpha < 1f)
            {
                var value = _loadingCanvasGroup.alpha;
                _loadingCanvasGroup.alpha = Mathf.Clamp01(value + Time.deltaTime * 1);

                yield return null;
            }
        }
        private IEnumerator HideLoadingIcon()
        {
            while (_loadingCanvasGroup.alpha > 0f)
            {
                var value = _loadingCanvasGroup.alpha;
                _loadingCanvasGroup.alpha = Mathf.Clamp01(value - Time.deltaTime * 1);

                yield return null;
            }
        }
        private IEnumerator ShowBackground()
        {
            _globalCanvasGroup.blocksRaycasts = true;
            
            while (_globalCanvasGroup.alpha < 1f)
            {
                var value = _globalCanvasGroup.alpha;
                _globalCanvasGroup.alpha = Mathf.Clamp01(value + Time.deltaTime * _loadingRevealHideTime);

                yield return new WaitForEndOfFrame();
            }
        }
        private IEnumerator HideBackground()
        {
            _globalCanvasGroup.blocksRaycasts = false;
            
            while (_globalCanvasGroup.alpha > 0f)
            {
                var value = _globalCanvasGroup.alpha;
                _globalCanvasGroup.alpha = Mathf.Clamp01(value - Time.deltaTime * _loadingRevealHideTime);

                yield return null;
            }
        }
        
        private IEnumerator LoadScene(SceneLoadData levelData)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_gameManager));
            
            _globalCanvasGroup.blocksRaycasts = true;
            
            //SHOW BLACK BACKGROUND
            while (_globalCanvasGroup.alpha < 1f)
            {
                var value = _globalCanvasGroup.alpha;
                _globalCanvasGroup.alpha = Mathf.Clamp01(value + Time.deltaTime * _loadingRevealHideTime);

                yield return new WaitForEndOfFrame();
            }
            
            //SHOW LOADING ICON
            while (_loadingCanvasGroup.alpha < 1f)
            {
                var value = _loadingCanvasGroup.alpha;
                _loadingCanvasGroup.alpha = Mathf.Clamp01(value + Time.deltaTime * 1);

                yield return null; 
            }

            var operations = new List<AsyncOperation>();
            
            if (levelData.CurrentUiScene != null) operations.Add(SceneManager.UnloadSceneAsync(levelData.CurrentUiScene));
            operations.Add(SceneManager.UnloadSceneAsync(levelData.CurrentScene));
            operations.Add( SceneManager.LoadSceneAsync(levelData.NextScene, LoadSceneMode.Additive));
            if(levelData.NextUiScene != null) operations.Add(SceneManager.LoadSceneAsync(levelData.NextUiScene, LoadSceneMode.Additive));

            var isLoadingDone = false;

            while (!isLoadingDone) //LOADING SCENE
            {
                var loadingLoadProgress = 0f;
                isLoadingDone = true;

                for (int i = 0; i < operations.Count; i++)
                {
                    if (operations[i] == null || operations[i].isDone)
                    {
                        loadingLoadProgress += 1;
                        continue;
                    }

                    loadingLoadProgress += operations[i].progress / 0.9f;
                    isLoadingDone = false;
                }

                _progressMark.fillAmount = (loadingLoadProgress / operations.Count) / 2;
                yield return null;
            }

            _progressMark.fillAmount = 0.5f;
            var current = Initializer.Current;

            while (current == null)
            {
                yield return null;
                current = Initializer.Current;
            }

            current.BeginInitialization();
            var isInitializingDone = false;
            
            while (!isInitializingDone) //INITIALIZATION MANAGER % 
            {
                var initializationProgress = 0f;
                isInitializingDone = true;

                if (current.IsDone)
                {
                    initializationProgress = 1f;
                }
                else
                {
                    initializationProgress = current.Progress;
                    isInitializingDone = false;
                }

                _progressMark.fillAmount = (1 + initializationProgress) / 2;

                yield return null;
            } 

            _progressMark.fillAmount = 1f;
            
            //HIDE LOADING ICON
            while (_loadingCanvasGroup.alpha > 0f)
            {
                var value = _loadingCanvasGroup.alpha;
                _loadingCanvasGroup.alpha = Mathf.Clamp01(value - Time.deltaTime * 1);

                yield return null;
            }

            _globalCanvasGroup.blocksRaycasts = false;
            //HIDE BLACK SCREEN
            while (_globalCanvasGroup.alpha > 0f)
            {
                var value = _globalCanvasGroup.alpha;
                _globalCanvasGroup.alpha = Mathf.Clamp01(value - Time.deltaTime * _loadingRevealHideTime);

                yield return null;
            }
            
            _progressMark.fillAmount = 0f;
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelData.NextScene));
            OnLoadingComplete?.Invoke();
            IsLoading = false;
            SceneLoaderCalled = false;
        }
        
        private IEnumerator LoadMainMenu()
        {
            yield return new WaitForEndOfFrame();
            
            var mainMenuOperation = SceneManager.LoadSceneAsync(_mainMenu, LoadSceneMode.Additive);
            mainMenuOperation.allowSceneActivation = false;
            
            while (!mainMenuOperation.isDone)
            {
                if (mainMenuOperation.progress >= 0.9f)
                {
                    mainMenuOperation.allowSceneActivation = true;
                }
                
                yield return new WaitForEndOfFrame();
            }

            _globalCanvasGroup.blocksRaycasts = false;
            
            while (_globalCanvasGroup.alpha > 0f)
            {
                var value = _globalCanvasGroup.alpha;
                _globalCanvasGroup.alpha = Mathf.Clamp01(value - Time.deltaTime * _mainMenuRevealTime);

                yield return new WaitForEndOfFrame();
            }

            _progressMark.fillAmount = 0f;
        }
    }

    [Serializable]
    public class GameState
    {
        public static GameState Current { get; private set; }
        
        [SerializeField] private SceneFlag _hasSword;
        [SerializeField] private SceneFlag _soulAltarDiscovered;
        [SerializeField] private SceneFlag _soulAltarDoorOpened;
        
        [SerializeField] private bool _InCombat = false;
        [SerializeField] private bool _CanAttackMelee = false;
        [SerializeField] private bool _CanAttackRange = true;
        [SerializeField] private bool _CanDash = true;
        [SerializeField] private bool _CanMove = true;

        public SceneFlag HasSword => Current._hasSword;
        public SceneFlag SoulAltarDiscovered => Current._soulAltarDiscovered;
        public SceneFlag SoulAltarDoorOpened => Current._soulAltarDoorOpened;
        
        public static bool InCombat => Current._InCombat;
        public static bool CanAttackRange => Current._CanAttackRange;
        public static bool CanAttackMelee => Current._CanAttackMelee;
        public static bool CanDash => Current._CanDash;
        public static bool CanMove => Current._CanMove;

        public void SetCombatStatus(bool inCombat) => _InCombat = inCombat;
        public void SetCanAttackRange(bool canAttackRange) => _CanAttackRange = canAttackRange;
        public void SetCanAttackMelee(bool canAttackMelee) => _CanAttackMelee = canAttackMelee;
        public void SetCanDash(bool canDash) => _CanDash = canDash;
        public void SetCanMove(bool canMove) => _CanMove = canMove;

        public GameState()
        {
            Current = this;
        }

        public void Load(bool hasSword, bool altarDiscovered, bool altarDoorOpen)
        {
            _hasSword.Value = hasSword;
            _hasSword.Value = hasSword;
            _hasSword.Value = hasSword;
        }
    }
}