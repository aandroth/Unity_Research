using UnityEngine;

public class Item : MonoBehaviour
{
    public SpriteRenderer m_spriteRenderer = null;
    public Vector2Int m_size = Vector2Int.one;
    public int m_health = 1;
    public bool m_nonDestructible = false;
    public void SetItemValues(ItemData itemData)
    {
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = itemData.sprite;
        m_size = itemData.size;
        m_health = itemData.health;
        m_nonDestructible = itemData.nonDestructible;
    }
}
