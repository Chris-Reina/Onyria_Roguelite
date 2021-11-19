using System;

namespace DoaT
{
    public interface IActiveUIComponent
    {
        bool IsActive { get; }
    } 
    public interface IGroupController : IActiveUIComponent
    {
        public event Action OnHideUI;
        public event Action OnShowUI;
        public event Action OnUpdateUI;
    }

    public interface IInputUIComponent
    {
        void OnSelectInput();
        void OnReturnInput();
    }
}