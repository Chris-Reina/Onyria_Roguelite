using System;

namespace DoaT
{
    public interface IBehaviour : IUnloadable, IPausable
    {
        event Action<object[]> OnActionCancel;
        event Action<object[]> OnUpdateRotationRequest;
        
        bool IsLocked { get; }
        bool IsOnCooldown { get; }
    }
}