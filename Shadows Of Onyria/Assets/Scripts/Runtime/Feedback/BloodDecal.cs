using System;
using DoaT;
using UnityEngine;
using Random = UnityEngine.Random;

public class BloodDecal : MonoBehaviour, IPoolSpawn, IUpdate
{
    public float expandTimer;
    public float remainTimer;
    public float retractTimer;

    private Pool<BloodDecal> _parentPool;
    private readonly TimerHandler _aliveTimer = new TimerHandler();
    
    #pragma warning disable 649
    [SerializeField] private Texture[] _textures;
    [SerializeField] private Renderer _rend;
    #pragma warning restore 649
    
    private static readonly int Decal = Shader.PropertyToID("_Decal");
    private static readonly int ObjectScale = Shader.PropertyToID("_ObjectScale");
    private static readonly int CurrentValue = Shader.PropertyToID("_CurrentValue");
    private static readonly int PositionX = Shader.PropertyToID("_PositionX");
    private static readonly int PositionY = Shader.PropertyToID("_PositionY");
    private static readonly int PositionZ = Shader.PropertyToID("_PositionZ");

    public void Initialize()
    {
        TimerManager.SetTimer(_aliveTimer, Deactivate, expandTimer + remainTimer + retractTimer);
    }
    
    public void OnUpdate()
    {
        if (_aliveTimer.Elapsed <= expandTimer)
        {
            _rend.material.SetFloat(CurrentValue, _aliveTimer.Elapsed / expandTimer);
            return;
        }
        
        if (_aliveTimer.Elapsed >= expandTimer+remainTimer)
        {
            _rend.material.SetFloat(CurrentValue, 1-(_aliveTimer.Elapsed - expandTimer - remainTimer) / retractTimer);
        }
    }

    public void SetParentPool<T>(T parent)
    {
        _parentPool = parent as Pool<BloodDecal>;
    }

    public void Activate(Vector3 position, Quaternion rotation)
    {
        _rend.gameObject.SetActive(true);
        
        var temp = transform;
        temp.position = position;
        temp.rotation = rotation;
        ExecutionSystem.AddUpdate(this);
        
        var material = _rend.material;
        material.SetTexture(Decal, _textures[Random.Range(0, _textures.Length)]);
        material.SetFloat(ObjectScale, transform.lossyScale.GetMean());
        material.SetFloat(CurrentValue, 0);
        material.SetFloat(PositionX, position.x);
        material.SetFloat(PositionY, position.y);
        material.SetFloat(PositionZ, position.z);
    }

    public void Deactivate()
    {
        _rend.gameObject.SetActive(false);
        
        ExecutionSystem.RemoveUpdate(this, false);
        _parentPool.ReturnObject(this);
    }

    private void OnDestroy()
    {
        ExecutionSystem.RemoveUpdate(this, true);
    }
}
