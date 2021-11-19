using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Gameplay/Enemy Wave", fileName = "Enemy Wave")]
    public class EnemyWave : ScriptableObject
    {
        public bool isSpecialEncounter;

#pragma warning disable 649
        [SerializeField] private List<Enemy> enemies;
        [SerializeField] private List<int> enemiesAmount;
#pragma warning restore 649

        private Dictionary<Enemy, int> _internal;

        private void InitializeDictionary()
        {
            if (enemies.Count != enemiesAmount.Count)
                throw new IndexOutOfRangeException("Cannot initialize dictionary.");

            var tempDict = new Dictionary<Enemy, int>();
            for (int i = 0; i < enemies.Count; i++)
            {
                if (tempDict.ContainsKey(enemies[i]))
                {
                    tempDict[enemies[i]] += enemiesAmount[i];
                    continue;
                }

                tempDict.Add(enemies[i], enemiesAmount[i]);
            }

            _internal = tempDict;
        }

        public Tuple<Enemy, int> GetEnemyAmounts(Enemy type)
        {
            if (_internal == default)
            {
                InitializeDictionary();
            }

            if (!_internal.ContainsKey(type))
                return new Tuple<Enemy, int>(type, 0);

            return new Tuple<Enemy, int>(type, _internal[type]);
        }

        public HashSet<Enemy> GetEnemyTypes()
        {
            if (_internal == default)
            {
                InitializeDictionary();
            }

            var temp = new HashSet<Enemy>();

            foreach (var item in _internal)
            {
                temp.Add(item.Key);
            }

            return temp;
        }
    }
}
