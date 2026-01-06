using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class ItemData : ScriptableObject
{
    public Sprite sprite = null;
    public Vector2Int size = Vector2Int.one;
    public PlacementType placementType = PlacementType.NEAR_WALL;
    public bool addOffset = true;
    public int health = 4;
    public bool nonDestructible = false;
}
