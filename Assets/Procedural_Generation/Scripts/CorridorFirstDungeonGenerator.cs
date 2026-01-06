using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    // Rooms Dictionary
    Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    private Vector2Int prevDirection = new Vector2Int();
    private List<Vector2Int> possibleDeadEnds = new List<Vector2Int>();
    private List<GameObject> debugSpheres = new List<GameObject>();

    [SerializeField]
    private Vector2Int corridorLengthConstraints = new Vector2Int() {};
    [SerializeField]
    private Vector2Int corridorWidthConstraints = new Vector2Int() { };
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    [SerializeField]
    private RandomColorPicker m_randColorPicker;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        possibleDeadEnds = new List<Vector2Int>();
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        foreach (GameObject sphere in debugSpheres) DestroyImmediate(sphere);

        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        CreateRoomsAtDeadEnd(roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);

        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        ConsolidateRooms();
        ColorRooms();
    }

    private void CreateRoomsAtDeadEnd(HashSet<Vector2Int> roomFloors)
    {
        foreach (var deadEnd in possibleDeadEnds)
        {
            if (!roomFloors.Contains(deadEnd))
            {
                var roomFloor = RunRandomWalk(randomWalkParameters, deadEnd);
                roomFloors.UnionWith(roomFloor);
                roomsDictionary[deadEnd] = roomFloor;
            }
        }
    }

    private void ConsolidateRooms()
    {
        bool loopAgain = true;
        while (loopAgain)
        {
            loopAgain = false;
            foreach (var key1 in roomsDictionary.Keys)
            {
                List<Vector2Int> roomsToDelete = new List<Vector2Int>();
                foreach (var key2 in roomsDictionary.Keys)
                {
                    if (key1 != key2 && roomsDictionary[key1].Overlaps(roomsDictionary[key2]))
                    {
                        roomsDictionary[key1].UnionWith(roomsDictionary[key2]);
                        roomsToDelete.Add(key2);
                        loopAgain = true;
                    }
                }
                roomsToDelete.ForEach(key2 => roomsDictionary.Remove(key2));
                if (loopAgain) break;
            }
        }
    }

    private void ColorRooms()
    {
        foreach(var roomRootPosition in roomsDictionary.Keys)
        {
            Color roomColor = m_randColorPicker.GetRandomColor();
            Debug.Log($"Chosen color is {roomColor}");
            foreach (var position in roomsDictionary[roomRootPosition])
                tilemapVisualizer.AddColorToSingleFloorTile(position, roomColor);
        }
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (Vector2Int roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomsDictionary[roomPosition] = roomFloor;
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        possibleDeadEnds.Add(currentPosition);
        Vector2Int currDirection;

        for (int i = 0; i < corridorCount; i++)
        {
            int randomizedCorridorLength = UnityEngine.Random.Range(corridorLengthConstraints.x, corridorLengthConstraints.y);
            int randomizedCorridorWidth = UnityEngine.Random.Range(corridorWidthConstraints.x, corridorWidthConstraints.y);
            currDirection = Direction2D.GetRandomDirection();

            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, randomizedCorridorLength, randomizedCorridorWidth, currDirection, prevDirection, 2);
            prevDirection = currDirection;
            currentPosition = corridor[^1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
            if (currDirection.x == -prevDirection.x || currDirection.y == -prevDirection.y ||
                i == corridorCount - 1)
            {
                possibleDeadEnds.Add(currentPosition);
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
                debugSpheres.Add(go);
            }
        }
    }
}