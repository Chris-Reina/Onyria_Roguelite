#pragma warning disable CS0649
using System;
using DoaT.Attributes;
using UnityEngine;

namespace DoaT
{
    /// <summary>
    /// Model holds the data that the controller and view use.
    /// </summary>
    public class TheodenModel : MonoBehaviourInit
    {
        public SceneEvent onDeath;
        public TheodenData Data => _modifiedData.data;

        [Header("References")] 
        public Health health;
        public Darkness darkness;
        public new Rigidbody rigidbody;

        [Header("Data")] [SerializeField] private PlayerStreamedData _streamedData;
        [SerializeField] private TheodenModifiedData _modifiedData;
        [SerializeField] private TheodenBaseData _baseData;
        
        [Header("Temporal")]
        public float darknessPerSecond = 1f;

        private float _originalMaxHealth;
        private float _originalMaxDarkness;
        
        private void Awake()
        {
            if (_baseData == null)
                DebugManager.LogError("Player data not available.");
            else
                _modifiedData = new TheodenModifiedData(_baseData.data);

            health = GetComponent<Health>();

            if (onDeath != null)
            {
                health.OnDeath += () => EventManager.Raise(onDeath);
                health.OnDeath += () => PersistentData.RunGenerationManager.AssignExperience();
            }

            EventManager.Subscribe(UIEvents.TESTOnBodySoulAdded, AddModifier);
            EventManager.Subscribe(UIEvents.TESTOnBodySoulRemoved, RemoveModifier);
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
            EventManager.Subscribe(PlayerEvents.OnLevelConfirmation, UpdateStats);
            
            EventManager.Subscribe(ItemEvents.HealthPotion, HealthPotionImpl);
            EventManager.Subscribe(ItemEvents.DarknessPotion, DarknessPotionImpl);
            
            _originalMaxHealth = health.MaxHealth;
            _originalMaxDarkness = darkness.MaxDarkness;
        }

        public override float OnInitialization()
        {
            _streamedData.positionCallback += GetPosition;
            
            health.SetValue(PersistentData.Health.value);
            darkness.SetValue(PersistentData.Darkness.value);
            
            UpdateStats(null);
            return 1f;
        }

        private void AddModifier(params object[] parameters)
        {
            if (PersistentData.SoulInventory.GetAttributeModifier(out var aM))
            {
                _modifiedData.AddModifier(aM);
            }
        }

        private void RemoveModifier(params object[] parameters)
        {
            if (PersistentData.SoulInventory.GetAttributeModifier(out var aM))
            {
                _modifiedData.RemoveModifier(aM);
            }
        }

        public float GetProcessedDamage(float amount, DamageType type) => _modifiedData.GetDamage(amount, type);
        private Vector3 GetPosition() => transform.position;

        private void Unload(params object[] parameters)
        {
            PersistentData.Health.value = _originalMaxHealth * (health.CurrentValue / health.MaxHealth);
            PersistentData.Darkness.value = _originalMaxDarkness * (darkness.CurrentValue / darkness.MaxDarkness);
            _streamedData.positionCallback -= GetPosition;
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(UIEvents.TESTOnBodySoulAdded, AddModifier);
            EventManager.Unsubscribe(UIEvents.TESTOnBodySoulRemoved, RemoveModifier);
            EventManager.Unsubscribe(PlayerEvents.OnLevelConfirmation, UpdateStats);
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
            
            EventManager.Unsubscribe(ItemEvents.HealthPotion, HealthPotionImpl);
            EventManager.Unsubscribe(ItemEvents.DarknessPotion, DarknessPotionImpl);
        }

        private void UpdateStats(object[] objects)
        {
            var newAmountVitality = LevelAttributeManagementUtility.GetVitalityByLevel(PersistentData.LevelStats.Vitality-1);
            health.ChangeMaxHealth(_originalMaxHealth + newAmountVitality);
            PersistentData.Health.value = health.CurrentValue;
            
            var newAmountSpirit = LevelAttributeManagementUtility.GetSpiritByLevel(PersistentData.LevelStats.Spirit-1);
            darkness.ChangeMaxDarkness(_originalMaxDarkness + newAmountSpirit);
            PersistentData.Darkness.value = darkness.CurrentValue;
            
            var newAmountStrength = LevelAttributeManagementUtility.GetStrengthByLevel(PersistentData.LevelStats.Strength-1);
            _modifiedData.data.baseDamage = _baseData.data.baseDamage + newAmountStrength;
            
            var (item1, item2) = LevelAttributeManagementUtility.GetAgilityByLevel(PersistentData.LevelStats.Agility-1);
            _modifiedData.data.locomotion.movementSpeed = _baseData.data.locomotion.movementSpeed + item1;
            _modifiedData.data.dash.dashDistance = _baseData.data.dash.dashDistance + item2;
        }
        
        
        private void HealthPotionImpl(object[] obj)
        {
            var amount = (int) obj[0];
            health.Heal(amount);
        }
        private void DarknessPotionImpl(object[] obj)
        {
            var amount = (int) obj[0];
            darkness.Gain(amount);
        }
    }
}
#pragma warning restore CS0649
