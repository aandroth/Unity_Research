using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemPlacementHelper
{
    public enum PlacementType {OpenSpace, NearWall}

    Dictionary<PlacementType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();
    HashSet<Vector2Int> roomFloorNoCorridor;

    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, 
                               HashSet<Vector2Int> roomFloorNoCorridor)
    {
        Graph graph = new Graph(roomFloor);
        this.roomFloorNoCorridor = roomFloorNoCorridor;

        foreach (Vector2Int pos in roomFloor)
        {
            int neighborsCount8Dir = graph.GetNeighbors8Directions(pos).Count;
            PlacementType placementType = neighborsCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;
            
            if(!tileByType.ContainsKey(placementType))
                tileByType[placementType] = new HashSet<Vector2Int>();

            if (placementType == PlacementType.NearWall && graph.GetNeighbors4Directions(pos).Count < 3)
                continue;
            tileByType[placementType].Add(pos);
        }
    }

    public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector2Int size, bool addOffset)
    {
        int itemArea = size.x * size.y;
        if (tileByType[placementType].Count < itemArea)
            return null;

        int iteration = 0;
        while (iteration < iterationsMax)
        {
            iteration++;
            int index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
            Vector2Int randomPos = tileByType[placementType].ElementAt(index);

            if(itemArea > 1)
            {
                var (result, placementPositions) = PlaceBigItem(randomPos, size, addOffset);

                if(!result)
                    continue;

                tileByType[placementType].ExceptWith(placementPositions);
                tileByType[PlacementType.NearWall].ExceptWith(placementPositions);
            }
            else
            {
                tileByType[placementType].Remove(randomPos);
            }

            return randomPos;
        }
        return null;
    }

    private (bool, List<Vector2Int>) PlaceBigItem(Vector2Int originalPos, Vector2Int size, bool addOffset)
    {
        List<Vector2Int> placementPositions = new List<Vector2Int>() {originalPos};
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxY = addOffset ? size.y + 1 : size.y;
        int minX = addOffset ? -1 : 0;
        int minY = addOffset ? -1 : 0;

        for (int row = minX; row <= maxX; row++)
        {
            for (int col = minY; col <= maxY; col++)
            {
                if (col == 0 && row == 0) continue;

                Vector2Int newPosToCheck = new Vector2Int(originalPos.x + row, originalPos.y + col);
                if (!roomFloorNoCorridor.Contains(newPosToCheck)) return (false, placementPositions);

                placementPositions.Add(newPosToCheck);
            }
        }
        return (true, placementPositions);
    }
}
