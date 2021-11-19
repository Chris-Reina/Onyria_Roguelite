using System;
using UnityEngine;

namespace DoaT
{
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem Current { get; set; }

        [SerializeField] private UITooltip _tooltip;
        
        private void Awake()
        {
            if (Current == null)
                Current = this;
            else if (Current != this)
            {
                DebugManager.LogWarning($"Duplicate Singleton of type {GetType()}");
                Destroy(this);
                return;
            }
        }

        public static void Show(Tuple<string,int> content, 
                                Tuple<string,int> header = null, 
                                Tuple<string,int> footer = null)
        {
            var auxHeader = header ?? new Tuple<string,int>("",0);
            var auxFooter = footer ?? new Tuple<string,int>("",0);

            Current._tooltip.SetText(content, auxHeader, auxFooter);
            Current._tooltip.gameObject.SetActive(true);
        }
        
        public static void Hide()
        {
            Current._tooltip.gameObject.SetActive(false);
        }
    }
}
