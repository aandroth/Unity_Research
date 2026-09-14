using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PerlinNoiseGeneration
{
    public Vector2 RandomGradient(int ix, int iy)
    {
        // Random float. No precomputed gradients mean this works for any number of grid coordinates
        float random = 2920f * Mathf.Sin(ix * 21942f + iy * 171324f + 8912f) * Mathf.Cos(ix * 23157f * iy * 217832f + 9758f);
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }

    public float DotGridGradient(int ix, int iy, float x, float y)
    {
        Vector2 gradient = RandomGradient(ix, iy);

        float dx = x - (float)ix;
        float dy = y - (float)iy;
        return (dx * gradient.x + dy * gradient.y);
    }

    public float Interpolate(float a0, float a1, float w)
    {
        // Cubic interpolation
        return (a1 - a0) * (w * w * (3f - 2f * w)) + a0;
    }

    public float Perlin(float x, float y)
    {
        int x0 = (int)x;
        int y0 = (int)x;
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        float sx = x - (float)x0;
        float sy = y - (float)y0;

        float n0 = DotGridGradient(x0, y0, x, y);
        float n1 = DotGridGradient(x1, y0, x, y);
        float ix0 = Interpolate(n0, n1, sx);

        n0 = DotGridGradient(x0, y1, x, y);
        n1 = DotGridGradient(x1, y1, x, y);
        float ix1 = Interpolate(n0, n1, sx);

        return Interpolate(ix0, ix1, sy);
    }

    public float PerlinNoiseByAutocomplete(float x, float y, float scale, int octaves, float persistence, float lacunarity)
    {
        float noiseValue = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxValue = 0f;
        for (int i = 0; i < octaves; i++)
        {
            noiseValue += Mathf.PerlinNoise(x * frequency / scale, y * frequency / scale) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return noiseValue / maxValue; // Normalize the result to [0, 1]
    }
}
