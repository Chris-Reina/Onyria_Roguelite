using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoaT
{
    public class RunGenerationManager : MonoBehaviour
    {
        private const int STAGE_NUMBER_CAP = 19;
        
        [SerializeField] private RunData _runData;
        [SerializeField] private EnemyEntity[] _enemies; //TODO : MAKE GENERATION MODULAR
        
        private int _currentStage = 1;
        private int _vendorAmount = 0;

        public int CurrentStage
        {
            get => _currentStage;
            set
            {
                if (value > STAGE_NUMBER_CAP)
                {
                    _currentStage = STAGE_NUMBER_CAP;
                    return;
                }

                if (value <= 0)
                {
                    _currentStage = 1;
                    return;
                }

                _currentStage = value;
            }
            
        } 

        public Tuple<EnemyEntity, int>[] GetEnemiesToSpawn()
        {
            //return new Tuple<EnemyEntity, int>[1] {new Tuple<EnemyEntity, int>(_enemies[1], 1)};
            return GetEnemiesToSpawnImpl();
        }

        public void AssignExperience()
        {
            var exp = Mathf.RoundToInt(750 + (150 * CurrentStage * (CurrentStage / 1.5f)));
            PersistentData.Player.Experience += exp;
        }

        private Tuple<EnemyEntity, int>[] GetEnemiesToSpawnImpl() //TODO : Implement Percentile generation random chance system
        {
            var amount = _runData.run[CurrentStage - 1].enemyCount;
            var tuples = new Tuple<EnemyEntity, int>[4];

            var zombie = 0;
            var skull = 0;
            var demon = 0;
            var banshee = 0;
            
            for (int i = 0; i < amount; i++)
            {
                var rand = Random.Range(0, 4);

                switch (rand)
                {
                    case 0:
                        zombie += 1;
                        break;
                    case 1:
                        skull += 1;
                        break;
                    case 2:
                        demon += 1;
                        break;
                    case 3:
                        banshee += 1;
                        break;
                }
            }

            tuples[0] = new Tuple<EnemyEntity, int>(_enemies[0], zombie);
            tuples[1] = new Tuple<EnemyEntity, int>(_enemies[1], skull);
            tuples[2] = new Tuple<EnemyEntity, int>(_enemies[2], demon);
            tuples[3] = new Tuple<EnemyEntity, int>(_enemies[3], banshee);

            return tuples;
        }

        public ScenePair GetCurrentLevelScene()
        {
            return _runData.run[CurrentStage].ToScenePair();
        }
        public ScenePair GetNextLevelScene()
        {
            var stg = CurrentStage + 1 > STAGE_NUMBER_CAP ? STAGE_NUMBER_CAP - 1 : CurrentStage;

            return _runData.run[stg].ToScenePair();
        }
        
        public void AddToCurrentStage()
        {
            CurrentStage += 1;
        }

        public void SetStage(int stage)
        {
            CurrentStage = stage;
        }

        public string GetZoneName()
        {
            var isVendor = CurrentStage % 6 == 0;

            return isVendor ? "Trader's Crossroad" : $"Catacombs {CurrentStage - _vendorAmount}";
        }

        public void AddToVendorStage() => _vendorAmount += 1;
        public void SetVendorStage(int i) => _vendorAmount = i;

    }
}
