using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    [System.Serializable]
    public sealed class CharacterInputPC : CharacterInput
    {
        [SerializeField] private float _inputX;
        [SerializeField] private float _inputY;

        private readonly CursorGameSelection _selection;
        private readonly IEntity _parent;
        
        public override bool HasInput => _inputX != 0 || _inputY != 0;

        public CharacterInputPC(CursorGameSelection selection, IEntity parent)
        {
            _parent = parent;
            _selection = selection;

            MovementInput = Vector3.forward;
            AimInput = Vector3.forward;
            
            LastMovementInput = World.ScreenRight;
            LastAimInput = World.ScreenRight;
           
            ExecutionSystem.AddUpdate(this);
            EventManager.Subscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Subscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }

        private void UpdateX(float x) => _inputX = x;
        private void UpdateY(float y) => _inputY = y;

        public override Vector3 CalculateMovementInput()
        {
            return (World.ScreenRight * _inputX + World.ScreenForward * _inputY).normalized;
        }

        public override Vector3 CalculateAimInput()
        {
            return (_selection.raycastResult.Point - _parent.Position).normalized;
        }

        public override void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, true);
            EventManager.Unsubscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Unsubscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }

        public override void DisableInput()
        {
            InputSystem.UnbindAxis(InputProfile.Gameplay,"Right", AxisEvent.Fixed, UpdateX);
            InputSystem.UnbindAxis(InputProfile.Gameplay,"Forward", AxisEvent.Fixed, UpdateY);
        }

        public override void EnableInput()
        {
            InputSystem.BindAxis(InputProfile.Gameplay,"Right", AxisEvent.Fixed, UpdateX);
            InputSystem.BindAxis(InputProfile.Gameplay,"Forward", AxisEvent.Fixed, UpdateY);
        }
    }
}