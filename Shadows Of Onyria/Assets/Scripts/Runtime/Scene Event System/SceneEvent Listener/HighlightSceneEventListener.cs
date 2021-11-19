using System.Linq;
using UnityEngine;

namespace DoaT
{
    public class HighlightSceneEventListener : BaseSceneEventListenerDual
    {
        public Renderer[] renderers;

        private Material[] _materials;
        private static readonly int HighlightAmount = Shader.PropertyToID("_HighlightAmount");

        protected override void Awake()
        {
            base.Awake();

            if (renderers == null)
            {
                DebugManager.LogWarning($"A Listener of type {GetType()} has no renderer attached and will be ignored.");
                return;
            }

            _materials = renderers.SelectMany(x => x.materials, (rend, mat) => mat).ToArray();
        }

        public override void OnEventTriggered(params object[] parameters)
        {
            if (_sceneEvent == null) return;
            foreach (var mat in _materials)
            {
                mat.SetFloat(HighlightAmount, 1f);
            }
        }
        
        public override void OnEventTriggeredDual(params object[] parameters)
        {
            if (_sceneEventDual == null) return;
            foreach (var mat in _materials)
            {
                mat.SetFloat(HighlightAmount, 0f);
            }
        }
    }
}
