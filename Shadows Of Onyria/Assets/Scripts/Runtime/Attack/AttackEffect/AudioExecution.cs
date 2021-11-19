using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [System.Serializable, CreateAssetMenu(menuName = "Abilities/Attacks/Attack Effect Execution/Audio",fileName = "Audio Execution")]
    public class AudioExecution : AttackExecution
    {
        public AudioCue soundCue;

        public override void Execute(HashSet<IGridEntity> targets, AttackInfo info)
        {
            AudioSystem.PlayCue(soundCue);
        }
    }
}
