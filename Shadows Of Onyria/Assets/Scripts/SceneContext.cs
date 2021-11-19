using UnityEngine;

namespace DoaT
{
    public class SceneContext : MonoBehaviour
    {
        public static bool UseDarkness { get; private set; }
        public static bool IsRunMap { get; private set; }

        [SerializeField] private bool _shouldUseDarkness;
        [SerializeField] private bool _isRunMap;

        private void Awake()
        {
            UseDarkness = _shouldUseDarkness;
            IsRunMap = _isRunMap;
        }
    }

}