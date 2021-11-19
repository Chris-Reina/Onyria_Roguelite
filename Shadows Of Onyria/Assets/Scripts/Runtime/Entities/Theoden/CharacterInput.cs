using UnityEngine;

namespace DoaT
{
    public abstract class CharacterInput : IUpdate, IUnloadable, IPausable, IInputHandler
    {
        public virtual Vector3 MovementInput { get; protected set; }
        public virtual Vector3 AimInput { get; protected set; }
        public virtual Vector3 LastMovementInput { get; set; } = Vector3.zero;
        public virtual Vector3 LastAimInput { get; set; } = Vector3.zero;

        public abstract bool HasInput { get; }

        public virtual void OnUpdate()
        {
            MovementInput = CalculateMovementInput();
            AimInput = CalculateAimInput();
        }

        public virtual Vector3 CalculateMovementInput()
        {
            return default;
        }

        public virtual Vector3 CalculateAimInput()
        {
            return default;
        }

        public virtual void UpdateLastMovementDirection()
        {
            LastMovementInput = MovementInput;
        }

        public virtual void UpdateLastAimingDirection()
        {
            LastAimInput = AimInput;
        }
        
        public virtual void UpdateLastMovementDirection(Vector3 inputOverride)
        {
            LastMovementInput = inputOverride;
        }

        public virtual void UpdateLastAimingDirection(Vector3 inputOverride)
        {
            LastAimInput = inputOverride;
        }

        public abstract void Unload(params object[] parameters);

        public virtual void OnGamePause() => DisableInput();
        public virtual void OnGameResume() => EnableInput();
        
        public abstract void DisableInput();
        public abstract void EnableInput();
        
        public virtual void DisableInputEvent(params object[] parameters) => DisableInput();
        public virtual void EnableInputEvent(params object[] parameters) => EnableInput();
    }
}
