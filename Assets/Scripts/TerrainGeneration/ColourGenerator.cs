using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColourGenerator {

    /// <summary>
    /// Generates an array of colours corresponding to the heights of each vertex 
    /// provided by the given heightMap
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="regions"></param>
    /// <returns></returns>
	public static Color32[] GenerateColourMap(float[,] heightMap, RegionMap regions) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        int vertAmount = (width - 1) * (height - 1) * 6;

        float maxVariation = 0.02f; 

        Color32[] colourMap = new Color32[vertAmount];

        int vertexIndex = 0;
        for(int y = 0; y < height - 1; y++) {
            for(int x = 0; x < width - 1; x++) {
                float vertHeight = heightMap[x, y];

                Color32 newColour = regions.Evaluate(vertHeight);
                newColour = AddVariation(newColour, maxVariation);

                for (int i = 0; i < 6; i++) {
                    colourMap[vertexIndex + i] = newColour;
                }

                vertexIndex += 6;
            }
        }

        return colourMap;
    }

    /// <summary>
    /// Add variation to a colour based on a range of +- maxVariation
    /// </summary>
    /// <param name="startColour"></param>
    /// <param name="maxVariation"></param>
    /// <returns></returns>
    public static Color32 AddVariation(Color32 startColour, float maxVariation) {
        float newR = Mathf.Clamp((startColour.r / 255f) + Random.Range(-maxVariation, maxVariation), 0, 1);
        float newG = Mathf.Clamp((startColour.g / 255f) + Random.Range(-maxVariation, maxVariation), 0, 1);
        float newB = Mathf.Clamp((startColour.b / 255f) + Random.Range(-maxVariation, maxVariation), 0, 1);
        Color32 newColour = new Color(newR, newG, newB);
        return newColour;
    }
}