using System.Collections.Generic;
using UnityEngine;

public class ItemPlacementHelper
{
    public enum PlacementType {NEAR_WALL, IN_CENTER}

    Dictionary<PlacementType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();

    HashSet<Vector2Int> roomFloorNoCorridor;

    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridor)
    {

    }
}
