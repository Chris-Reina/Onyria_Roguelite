using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoaT
{
    // [Serializable, CreateAssetMenu(fileName = "Scene Pool", menuName = "Data/Scenes/Pool")]
    // public class ScenePool : ScriptableObject
    // {
    //     
    // }
    
    public class RunPlanner : MonoBehaviourInit //TODO : Reimplement HashSet when More Maps are created
    {
        public List<LevelSceneData> scenePool = new List<LevelSceneData>();

        public int runStagesAmount = 15;
        public int initialEnemiesAmount = 6;
        public IntRange randomEnemiesPerStage = new IntRange();
        
        public SceneData normalUI;

        public SceneData vendorScene;
        public SceneData vendorUI;

        public RunData Output;

        [Space] public SceneLoader sceneLoader;
        

        private void Awake()
        {
            Output.run.Clear(); 
        }
        
        public override float OnInitialization()
        {
            GameManager.Current._gameState.SetCanAttackMelee(true);
            Output.run = Generate(runStagesAmount);
            
            /*//// VENDOR TEST
            /*#1#var vendorStage = new Stage("Trader Crossroad") {scene = vendorScene, uiScene = vendorUI};
            /*#1#Output.run.Insert(0, vendorStage);
            //// VENDOR TEST*/
            
            PersistentData.RunGenerationManager.SetStage(1);
            PersistentData.RunGenerationManager.SetVendorStage(0);
            sceneLoader.SetLevelAtWin(Output.run[0]);
            return 1f;
        }

        private List<Stage> Generate(int maxDepth, int currentDepth = 1)
        {
            if (currentDepth > maxDepth)
                return new List<Stage>();

            var tempStage = new Stage
            {
                scene = scenePool[Random.Range(0, scenePool.Count)],
                uiScene = normalUI,
                enemyCount = initialEnemiesAmount + randomEnemiesPerStage.Random()
            };

            var list = Generate(maxDepth, currentDepth + 1);

            if (currentDepth % 5 == 0)
            {
                var vendorStage = new Stage("Trader Crossroad") { scene = vendorScene, uiScene = vendorUI };
                
                list.Insert(0, vendorStage);
            }

            list.Insert(0, tempStage);
            return list;
        }
    }

    [Serializable]
    public class Stage
    {
        [HideInInspector] public string Name = "Stage";
        
        public SceneData scene;
        public SceneData uiScene;
        
        public int enemyCount;
        
        public Stage(){}

        public Stage(string name)
        {
            Name = name;
        }

        public ScenePair ToScenePair()
        {
            return new ScenePair(scene, uiScene);
        }
    }
}
