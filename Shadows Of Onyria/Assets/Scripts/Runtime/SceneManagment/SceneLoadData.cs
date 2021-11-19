using System;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public class SceneLoadData
    {
        [SerializeField] private ScenePair _scene;
        [SerializeField] private ScenePair _nextScene;

        public SceneReference CurrentScene => _scene.scene != null ? _scene.scene.scene : null;
        public SceneReference CurrentUiScene => _scene.ui != null ? _scene.ui.scene : null;
        public SceneReference NextScene => _nextScene.scene != null ? _nextScene.scene.scene : null;
        public SceneReference NextUiScene => _nextScene?.ui != null ? _nextScene.ui.scene : null;

        public SceneLoadData(ScenePair current, ScenePair next)
        {
            _scene = current;
            _nextScene = next;
        }
    }
}