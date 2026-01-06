using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorPicker : MonoBehaviour
{
    private List<Color> m_masterColorList = new List<Color>
    {
        Color.blue,
        Color.green,
        Color.red,
        Color.yellow,
        new Color(1, 0.3f, 0.3f), // Pink
        new Color( 0.2f, 1, 1), // Teal
        new Color(1,   0, 1)  // Purple
    };
    private List<Color> m_editableColorList = new List<Color>();

    public Color GetRandomColor()
    {
        if (m_editableColorList.Count == 0) m_editableColorList = new List<Color>(m_masterColorList);

        int idx = Random.Range(0, m_editableColorList.Count);
        Color chosenColor = m_editableColorList[idx];
        m_editableColorList.RemoveAt(idx);
        return chosenColor;
    }
}
