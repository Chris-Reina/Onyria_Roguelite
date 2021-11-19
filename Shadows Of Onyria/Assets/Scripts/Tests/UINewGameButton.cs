namespace DoaT.UI
{
    public class UINewGameButton : UIButton
    {
        public SceneFlagProfile reset;

        public ScenePair thisLevelData;
        public ScenePair nextLevelData;

        protected override void Awake()
        {
            base.Awake();

            OnInteractionSucceeded += Play;
        }

        private void Play()
        {
            reset.ResetFlags();
            GameManager.BeginLoad(new SceneLoadData(thisLevelData, nextLevelData));
        }
    }
}