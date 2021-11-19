using System.Collections.Generic;

namespace DoaT
{
    public class ChallengeStatusEventTrigger : BaseSceneEventTrigger, IUpdate
    {
        public List<EnemyEntity> enemies;

        private bool done;

        private void Start()
        {
            //ExecutionSystem.AddUpdate(this);
            DebugManager.LogError("DEPRECATED!");
        }

        public void OnUpdate()
        {
            if (!CanTrigger) return;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsDead) return;
            }
            
            Trigger();
        }

        public override void Trigger()
        {
            if (!CanTrigger) return;
            
            EventManager.Raise(_sceneEvent);
            _canTrigger = false;
        }
    }
}
