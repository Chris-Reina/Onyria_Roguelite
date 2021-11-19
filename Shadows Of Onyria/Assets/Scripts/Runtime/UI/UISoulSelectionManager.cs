using System;

namespace DoaT
{
    public class UISoulSelectionManager : CanvasGroupController, IInputUIComponent
    {
        public UISoul selectedSoul;
        private bool _hasSoul = false;

        protected override void Awake()
        {
            base.Awake();
            EventManager.Subscribe(UIEvents.OnTargetableClicked, OnTargetableClicked);
        }

        private void OnTargetableClicked(params object[] parameters)
        {
            if (!IsActive) return;
            
            var target = (ITargetableUI)parameters[0];

            if (_hasSoul)
            {
                switch (target)
                {
                    //Swap
                    case UISoul soul:
                    {
                        var oldSoul = selectedSoul;
                        selectedSoul = soul;
                        EventManager.Raise(UIEvents.OnSoulDropped, oldSoul);
                        EventManager.Raise(UIEvents.OnSoulSelected, soul);
                        break;
                    }
                    //Deposit
                    case UISoulSlot slot when slot.ContainedSoul == null:
                        slot.SetContainedSoul(selectedSoul);
                        selectedSoul.ContainedSoul.slotID = slot.SoulSlotType;
                        EventManager.Raise(UIEvents.OnSoulDropped, selectedSoul);
                        selectedSoul = null;
                        _hasSoul = false;
                        break;
                    //Swap
                    case UISoulSlot slot:
                        var oldSoul1 = selectedSoul;
                        selectedSoul = slot.SwapContainedSoul(selectedSoul);
                        oldSoul1.ContainedSoul.slotID = slot.SoulSlotType;
                        selectedSoul.ContainedSoul.slotID = SoulSlotType.None;
                        EventManager.Raise(UIEvents.OnSoulDropped, oldSoul1);
                        EventManager.Raise(UIEvents.OnSoulSelected, selectedSoul);
                        break;
                }
            }
            else
            {
                switch (target)
                {
                    //Obtain
                    case UISoul soul:
                        selectedSoul = soul;
                        _hasSoul = true;
                        EventManager.Raise(UIEvents.OnSoulSelected, soul);
                        break;
                    //Nothing
                    case UISoulSlot slot when slot.ContainedSoul == null:
                        return; 
                    //Obtain
                    case UISoulSlot slot:
                        selectedSoul = slot.ReleaseContainedSoul();
                        selectedSoul.ContainedSoul.slotID = SoulSlotType.None;
                        _hasSoul = true;
                        EventManager.Raise(UIEvents.OnSoulSelected, selectedSoul);
                        break;
                }
            }
        }

        public void OnSelectInput() { }
        public void OnReturnInput()
        {
            if (!_hasSoul)
            {
                var tar = UIMasterController.CurrentTarget;
                if (tar is UISoulSlot slot && slot.ContainedSoul !=  null)
                {
                    var s = slot.ReleaseContainedSoul();
                    EventManager.Raise(UIEvents.OnSoulDropped, s);
                    return;
                }
                HideUI();
                return;
            }
               
            EventManager.Raise(UIEvents.OnSoulDropped, selectedSoul);
            selectedSoul = null;
            _hasSoul = false;
        }
        public override void HideUI()
        {
            base.HideUI();
            EventManager.Raise(UIEvents.OnSoulWindowApply);
            
            if (selectedSoul == null) return;
            
            EventManager.Raise(UIEvents.OnSoulDropped, selectedSoul);
            selectedSoul = null;
            _hasSoul = false;
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(UIEvents.OnTargetableClicked, OnTargetableClicked);
        }
    }
}
