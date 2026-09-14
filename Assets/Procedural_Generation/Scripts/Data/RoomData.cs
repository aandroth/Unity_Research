using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    [Serializable]
    public struct RoomItemData
    {
        public int minQuantity, maxQuantity;
        public ItemData itemData;
    }

    [SerializeField]
    public bool m_playerRoom = false;
    [SerializeField]
    public List<RoomItemData> m_itemDatas;

    [SerializeField]
    private ItemPlacementHelper prefabPlacer;

    private List<(ItemData, int)> m_itemsToPlace = new List<(ItemData, int)>();

    

    public void GenerateItemQuantities()
    {
        m_itemsToPlace = new List<(ItemData, int)>();
        foreach (RoomItemData itemData in m_itemDatas)
        {
            int quantity = UnityEngine.Random.Range(itemData.minQuantity, itemData.maxQuantity + 1);
            if (quantity > 0)
                m_itemsToPlace.Add((itemData.itemData, quantity));
        }
    }

    public void CreateItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridor)
    {
        prefabPlacer = new ItemPlacementHelper(roomFloor, roomFloorNoCorridor);
    }

    public List<GameObject> PlaceItems(GameObject itemPrefab)
    {
        List<GameObject> createdItemsList = new List<GameObject>();
        foreach (var item in m_itemsToPlace)
        {
            for (int i = 0; i < item.Item2; i++)
            {
                Vector2? placementPosition = prefabPlacer.GetItemPlacementPosition(item.Item1.placementType, item.Item2, item.Item1.size, item.Item1.addOffset);
                GameObject createdItem = null;
                if (placementPosition != null)
                {
                    createdItem = GameObject.Instantiate(itemPrefab, new Vector3(placementPosition.Value.x+0.5f, placementPosition.Value.y+0.5f, 0), Quaternion.identity);
                    createdItem.GetComponent<Item>().SetItemValues(item.Item1);
                    createdItemsList.Add(createdItem);
                }
            }
        }
        return createdItemsList;
    }
}
