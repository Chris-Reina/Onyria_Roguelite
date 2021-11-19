namespace DoaT
{
    public interface IController : IInputHandler, IPausable
    {
        //TODO: Determine if shared implementation is required

        bool InControl { get; }
        bool IsLocked { get; }
        bool IsOnCooldown { get; }

        void Interrupt();
        void ControlGained();
        void ControlLost();
        
        void Dispose();
    }
}