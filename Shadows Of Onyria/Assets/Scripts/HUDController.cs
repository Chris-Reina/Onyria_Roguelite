using UnityEngine;

namespace DoaT
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup DarknessGroup;
        [SerializeField] private CanvasGroup DarknessGroupOnPlayer;

        private void Awake()
        {
            var f = SceneContext.UseDarkness ? 1f : 0f;

            DarknessGroup.alpha = f;
            if(DarknessGroupOnPlayer != null) DarknessGroupOnPlayer.alpha = f;
        }
    }
}