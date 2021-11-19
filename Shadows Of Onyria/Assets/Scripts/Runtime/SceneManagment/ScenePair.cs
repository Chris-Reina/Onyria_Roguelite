using System;

namespace DoaT
{
    [Serializable]
    public class ScenePair
    {
        public SceneData scene;
        public SceneData ui;

        public ScenePair(SceneData scene, SceneData ui)
        {
            this.scene = scene;
            this.ui = ui;
        }
    }
}