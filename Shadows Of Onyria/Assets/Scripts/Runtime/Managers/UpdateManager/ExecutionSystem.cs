using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    /// <summary>
    /// Manages Updates from subscribed objects.
    /// </summary>
    [DisallowMultipleComponent]
    public class ExecutionSystem : MonoBehaviour
    {
        private static ExecutionSystem Current { get; set; }

        private static bool _paused = false;
        public static bool Paused
        {
            get => _paused;
            private set
            {
                ExecutePauseInterface(value);
                _paused = value;
            }
        }
        private readonly HashSet<IPausable> _pausables = new HashSet<IPausable>();

        private readonly Dictionary<IUpdate, UpdateContext> _updates = new Dictionary<IUpdate, UpdateContext>();
        private readonly Queue<IUpdate> _removeQueue = new Queue<IUpdate>();
        
        private readonly HashSet<IUpdate> _addQueueBuckets = new HashSet<IUpdate>();
        private readonly Queue<IUpdate> _addQueue = new Queue<IUpdate>();
        private bool _update;

        private void Awake()
        {
            if (Current == null)
                Current = this;
            else
            {
                Destroy(this);
                return;
            }
        }
        private void Start()
        {
            GameManager.Current.OnLoadingComplete += StartUpdateWarming;
        }

        private void StartUpdateWarming() => StartCoroutine(WarmUpdate());
        
        private void Update()
        {
            // if (Input.GetKeyUp(KeyCode.P) && GameManager.CanOpenPauseMenu)
            // {
            //     Paused = !_paused;
            // }
            
            while (_addQueue.Count > 0)
            {
                var update = _addQueue.Dequeue();
                _addQueueBuckets.Remove(update);
                var ctx = new UpdateContext(update);
                if (ctx.Pause)
                {
                    var pause = update as IPausable;
                    if (!_pausables.Contains(pause)) _pausables.Add(pause);
                }
                
                _updates.Add(update, ctx);
            }
            
            if (!_update) return;
            if (_updates.Count == 0) return;

            foreach (var update in _updates)
            {
                if (update.Value.IsDisposed) continue;
                if (update.Value.Pause && Paused) continue;
                if (update.Key == null)
                {
                    _removeQueue.Enqueue(update.Key);
                    continue;
                }
                
                update.Key.OnUpdate();
            }
        }

        private void LateUpdate()
        {
            if(!_paused) TimerManager.OnUpdate();
            
            while (_removeQueue.Count > 0)
            {
                var update = _removeQueue.Dequeue();

                if (!_updates.ContainsKey(update)) return;
                if (!_updates[update].IsDisposed)
                {
                    continue;
                }
                if (_updates[update].Pause)
                {
                    var pause = update as IPausable;
                    if (_pausables.Contains(pause)) _pausables.Remove(pause);
                }
                
                _updates.Remove(update);
            }
        }

        private IEnumerator WarmUpdate()
        {
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            
            _update = true;
        }
        
        private void AddUpdateImpl(IUpdate update)
        {
            if (_updates.ContainsKey(update))
            {
                var ctx = _updates[update];
                
                if (ctx.IsDisposed)
                    ctx.IsDisposed = false;
                
                return;
            }

            if (_addQueueBuckets.Contains(update)) return;

            _addQueueBuckets.Add(update);
            _addQueue.Enqueue(update);
        }
        private void RemoveUpdateImpl(IUpdate update, bool immediate)
        {
            if (!_updates.ContainsKey(update)) return;
            var pauses = update as IPausable;
            if (_pausables.Contains(pauses)) _pausables.Remove(pauses);
            if (immediate)
            {
                if (_updates[update].Pause)
                {
                    var pause = update as IPausable;
                    if (_pausables.Contains(pause)) _pausables.Remove(pause);
                }
                _updates.Remove(update);
                return;
            }
            _updates[update].IsDisposed = true;
            _removeQueue.Enqueue(update);
        }
        private void AddPausableImpl(IPausable pausable)
        {
            if (_pausables.Contains(pausable)) return;
            _pausables.Add(pausable);
        }
        private void RemovePausableImpl(IPausable pausable)
        {
            if (!_pausables.Contains(pausable)) return;
            _pausables.Remove(pausable);
        }

        public static void AddUpdate(IUpdate update) => Current.AddUpdateImpl(update);
        public static void RemoveUpdate(IUpdate update, bool immediate) => Current.RemoveUpdateImpl(update, immediate);
        public static void AddPausable(IPausable pausable) => Current.AddPausableImpl(pausable);
        public static void RemovePausable(IPausable pausable) => Current.RemovePausableImpl(pausable);
        
        
        private static void ExecutePauseInterface(bool value)
        {
            EventManager.Raise(value ? PlayerEvents.OnDisableInputs : PlayerEvents.OnEnableInputs);

            if (_paused == value)
                return;

            var collection = Current._pausables;
            if (collection.Count <= 0) 
                return;
            
            foreach (var kvp in collection)
            {
                if (value)
                {
                    kvp.OnGamePause();
                    continue;
                }

                kvp.OnGameResume();
            }
        }

        public static void Pause()
        {
            Paused = true;
        }
        public static void Resume()
        {
            Paused = false;
        }
    }

    public class UpdateContext
    {
        public IUpdate Component { get; }
        public bool Pause { get; }
        public bool IsDisposed { get; set; }

        public UpdateContext(IUpdate update)
        {
            Component = update;
            Pause = update is IPausable;
            IsDisposed = false;
        }
    }

    public interface IPausable
    {
        void OnGamePause();
        void OnGameResume();
    }
}
