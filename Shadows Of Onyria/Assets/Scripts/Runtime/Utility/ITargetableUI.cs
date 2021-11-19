using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetableUI
{
    event Action<bool, bool> OnPointerInteract;
    void OnClick();
}
