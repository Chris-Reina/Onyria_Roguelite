using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(fileName = "CursorGameSelection", menuName = "Data/CursorGameSelection")]
    public class CursorGameSelection : ScriptableObject
    {
        [HideInInspector] public CursorRaycastResult raycastResult;
        public Func<ITargetableUI> GetPointerTarget;

        private void OnDisable()
        {
            raycastResult = default;
            GetPointerTarget = default;
        }
    }
}
