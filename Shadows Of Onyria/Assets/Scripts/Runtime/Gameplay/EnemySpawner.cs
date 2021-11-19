using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DoaT.AI;
using UnityEngine;

namespace DoaT
{
    public class EnemySpawner : MonoBehaviourInit, IUpdate, IUnloadable
    {
        public event Action<TheodenController> OnEnemyDetected;
        
        [SerializeField] private SceneEvent _challengeCompleteEvent;
        [SerializeField] private SceneFlag _challengeCompleteFlag;
        [SerializeField] private Graph _pathfindingGraph;
        [SerializeField] private float _startPositionDeadZone;
        [SerializeField] private float _entitySeparation;

        [SerializeField] private List<EnemyEntity> _enemies = new List<EnemyEntity>();
        
        public bool EnemyDetected { get; private set; }

        private void Awake()
        {
            if(_pathfindingGraph == null) _pathfindingGraph = FindObjectOfType<Graph>();
        }

        private void Start()
        {
            GameManager.Current.OnLoadingComplete += AddUpdateInit;
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }

        private void AddUpdateInit() => ExecutionSystem.AddUpdate(this);
        
        public override float OnInitialization()
        {
            var tuples = PersistentData.RunGenerationManager.GetEnemiesToSpawn();

            foreach (var (enemy, amount) in tuples)
            {
                for (int i = 0; i < amount; i++)
                {
                    SpawnEnemy(enemy);
                }
            }

            return 1f;
        }

        public void SpawnEnemy(EnemyEntity enemy)
        {
            var pos = GetSpawnPosition();
            var temp = Instantiate(enemy, transform);
            temp.SetManager(this);
            temp.transform.position = pos;
            _enemies.Add(temp);
        }

        public void SpawnEnemy(EnemyEntity enemy, Vector3 center, float initialRadius)
        {
            var pos = GetSpawnPosition(center, initialRadius);
            var temp = Instantiate(enemy, transform);
            temp.SetManager(this);
            temp.transform.position = pos;
            _enemies.Add(temp);
        }

        public void RaiseEnemyDetection(TheodenController player)
        {
            EnemyDetected = true;
            OnEnemyDetected?.Invoke(player);
        }

        public void OnUpdate()
        {
            if (_challengeCompleteFlag.Value) return;
            
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (!_enemies[i].IsDead) return;
            }

            EventManager.Raise(_challengeCompleteEvent);
            PersistentData.RunGenerationManager.AddToCurrentStage();
            _challengeCompleteFlag.Value = true;
        }

        private Vector3 GetSpawnPosition(Vector3 center, float initialRadius)
        {
            var positions = _enemies.Select(x => x.Position).ToArray();

            return _pathfindingGraph.GetValidPosition(center, initialRadius, 0.75f, ref positions);
        }
        
        private Vector3 GetSpawnPosition()
        {
            var positions = _enemies.Select(x => x.Position).ToArray();

            return _pathfindingGraph.GetValidPosition(World.GetPlayer().Position,_startPositionDeadZone, ref positions, _entitySeparation);
        }

        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, true);
        }
        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
            GameManager.Current.OnLoadingComplete -= AddUpdateInit;
        }
    }
}
