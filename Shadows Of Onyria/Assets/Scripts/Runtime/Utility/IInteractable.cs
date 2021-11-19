using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
   string InteractMessage { get; }
   bool Interactable { get; }
   bool Interact();
}
