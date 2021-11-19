using System;
using DoaT;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootDropper : MonoBehaviour
{
    public Loot loot;

    public SoulType zombie;
    public SoulType CrystalSkull;

    //TODO: Enable Different souls to be dropped
    //TODO: Expand loot Dropping algorithm
    //TODO: Create a more Modular Approach
    
    private void Awake() 
    {
        EventManager.Subscribe(GameEvents.SpawnZombieSoul, SpawnZS);
        EventManager.Subscribe(GameEvents.SpawnCrystalSkullSoul, SpawnCSS); 
    }

    private void SpawnZS(params object[] parameters)
    {
        if (Random.value >= 0.7f) return;
        
        var position = (Vector3) parameters[0];

        var temp = Instantiate(loot, transform);
        temp.type = zombie;
        temp.transform.position = position;
    }
    
    private void SpawnCSS(params object[] parameters)
    {
        if (Random.value >= 0.7f) return;
        var position = (Vector3) parameters[0];
        
        var temp = Instantiate(loot, transform);
        temp.type = CrystalSkull;
        temp.transform.position = position;
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(GameEvents.SpawnZombieSoul, SpawnZS);
        EventManager.Unsubscribe(GameEvents.SpawnCrystalSkullSoul, SpawnCSS); 
    }
}