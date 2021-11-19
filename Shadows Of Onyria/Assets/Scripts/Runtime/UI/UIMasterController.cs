using System;
using System.Collections.Generic;
using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    public class UIMasterController : MonoBehaviour
    {
        private static UIMasterController Instance { get; set; }
        
        public static event Action OnHideUI;
        public static event Action OnActivateMenu;

        public static bool IsUIActive => _activeCount > 0;
        
        private static readonly List<IActiveUIComponent> ActiveUIComponents = new List<IActiveUIComponent>();
        private static int _activeCount;

        [SerializeField] private CursorGameSelection _selection;
        [SerializeField] private List<SceneFlag> UIFlags = new List<SceneFlag>(); 
        public static ITargetableUI CurrentTarget => Instance._selection.GetPointerTarget?.Invoke();
        public static bool HasInstance => Instance != null;

        public void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);

            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }

        private void Unload(params object[] parameters)
        {
            Destroy(Instance);
        }

        private void Start()
        {
            InputSystem.BindKey(InputProfile.Gameplay,"Menu", KeyEvent.Release, MenuKey);
            InputSystem.BindKey(InputProfile.UI,"Exit", KeyEvent.Release, CloseHUD);
            InputSystem.BindKey(InputProfile.UI,"Select", KeyEvent.Release, PropagateSelectInput);
            InputSystem.BindKey(InputProfile.UI,"Return", KeyEvent.Release, PropagateReturnInput);
            
            if (UIFlags.Count == 0) return;
            
            for (var i = 0; i < UIFlags.Count; i++)
            {
                UIFlags[i].ResetValue();
            }
        }

        public static void AddTrackingUI(IActiveUIComponent component)
        {
            if (ActiveUIComponents.Contains(component)) return;
            if (_activeCount == 0)
            {
                InputSystem.SetCurrentProfile(InputProfile.UI);
            }
            ActiveUIComponents.Add(component);
            _activeCount += 1;
            
        }
        public static void RemoveTrackedUI(IActiveUIComponent component)
        {
            if (!ActiveUIComponents.Contains(component)) return;
            ActiveUIComponents.Remove(component);
            _activeCount -= 1;
            
            if (_activeCount == 0)
            {
                InputSystem.SetCurrentProfile(InputProfile.Gameplay);
                
                if (Instance.UIFlags.Count == 0) return;
            
                for (var i = 0; i < Instance.UIFlags.Count; i++)
                {
                    Instance.UIFlags[i].ResetValue();
                }
            }
        }

        private void PropagateSelectInput()
        {
            if (!IsUIActive) return;

            var tar = _selection.GetPointerTarget?.Invoke();
            if (tar != null)
            {
                tar.OnClick();
                return;
            }
            
            for (int i = 0; i < ActiveUIComponents.Count; i++)
            {
                if (ActiveUIComponents[i] is IInputUIComponent inputUI)
                {
                    inputUI.OnSelectInput();
                }
            }
        }
        private void PropagateReturnInput()
        {
            if (!IsUIActive) return;
            
            for (int i = 0; i < ActiveUIComponents.Count; i++)
            {
                if (ActiveUIComponents[i] is IInputUIComponent inputUI)
                {
                    inputUI.OnReturnInput();
                }
            }
        }

        private void MenuKey()
        {
            if (!GameManager.CanOpenPauseMenu) return;
            if (!UIIngameMenu.IsShowing && _activeCount == 0)
            {
                UIIngameMenu.ActivateMainMenu();
            }
            
            InputSystem.SetCurrentProfile(InputProfile.UI);
        }

        private void CloseHUD()
        {
            OnHideUI?.Invoke();
            ActiveUIComponents.Clear();
            UIIngameMenu.DeactivateMainMenu();
            _activeCount = 0;
            if (UIFlags.Count == 0) return;
            
            for (var i = 0; i < UIFlags.Count; i++)
            {
                UIFlags[i].ResetValue();
            }
            
            InputSystem.SetCurrentProfile(InputProfile.Gameplay);
        }

        private void OnDestroy()
        {
            OnHideUI = default;
            OnActivateMenu = default;
            ActiveUIComponents.Clear();
            _activeCount = 0;
            Instance = null;
            
            if (UIFlags.Count == 0) return;
            
            for (var i = 0; i < UIFlags.Count; i++)
            {
                UIFlags[i].ResetValue();
            }
            
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
            InputSystem.UnbindKey(InputProfile.Gameplay,"Menu", KeyEvent.Release, MenuKey);
            InputSystem.UnbindKey(InputProfile.UI,"Exit", KeyEvent.Release, CloseHUD);
            InputSystem.UnbindKey(InputProfile.UI,"Select", KeyEvent.Release, PropagateSelectInput);
            InputSystem.UnbindKey(InputProfile.UI,"Return", KeyEvent.Release, PropagateReturnInput);
        }
    }
}
