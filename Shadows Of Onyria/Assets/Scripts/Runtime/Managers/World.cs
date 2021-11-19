using System;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class World : MonoBehaviourInit
{
    public static World Current { get; private set; }
    public static string ZoneName => Current._zoneName;
    
    public static Vector3 ScreenForward { get; } = new Vector3(-1f, 0, 1f).normalized;
    public static Vector3 ScreenRight { get; } = new Vector3(1f, 0, 1f).normalized;
    
    public static ItemQualityUtility ItemQualityUtility { get; private set; }
    
    public static SpatialGrid SpatialGrid { get; } = new SpatialGrid();
    public static PlaneManager PlaneManager { get; private set; }
    public static GameSelectionManager GameSelectionManager { get; private set; }
    public static Camera MainCamera => Current._mainCamera;
    
    [SerializeField] private TheodenController _player;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private string _zoneName;
    
    private InteractableManager _interactableManager;

    public List<TimerHandler> currentDebugTimer = new List<TimerHandler>();
    
    private void Awake()
    {
        if (Current == null) Current = this;
        else if(Current != this) Destroy(this);

        if (_player == null) _player = FindObjectOfType<TheodenController>();
        if (_mainCamera == null) _mainCamera = Camera.main;
    }
    
    public override float OnInitialization()
    {
        ItemQualityUtility = Managers.GetManager<ItemQualityUtility>();
        GameSelectionManager = Managers.GetManager<GameSelectionManager>();
        
        _interactableManager = new InteractableManager();
        PlaneManager = new PlaneManager(_player);
        GameSelectionManager.Initialize(PlaneManager, _mainCamera);
        return 1f;
    }

    public static TheodenController GetPlayer()
    {
        return Current._player;
    }

    public static float DistanceToPlayer(IEntity entity)
    {
        var playerPos = Current._player.Position.SetY(0);
        var entityPos = entity.Position.SetY(0);

        return Vector3.Distance(playerPos, entityPos);
    }

    public static void Clear(params object[] parameters)
    {
        if(Current != null) Current._interactableManager.Dispose();
        
        ItemQualityUtility = null;
        if(GameSelectionManager != null) GameSelectionManager.Unload();
        GameSelectionManager = null;
        
        SpatialGrid.Clear();//TODO: REIMPLEMENT
        Managers.Clear();
        
        PlaneManager = null;
        TimerManager.Unload();

        Current = null;
    }
}
