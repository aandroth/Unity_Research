using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BoxGenerator : MonoBehaviour
{
    public GameObject boxPrefab;
    public List<GameObject> boxList = null;
    public int areaWidth, areaLength;
    public float minHeight, maxHeight;


    public void Start()
    {
        GenerateBoxes();
    }
    public void GenerateBoxes()
    {
        PerlinNoiseGeneration perlinNoise = new PerlinNoiseGeneration();
        ClearBoxes();
        for (int i = 0; i < areaWidth; i++)
        {
            for (int j = 0; j < areaLength; j++)
            {
                float val = 0;

                float freq = 1;
                float amp = 1;

                for (int k = 0; k < 4; k++)
                {
                    // val += Mathf.PerlinNoise(i * freq, j * freq) * amp;
                    val += perlinNoise.Perlin(i * freq / areaWidth, j * freq / areaLength) * amp;
                    freq *= 2;
                    amp *= 0.5f;
                }
                val *= 1.2f;

                // Clipping
                if (val > 1) val = 1;
                if (val < -1) val = -1;


                //float randHeight = Random.Range(minHeight, maxHeight);
                Vector3 randomPosition = new Vector3(i, (int)(maxHeight * val), j);
                GameObject box = Instantiate(boxPrefab, randomPosition, Quaternion.identity);
                boxList.Add(box);
            }
        }
    }

    private void ClearBoxes()
    {
        if (boxList != null)
        {
            foreach (GameObject box in boxList)
            {
                Destroy(box);
            }
            boxList.Clear();
        }
    }
}