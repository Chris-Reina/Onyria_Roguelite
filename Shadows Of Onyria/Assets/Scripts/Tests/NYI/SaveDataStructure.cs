using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.Save
{
    [Serializable]
    public class SaveDataStructure
    {
        public SaveMetaData metaData = new SaveMetaData();
        public PlayerDataStructure player = new PlayerDataStructure();
        public GameFlagsDataStructure gameFlags = new GameFlagsDataStructure();
        public SoulInventoryDataStructure soulInventory = new SoulInventoryDataStructure();
        
        public SaveDataStructure(){}
        public SaveDataStructure(SaveMetaData previousData)
        {
            metaData = previousData.Clone();
        }
    }

    [Serializable]
    public class SaveMetaData : IClone<SaveMetaData>
    {
        private string _gameZone;
        private DateTime _saveTime;
        private int _characterLevel;
        private float _playedSeconds;
        
        public string GameZone => _gameZone;
        public DateTime SaveTime => _saveTime;
        public int CharacterLevel => _characterLevel;
        public float PlayedTime => _playedSeconds;

        public SaveMetaData()
        {
            _saveTime = DateTime.Now;
        }
        
        public SaveMetaData SetZone(string zone)
        {
            _gameZone = zone;
            return this;
        }
        
        public SaveMetaData SetSaveTime(DateTime saveTime)
        {
            _saveTime = saveTime;
            return this;
        }
        
        public SaveMetaData SetLevel(int level)
        {
            _characterLevel = level;
            return this;
        }
        
        public SaveMetaData SetPlayedTime(float playedTime)
        {
            _playedSeconds = playedTime;
            return this;
        }
        

        public SaveMetaData Clone()
        {
            return new SaveMetaData
            {
                _gameZone = this._gameZone,
                _saveTime = this._saveTime,
                _characterLevel = this._characterLevel,
                _playedSeconds = this._playedSeconds
            };
        }
    }

    [Serializable]
    public class SoulInventoryDataStructure : ISaveDataStructure<SoulInventoryData>
    {
        [Serializable]
        public class SoulToken
        {
            public string type;
            public int level;
            public int slotID;

            public SoulToken(Soul data)
            {
                type = data.type.identifier;
                level = data.level;
                slotID = (int) data.slotID;
            }

            public Soul ToSoul(Dictionary<string, SoulType> types)
            {
                return new Soul(level, types[type], slotID);
            }
        }

        public SoulToken dashSlot;
        public SoulToken mainAttackSlot;
        public SoulToken rangeAttackSlot;
        public SoulToken bodySlot;

        public void SaveFromAsset(SoulInventoryData input)
        {
            dashSlot = new SoulToken(input.dashSlotSoul);
            mainAttackSlot = new SoulToken(input.mainAttackSlotSoul);
            rangeAttackSlot = new SoulToken(input.rangeAttackSlotSoul);
            bodySlot = new SoulToken(input.bodySlotSoul);
        }

        public void LoadIntoAsset(SoulInventoryData output)
        {
            var soulTypesDict = Resources.LoadAll<SoulType>("").Aggregate(new Dictionary<string, SoulType>(),
                (dict, type) =>
                {
                    dict.Add(type.identifier, type);
                    return dict;
                });

            output.dashSlotSoul = dashSlot.ToSoul(soulTypesDict);
            output.mainAttackSlotSoul = mainAttackSlot.ToSoul(soulTypesDict);
            output.rangeAttackSlotSoul = rangeAttackSlot.ToSoul(soulTypesDict);
            output.bodySlotSoul = bodySlot.ToSoul(soulTypesDict);
        }
    }
    
    [Serializable]
    public class PlayerDataStructure : ISaveDataStructure<PersistentPlayerData>
    {
        [Serializable]
        public class PlayerToken
        {
            public int level;
            public int experience;
            public List<int> stats;

            public PlayerToken(PersistentPlayerData data)
            {
                level = data.Level;
                experience = data.Experience;
                stats = GetIntList(data.LevelStats);
            }

            private List<int> GetIntList(PersistentStats stats)
            {
                return new List<int>
                {
                    stats.Vitality,
                    stats.Spirit,
                    stats.Strength,
                    stats.Agility,
                    stats.Intellect,
                    stats.Endurance,
                    stats.Adaptability
                };
            }

            public void SetStatValues(PersistentStats stats)
            {
                stats.Vitality = this.stats[0];
                stats.Spirit = this.stats[1];
                stats.Strength = this.stats[2];
                stats.Agility = this.stats[3];
                stats.Intellect = this.stats[4];
                stats.Endurance = this.stats[5];
                stats.Adaptability = this.stats[6];
            }
        }

        public PlayerToken data;

        public void SaveFromAsset(PersistentPlayerData input)
        {
            data = new PlayerToken(input);
        }

        public void LoadIntoAsset(PersistentPlayerData output)
        {
            output.Level = data.level;
            output.Experience = data.experience;
            data.SetStatValues(output.LevelStats);
        }
    }

    [Serializable]
    public class GameFlagsDataStructure : ISaveDataStructure<GameState>
    {
        [Serializable]
        public class GameFlagsToken
        {
            public bool hasSword;
            public bool soulAltarDiscovered;
            public bool soulAltarDoorOpened;

            public GameFlagsToken(bool hasSword, bool soulAltarDiscovered, bool soulAltarDoorOpened)
            {
                this.hasSword = hasSword;
                this.soulAltarDiscovered = soulAltarDiscovered;
                this.soulAltarDoorOpened = soulAltarDoorOpened;
            }
        }

        public GameFlagsToken data;
        
        public void SaveFromAsset(GameState input)
        {
            data = new GameFlagsToken(input.HasSword.Value, input.SoulAltarDiscovered.Value, input.SoulAltarDoorOpened.Value);
        }

        public void LoadIntoAsset(GameState output)
        {
            output.HasSword.Value = data.hasSword;
            output.SoulAltarDiscovered.Value = data.soulAltarDiscovered;
            output.SoulAltarDoorOpened.Value = data.soulAltarDoorOpened;
        }
    }


    [Serializable]
    public class SerializableDataDictionary<TKey, TValue>
    {
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();
    }

}