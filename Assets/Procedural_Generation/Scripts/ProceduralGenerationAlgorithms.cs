using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; ++i)
        {
            var newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength, int corridorWidth, Vector2Int direction, Vector2Int prevDirection = new Vector2Int(), int prevWidth = 1)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        bool isVerticalCorridor = direction.y != 0;
        Vector2Int extraWidthDirection = isVerticalCorridor ? new Vector2Int(1, 0) : new Vector2Int(0, 1);
        if(prevDirection.y != 0 && direction.x == -1) startPosition.x += prevWidth-1;
        if(prevDirection.y == 0 && direction.y == -1) startPosition.y += prevWidth-1;
        var currentPosition = startPosition;
        corridor.Add(currentPosition);
        for(int i=1; i<corridorWidth; ++i)
            corridor.Add(new Vector2Int(currentPosition.x + extraWidthDirection.x*i, currentPosition.y + extraWidthDirection.y * i));

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
            for (int j = 1; j < corridorWidth; ++j)
                corridor.Add(new Vector2Int(currentPosition.x + extraWidthDirection.x * j, currentPosition.y + extraWidthDirection.y * j));
        }
        return corridor;
    }

}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(0, -1),//DOWN
        new Vector2Int(-1, 0) //LEFT
    };
    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, -1), // DOWN-RIGHT
        new Vector2Int(-1, -1),//DOWN-LEFT
        new Vector2Int(-1, 1) //UP-LEFT
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), // UP
        new Vector2Int(1, 1), // UP-RIGHT
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(1, -1), // DOWN-RIGHT
        new Vector2Int(0, -1),//DOWN
        new Vector2Int(-1, -1),//DOWN-LEFT
        new Vector2Int(-1, 0), //LEFT
        new Vector2Int(-1, 1) //UP-LEFT
    };

    public static Vector2Int GetRandomDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}