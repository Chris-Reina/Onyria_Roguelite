using System;
using DoaT.Vendor;
using UnityEngine;

namespace DoaT
{
    public class PersistentData : MonoBehaviour
    {
        private static PersistentData Current { get; set; }

        [SerializeField] private PersistentPlayerData _player = new PersistentPlayerData();

        [SerializeField] private ItemInventory _itemInventory;
        [SerializeField] private SoulInventory _soulInventory;
        [SerializeField] private RunGenerationManager _runGenerationManager;
        //[SerializeField] private DamageTypeProfile _damageTypes;

        public static PersistentPlayerData Player => Current._player;
        public static PersistentValue Health => Current._player.Health;
        public static PersistentValue Darkness => Current._player.Darkness;
        public static PersistentStats LevelStats => Current._player.LevelStats;
        public static int Experience => Current._player.Experience;
        public static int Level => Current._player.Level;

        public static ItemInventory ItemInventory => Current._itemInventory;
        public static SoulInventory SoulInventory => Current._soulInventory;
        public static RunGenerationManager RunGenerationManager => Current._runGenerationManager;
        //public static DamageTypeProfile DamageTypes => Current._damageTypes;
        
        private void Awake()
        {
            if (Current == null)
                Current = this;
            else if (Current != this)
            {
                DebugManager.LogWarning($"Duplicate Singleton of type {GetType()}");
                Destroy(this);
                return;
            }
            
            if (_soulInventory == null) _soulInventory = GetComponent<SoulInventory>();
            if (_runGenerationManager == null) _runGenerationManager = GetComponent<RunGenerationManager>();
        }
    }

    [Serializable]
    public class PersistentPlayerData
    {
        [SerializeField] private PersistentStats _levelStats;
        [SerializeField] private PersistentValue _health = new PersistentValue();
        [SerializeField] private PersistentValue _darkness = new PersistentValue();
        [SerializeField] private int _experience = 0;
        [SerializeField] private int _level = 1;

        public PersistentValue Health { get => _health; set => _health = value; }
        public PersistentValue Darkness { get => _darkness; set => _darkness = value; }
        public int Experience { get => _experience; set => _experience = value; }
        public int Level { get => _level; set => _level = value; }
        public PersistentStats LevelStats { get => _levelStats; set => _levelStats = value; }
    }

    [Serializable]
    public class PersistentStats
    {
        [SerializeField] private int _vitality; //Max Health
        [SerializeField] private int _spirit; //Max Darkness
        [SerializeField] private int _strength; //Damage Main Attack
        [SerializeField] private int _agility; //Movement Speed / Dash Distance
        [SerializeField] private int _intellect; //Damage Range Attack
        [SerializeField] private int _endurance; //Less Damage taken
        [SerializeField] private int _adaptability; //Less Darkness Usage
        
        public int Vitality { get => _vitality; set => _vitality = value; } 
        public int Spirit { get => _spirit; set => _spirit = value; } 
        public int Strength { get => _strength; set => _strength = value; } 
        public int Agility { get => _agility; set => _agility = value; } 
        public int Intellect { get => _intellect; set => _intellect = value; } 
        public int Endurance { get => _endurance; set => _endurance = value; } 
        public int Adaptability { get => _adaptability; set => _adaptability = value; } 
    }
}
