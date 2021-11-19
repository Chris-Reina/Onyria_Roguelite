using System;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class UILevelUpManager : MonoBehaviour
{
    private class MutableValuePair
    {
        public int concrete;
        public int mutated;

        public MutableValuePair(int initialValue)
        {
            concrete = initialValue;
            mutated = initialValue;
        }

        public void UpdateValues(int value)
        {
            concrete = value;
            mutated = value;
        }
        public void UpdateValues(int concrete, int mutated)
        {
            this.concrete = concrete;
            this.mutated = mutated;
        }

        public void UpdateConcreteValue(int concreteValue)
        {
            concrete = concreteValue;
        }
        
        public void UpdateMutatedValue(int mutatedValue)
        {
            mutated = mutatedValue;
        }
    }
    
    [Header("Info")] 
    [SerializeField] private UIDualInformationHandler _level;
    [SerializeField] private UIDualInformationHandler _experience;
    [SerializeField] private UISingleInformationHandler _experienceRequired;

    [Header("Attributes")] 
    [SerializeField] private UIAttributeHandler _vitality; //Max Health
    [SerializeField] private UIAttributeHandler _spirit; //Max Darkness
    [SerializeField] private UIAttributeHandler _strength; //Damage Main Attack
    [SerializeField] private UIAttributeHandler _agility; //Movement Speed / Dash Distance
    [SerializeField] private UIAttributeHandler _intellect; //Damage Range Attack
    [SerializeField] private UIAttributeHandler _endurance; //Less Damage taken
    [SerializeField] private UIAttributeHandler _adaptability; //Less Darkness Usage

    private Dictionary<UIAttributeHandler, MutableValuePair> _attributeMap;
    private Dictionary<UIInformationHandler, MutableValuePair> _informationMap;
    private CanvasGroupController _parent;
    
    private void Awake()
    {
        _informationMap = new Dictionary<UIInformationHandler, MutableValuePair>
        {
            {_level, new MutableValuePair(0)},
            {_experience, new MutableValuePair(0)},
            {_experienceRequired, new MutableValuePair(0)},
        };
        
        _attributeMap = new Dictionary<UIAttributeHandler, MutableValuePair>
        {
            {_vitality, new MutableValuePair(0)},
            {_spirit, new MutableValuePair(0)},
            {_strength, new MutableValuePair(0)},
            {_agility, new MutableValuePair(0)},
            {_intellect, new MutableValuePair(0)},
            {_endurance, new MutableValuePair(0)},
            {_adaptability, new MutableValuePair(0)},
        };
        
        var parent = FindObjectOfType<UILevelModificationManager>();

        parent.OnShowUI += SetupDisplays;
        parent.OnHideUI += () => EventManager.Raise(PlayerEvents.OnLevelConfirmation);

        _parent = GetComponentInParent<CanvasGroupController>();
// #if UNITY_EDITOR
//         parent.OnShowUI += () => DebugManager.Log("On Show UI Correctly Called.");
//         parent.OnHideUI += () => DebugManager.Log("On Hide UI Correctly Called.");
// #endif
    }

    private void Start()
    {
        SetupDisplays();

        _vitality.OnAdd += () => TryModifyAttribute(_vitality);
        _vitality.OnRemove += () => TryRestoreAttribute(_vitality);

        _spirit.OnAdd += () => TryModifyAttribute(_spirit);
        _spirit.OnRemove += () => TryRestoreAttribute(_spirit);

        _strength.OnAdd += () => TryModifyAttribute(_strength);
        _strength.OnRemove += () => TryRestoreAttribute(_strength);

        _agility.OnAdd += () => TryModifyAttribute(_agility);
        _agility.OnRemove += () => TryRestoreAttribute(_agility);

        _intellect.OnAdd += () => TryModifyAttribute(_intellect);
        _intellect.OnRemove += () => TryRestoreAttribute(_intellect);

        _endurance.OnAdd += () => TryModifyAttribute(_endurance);
        _endurance.OnRemove += () => TryRestoreAttribute(_endurance);

        _adaptability.OnAdd += () => TryModifyAttribute(_adaptability);
        _adaptability.OnRemove += () => TryRestoreAttribute(_adaptability);
    }

    private void SetupDisplays()
    {
        var level = PersistentData.Level;
        var experience = PersistentData.Experience;

        var expReqAmount = LevelAttributeManagementUtility.GetExperienceRequirement(level + 1);
        
        _level.Setup("Level", level);
        _experience.Setup("Experience", experience);
        _experienceRequired.Setup("Experience Required", expReqAmount);

        var attributes = PersistentData.LevelStats;
        
        _vitality.Setup("Vitality", attributes.Vitality, attributes.Vitality);
        _spirit.Setup("Spirit", attributes.Spirit, attributes.Spirit);
        _strength.Setup("Strength", attributes.Strength, attributes.Strength);
        _agility.Setup("Agility", attributes.Agility, attributes.Agility);
        _intellect.Setup("Intellect", attributes.Intellect, attributes.Intellect);
        _endurance.Setup("Endurance", attributes.Endurance, attributes.Endurance);
        _adaptability.Setup("Adaptability", attributes.Adaptability, attributes.Adaptability);

        foreach (var kvp in _informationMap)
        {
            switch (kvp.Key)
            {
                case UISingleInformationHandler single:
                    kvp.Value.UpdateValues(single.GetValue());
                    break;
                case UIDualInformationHandler dual:
                    kvp.Value.UpdateValues(dual.GetValue(), dual.GetPossibleValue());
                    break;
            }
        }
        
        foreach (var kvp in _attributeMap)
        {
            kvp.Value.UpdateValues(kvp.Key.GetCurrentValue(), kvp.Key.GetPossibleValue());
        }
    }
    
    public void ConfirmChanges()
    {
        MutableToConcrete();

        var attributes = PersistentData.LevelStats;

        PersistentData.Player.Level = _level.GetValue();
        PersistentData.Player.Experience = _experience.GetValue();
        
        attributes.Vitality = _vitality.GetCurrentValue();
        attributes.Spirit = _spirit.GetCurrentValue();
        attributes.Strength = _strength.GetCurrentValue();
        attributes.Agility = _agility.GetCurrentValue();
        attributes.Intellect = _intellect.GetCurrentValue();
        attributes.Endurance = _endurance.GetCurrentValue();
        attributes.Adaptability = _adaptability.GetCurrentValue();
        
        EventManager.Raise(PlayerEvents.OnLevelConfirmation);
        _parent.HideUI();
    }

    private void MutableToConcrete()
    {
        foreach (var kvp in _informationMap)
        {
            switch (kvp.Key)
            {
                case UISingleInformationHandler single:
                    kvp.Value.UpdateValues(kvp.Value.mutated);
                    single.SetValue(kvp.Value.concrete);
                    break;
                case UIDualInformationHandler dual:
                    kvp.Value.UpdateValues(kvp.Value.mutated);
                    dual.SetValue(kvp.Value.concrete);
                    dual.SetPossibleValue(kvp.Value.concrete);
                    break;
            }
        }
        
        foreach (var kvp in _attributeMap)
        {
            kvp.Value.UpdateValues(kvp.Value.mutated);
            kvp.Key.SetValues(kvp.Value.concrete, kvp.Value.concrete);
        }
    }

    private void TryModifyAttribute(UIAttributeHandler attributeHandler)
    {
        var pair = _attributeMap[attributeHandler];

        var newLevel = _informationMap[_level].mutated + 1;
        if (!LevelAttributeManagementUtility.CanLevelUp(_informationMap[_experience].mutated, newLevel)) 
            return;
        
        var expNeeded = LevelAttributeManagementUtility.GetExperienceRequirement(newLevel);
        var nextExpNeeded = LevelAttributeManagementUtility.GetExperienceRequirement(newLevel + 1);
        var newExperience = _informationMap[_experience].mutated - expNeeded;
        
        _informationMap[_experienceRequired].mutated = nextExpNeeded;
        _informationMap[_experience].mutated = newExperience;
        _informationMap[_level].mutated = newLevel;

        pair.mutated += 1;
        AssignMutatedToDisplay(attributeHandler, pair);
    }
    
    private void TryRestoreAttribute(UIAttributeHandler attributeHandler)
    {
        var pair = _attributeMap[attributeHandler];

        if (pair.concrete == pair.mutated) return;

        var expRequired = LevelAttributeManagementUtility.GetExperienceRequirement(_informationMap[_level].mutated);

        _informationMap[_experienceRequired].mutated = expRequired;
        _informationMap[_experience].mutated += expRequired;
        _informationMap[_level].mutated -= 1;
        
        pair.mutated -= 1;

        AssignMutatedToDisplay(attributeHandler, pair);
    }

    private void AssignMutatedToDisplay(UIAttributeHandler attributeHandler, MutableValuePair pair)
    {
        foreach (var kvp in _informationMap)
        {
            switch (kvp.Key)
            {
                case UISingleInformationHandler single:
                    single.SetValue(kvp.Value.mutated);
                    break;
                case UIDualInformationHandler dual:
                    dual.SetValue(kvp.Value.concrete);
                    dual.SetPossibleValue(kvp.Value.mutated);
                    break;
            }
        }
        
        attributeHandler.SetValues(pair.concrete, pair.mutated);
    }
}

public static class LevelAttributeManagementUtility
{
    public static int GetExperienceRequirement(int level)
    {
        return 120 * (level - 1);
    }

    public static bool CanLevelUp(int totalExperience, int desiredLevel)
    {
        return totalExperience - GetExperienceRequirement(desiredLevel) > 0;
    }

    public static float GetVitalityByLevel(int level)
    {
        return level * 10f;
    }
    public static float GetSpiritByLevel(int level)
    {
        return level * 15f;
    }
    public static FloatRange GetStrengthByLevel(int level)
    {
        return new FloatRange(1 * level, 3 * level);
    }
    public static Tuple<float,float> GetAgilityByLevel(int level)
    {
        return new Tuple<float, float>(level * 0.2f, level * 0.1f);
    }
    public static float GetIntellectByLevel(int level)
    {
        return level * 10f;
    
    }
    public static float GetEnduranceByLevel(int level)
    {
        return level * 10f;
    }
    public static float GetAdaptabilityByLevel(int level)
    {
        return level * 10f;
    }
}