using UnityEngine;

namespace DoaT.UI
{
    public class UIExitGameButton : UIButton
    {
        protected override void Awake()
        {
            base.Awake();

            OnInteractionSucceeded += ExitGame;
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
