using System.Collections;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    private static RewardSystem Current { get; set; }
    
    private void Awake()
    {
        if (Current == null)
            Current = this;
        else
        {
            Destroy(this);
            return;
        }
    }

    public static void Gold(int amount) => Current.GoldImpl(amount);
    private void GoldImpl(int amount)
    {
        PersistentData.ItemInventory.data.Gold += amount;
    }
    
    public static void Experience(int amount) => Current.ExperienceImpl(amount);
    private void ExperienceImpl(int amount)
    {
        PersistentData.Player.Experience += amount;
    }
    
    public static void SoulExperience(SoulType type, int amount) => Current.SoulExperienceImpl(type, amount);
    private void SoulExperienceImpl(SoulType type, int amount)
    {
        //TODO: Implement
    }
}
