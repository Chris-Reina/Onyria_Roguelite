using System;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Entity/Enemy", fileName = "Enemy Entity")]
    public class Enemy : ScriptableObject
    {
        public EnemyEntity prefab;
        public float difficultyScore;
    }
}
