using UnityEngine;
using System.Collections;

// ORIGINAL SCRIPT CREATED BY SEBASTIAN LAGUE 
// *https://www.youtube.com/user/Cercopithecan*

// This class will generate a noise map using layered perlin noise
// 
// Perlin noise is a type of coherent, gradient noise. Instead of 
// completely random values per pixel, values are gradually changed
// between.
//
// This algorithm takes layers of perlin noise and with each subsequent
// layer, reduces its influence on the map and increases its frequenct
// This results in a semi-realistic heightmapping

public static class Noise {

    /// <summary>
    /// Returns a 2d array of floats, each value being between 0 and 1
    /// </summary>
    /// <param name="mapWidth"></param>
    /// <param name="mapHeight"></param>
    /// <param name="seed"></param>
    /// <param name="scale"></param>
    /// <param name="octaves"></param>
    /// <param name="persistance"></param>
    /// <param name="lacunarity"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        // This variable will contain our noise map
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Assign the seed througha pseudo-random number generator
        // This will allow us to receive the same noise map if the same seed is used
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Clamp scale to a minimum value
        if (scale <= 0) {
            scale = 0.0001f;
        }

        // Keep track of minimum and maximum values to normalise later
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        // Iterate through each pixel
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    // Sample the noise at x and y  for each octave
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency ;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    // Alter amplitude and frequency for next octave
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    /// <summary>
    /// Returns a portion of a given noise map
    /// </summary>
    /// <param name="noiseMap"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns></returns>
    public static float[,] TakeNoiseChunk(float[,] noiseMap, int startX, int startY, int endX, int endY) {
        float[,] noiseMapChunk = new float[(endX + 1) - startX, (endY + 1) - startY];

        for(int y = startY; y < endY + 1; y++) {
            for(int x = startX; x < endX + 1; x++) {
                noiseMapChunk[x - startX, y - startY] = noiseMap[x, y];
            }
        }

        return noiseMapChunk;
    }

    /// <summary>
    /// Subtracts all values in mapB from mapA
    /// </summary>
    /// <param name="mapA"></param>
    /// <param name="mapB"></param>
    /// <returns></returns>
    public static float[,] SubtractMap(float[,] mapA, float[,] mapB) {
        // Assert that mapA and mapB are of the same dimensions
        if (mapA.GetLength(0) != mapB.GetLength(0) || mapA.GetLength(1) != mapB.GetLength(1)) {
            Debug.LogError("SubtractMap: mapA must have same dimensions as mapB");
        }

        float[,] subMap = new float[mapA.GetLength(0),mapA.GetLength(1)];

        // Loop through all values in map
        for (int y = 0; y < mapA.GetLength(0); y++) {
            for (int x = 0; x < mapA.GetLength(1); x++) {
                // Subtract values and clamp between (0,1)
                subMap[x, y] = Mathf.Clamp01(mapA[x, y] - mapB[x, y]);
            }
        }

        return subMap;
    }
}