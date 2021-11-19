using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

public static class LayersUtility
{
    //Simple Layer INDEX
    public const int INVISIBLE_WALL_MASK_INDEX = 23;
    public const int RAGDOLL_MASK_INDEX = 24;
    public const int PLAYER_TRIGGERS_MASK_INDEX = 25;
    public const int PLAYER_WALK_CHECK_MASK_INDEX = 26;//= 11;
    public const int PLAYER_MASK_INDEX = 27;// = 8;
    public const int WALL_MASK_INDEX = 28;//= 10;
    public const int ENTITY_MASK_INDEX = 29;//= 29;
    public const int NODE_MASK_INDEX = 30;// = 9;
    public const int TRAVERSABLE_MASK_INDEX = 31;//= 31;
    
    //Simple Layer Masks
    public const int INVISIBLE_WALL_MASK = 1 << INVISIBLE_WALL_MASK_INDEX;
    public const int RAGDOLL_MASK = 1 << RAGDOLL_MASK_INDEX;
    public const int PLAYER_TRIGGERS_MASK = 1 << PLAYER_TRIGGERS_MASK_INDEX;
    public const int PLAYER_WALK_CHECK_MASK = 1 << PLAYER_WALK_CHECK_MASK_INDEX;
    public const int PLAYER_MASK = 1 << PLAYER_MASK_INDEX;
    public const int WALL_MASK = 1 << WALL_MASK_INDEX;
    public const int ENTITY_MASK = 1 << ENTITY_MASK_INDEX;
    public const int NODE_MASK = 1 << NODE_MASK_INDEX;
    public const int TRAVERSABLE_MASK = 1 << TRAVERSABLE_MASK_INDEX;
    public const int ALL_MASK = ~0;
    public const int NONE_MASK = 0;

    //Compound Layer Masks
    public const int CURSOR_SELECTOR_MASK = TRAVERSABLE_MASK | ENTITY_MASK;
    public const int NODE_NEIGHBOUR_MASK = WALL_MASK | NODE_MASK | INVISIBLE_WALL_MASK;
    public const int WALL_DETECTION_MASK = WALL_MASK | INVISIBLE_WALL_MASK;
    public const int PLAYER_DETECTION_SIGHT_MASK = WALL_MASK | PLAYER_MASK;
    public const int PLAYER_DETECTION_MOVEMENT_MASK = WALL_MASK | PLAYER_MASK | INVISIBLE_WALL_MASK;

    public static bool IsInMask(int mask, int layerIndex)
    {
        return (mask & (1 << layerIndex)) != 0;
    }
    
    public static bool IsInMask(LayerMask layerMask, int layerIndex)
    {
        return (layerMask & (1 << layerIndex)) != 0;
    }

    public static bool ContainsLayer(this LayerMask mask, int layerIndex)
    {
        return (mask & (1 << layerIndex)) != 0;
    }

}