using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoaT
{
    public class GameSelectionManager : Manager<CursorGameSelection>, IUpdate
    {
        private Camera _camera;
        private PlaneManager _plane;
        private ITargetableUI _currentPointerTarget;

        public ITargetableUI CurrentPointerTarget => _currentPointerTarget;

        public CursorRaycastResult CursorSelection
        {
            get => _config.raycastResult;
            private set => _config.raycastResult = value;
        }

        public void Initialize(PlaneManager plane, Camera mainCamera)
        {
            Initialize();
            _plane = plane;
            _camera = mainCamera;
            
            CursorSelection = new MovementRaycastResult(Vector3.zero);

            EventManager.Subscribe(UIEvents.OnSlotPointerEnter, AssignCurrentPointerTarget);
            EventManager.Subscribe(UIEvents.OnSlotPointerExit, ReleaseCurrentPointerTarget);
            
            _config.GetPointerTarget += () => CurrentPointerTarget;

            ExecutionSystem.AddUpdate(this);
        }

        public void OnUpdate()
        {
            CursorSelection = GetRaycastResult(_camera.ScreenPointToRay(Input.mousePosition));
        }

        private CursorRaycastResult GetRaycastResult(Ray ray)
        {
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) // UI
            {
                Vector3 result;

                if (Physics.Raycast(ray, out var world, 100f, LayersUtility.TRAVERSABLE_MASK))
                {
                    result = world.point;
                }
                else
                {
                    _plane.Plane.Raycast(ray, out var distance);
                    result = ray.GetPoint(distance);
                }

                return new UIRaycastResult(_currentPointerTarget, result);
            }

            if (!Physics.Raycast(ray, out var hit, 100f, LayersUtility.CURSOR_SELECTOR_MASK)
            ) //if the raycast hit nothing it will return a Plane based Movement Result.
            {
                _plane.Plane.Raycast(ray, out var result);
                return new MovementRaycastResult(ray.GetPoint(result));
            }

            var layer = hit.transform.gameObject.layer;

            // if (layer == LayersUtility.INTERACTABLE_MASK_INDEX || layer == LayersUtility.WORLD_ITEM_MASK_INDEX
            // ) //INTERACTABLE
            // {
            //     var interactable = hit.transform.parent.GetComponentInChildren<IInteractable>();
            //
            //     return new InteractableRaycastResult(hit.point, interactable);
            // }

            if (layer == LayersUtility.ENTITY_MASK_INDEX) //ENTITY
            {
                var entity = hit.transform.GetComponent<IEntity>();
                return new EntityRaycastResult(entity.Position, entity);
                //return new EntityRaycastResult(hit.point, entity);
            }

            return layer == LayersUtility.TRAVERSABLE_MASK_INDEX ? new MovementRaycastResult(hit.point) : null;
        }

        #region Events

        private void AssignCurrentPointerTarget(params object[] parameters)
        {
            _currentPointerTarget = (ITargetableUI) parameters[0];
        }

        private void ReleaseCurrentPointerTarget(params object[] parameters)
        {
            var slot = (ITargetableUI) parameters[0];

            if (ReferenceEquals(_currentPointerTarget, slot))
            {
                _currentPointerTarget = null;
            }
        }

        #endregion

        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, true);
            EventManager.Unsubscribe(UIEvents.OnSlotPointerEnter, AssignCurrentPointerTarget);
            EventManager.Unsubscribe(UIEvents.OnSlotPointerExit, ReleaseCurrentPointerTarget);
        }
        
        
    }

}