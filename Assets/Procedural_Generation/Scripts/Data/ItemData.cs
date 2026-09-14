using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class ItemData : ScriptableObject
{
    public Sprite sprite = null;
    public Vector2Int size = Vector2Int.one;
    public ItemPlacementHelper.PlacementType placementType = ItemPlacementHelper.PlacementType.NearWall;
    public bool addOffset = true;
    public int health = 4;
    public bool nonDestructible = false;
}
