using UnityEngine;
using System.Collections.Generic;

public class Graph : MonoBehaviour
{
    // Four-directional offsets (cardinal directions: up, down, left, right)
    private static readonly List<Vector2Int> FourDirections = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   // Up (North)
        new Vector2Int(0, -1),  // Down (South)
        new Vector2Int(-1, 0),  // Left (West)
        new Vector2Int(1, 0)    // Right (East)
    };
    
    // Eight-directional offsets (cardinal + diagonal directions)
    private static readonly List<Vector2Int> EightDirections = new List<Vector2Int>
    {
        new Vector2Int(0, 1),    // Up (North)
        new Vector2Int(0, -1),   // Down (South)
        new Vector2Int(-1, 0),   // Left (West)
        new Vector2Int(1, 0),   // Right (East)
        new Vector2Int(-1, 1),   // Up-Left (Northwest)
        new Vector2Int(1, 1),   // Up-Right (Northeast)
        new Vector2Int(-1, -1), // Down-Left (Southwest)
        new Vector2Int(1, -1)   // Down-Right (Southeast)
    };

    /// <summary>
    /// Gets the four-directional neighbors (up, down, left, right) of a given cell position.
    /// </summary>
    /// <param name="cell">The cell position to get neighbors for</param>
    /// <returns>List of Vector2Int representing the four cardinal neighbors</returns>
    public static List<Vector2Int> GetNeighbors4Directions(Vector2Int cell)
    {
        return GetNeighbors(cell, FourDirections);
    }
    
    /// <summary>
    /// Gets the eight-directional neighbors (including diagonals) of a given cell position.
    /// </summary>
    /// <param name="cell">The cell position to get neighbors for</param>
    /// <returns>List of Vector2Int representing all eight neighbors</returns>
    public static List<Vector2Int> GetNeighbors8Directions(Vector2Int cell)
    {
        return GetNeighbors(cell, EightDirections);
    }
    
    /// <summary>
    /// Helper method to get the neighbors of a specific Vector2Int location.
    /// </summary>
    /// <param name="startPosition">Starting position to check neighbors from</param>
    /// <param name="neighborsOffsetList">List of direction offsets to apply</param>
    /// <returns>List of Vector2Int representing the neighbors</returns>
    private static List<Vector2Int> GetNeighbors(Vector2Int startPosition, List<Vector2Int> neighborsOffsetList)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int offset in neighborsOffsetList)
        {
            Vector2Int neighbor = startPosition + offset;
            neighbors.Add(neighbor);
        }

        return neighbors;
    }
}
