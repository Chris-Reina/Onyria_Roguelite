using UnityEngine;

public static class CanvasGroupExtensions
{
    public static bool IsShowing(this CanvasGroup cg)
    {
        return cg.alpha == 1;
    }
    
    public static void Activate(this CanvasGroup cg)
    {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    
    public static void Deactivate(this CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public static void Setup(this CanvasGroup cg, float alpha, bool interactable, bool blocksRaycasts)
    {
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = blocksRaycasts;
    }

    public static CanvasGroup SetAlpha(this CanvasGroup cg, float alpha)
    {
        cg.alpha = alpha;
        return cg;
    }
    public static CanvasGroup SetInteractable(this CanvasGroup cg, bool interactable)
    {
        cg.interactable = interactable;
        return cg;
    }
    public static CanvasGroup SetBlocksRaycasts(this CanvasGroup cg, bool blocksRaycasts)
    {
        cg.blocksRaycasts = blocksRaycasts;
        return cg;
    }
    
}
