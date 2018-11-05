using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will generate a mesh from an input heightmap
// It will create individual vertices for each triangle in order to achieve a flat shaded, 
// solid colour terrain. 
// I am using this method as a first attempt, before delving into shaders and the like.

/*
 * |\ \~~~| |~~~/ /| Using this grid layout for the triangles
 * | \ \  | |  / / |
 * |  \ \ | | / /  |
 * |___\ \| |/ /~~~|
*/

/// <summary>
/// Generates a planar mesh in which each triangle has its own vertices
/// It then assigns height based on a height map and is scaled by the heightMultiplier
/// </summary>
public static class SplitVertexMeshGenerator {

    /// <summary>
    /// Generate a split vertex mesh
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static MeshData GeneratePlane(int size) {
        MeshData meshData = new MeshData(size, size);

        CreateTriangles(size, meshData);
        meshData.SplitVertices();

        return meshData;
    }

    /// <summary>
    /// Generate a split vertex mesh and apply a heightMap to the mesh
    /// </summary>
    /// <param name="heightMap"></param>
    /// <param name="heightMultiplier"></param>
    /// <returns></returns>
	public static MeshData GenerateHeightMesh(float[,] heightMap, float heightMultiplier) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);

        CreateTriangles(width, meshData);
        meshData.ApplyHeightMap(heightMap, heightMultiplier);
        meshData.SplitVertices();

        return meshData;
    }

    private static void CreateTriangles(int size, MeshData meshData) {
        // Defines the top left corner of the plane
        // Is used in centering the plane
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        int vertexIndex = 0;

        // Loop through each point in the grid
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                // Set the location of the vertex
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 0, topLeftZ - y);

                // If the vertex isnt occupying the right column or bottom row,
                // add the 2 triangles for this point
                if (x < size - 1 && y < size - 1) {
                    if (y % 2 == 0) {
                        if (x % 2 == 0) {
                            meshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                            meshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                        } else {
                            meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + size);
                            meshData.AddTriangle(vertexIndex + size + 1, vertexIndex + size, vertexIndex + 1);
                        }
                    } else {
                        if (x % 2 == 0) {
                            meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + size);
                            meshData.AddTriangle(vertexIndex + size + 1, vertexIndex + size, vertexIndex + 1);
                        } else {
                            meshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                            meshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                        }
                    }
                }

                vertexIndex++;
            }
        }
    }
}


/// <summary>
/// A class to hold data about a mesh
/// </summary>
public class MeshData {
    // Base components of the mesh
    public Vector3[] vertices;
    public int[] triangles;
    public Color32[] colours;

    int triangleIndex;

    // Constructor takes in dimensions
    public MeshData(int meshWidth, int meshHeight) {
        // Initialises all variables
        int vertAmount = (meshWidth - 1) * (meshHeight - 1) * 6;
        vertices = new Vector3[vertAmount];
        triangles = new int[vertAmount];
        colours = new Color32[vertAmount];
    }

    /// <summary>
    /// Takes in 3 vertices and creates a triangle from them
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    /// <summary>
    /// Create a new vertices such that each triangle has unshared vertices
    /// </summary>
    public void SplitVertices() {
        Vector3[] splitVerts = new Vector3[triangles.Length];

        for (int i = 0; i < triangles.Length; i++) {
            splitVerts[i] = vertices[triangles[i]];
            triangles[i] = i;
        }

        vertices = splitVerts;
    }

    public void ApplyHeightMap(float[,] heightMap, float heightMultiplier) {
        int size = heightMap.GetLength(0);
        int vertexIndex = 0;
        for (int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                vertices[vertexIndex].y = heightMap[x, y] * heightMultiplier;
                vertexIndex++;
            }
        }
    }

    public void SetColours(Color32[] colourMap) {
        colours = colourMap;
    }

    /// <summary>
    /// Create a new mesh from the data held in this instance of the class
    /// </summary>
    /// <returns></returns>
    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors32 = colours;
        // Recalculate all normals to correctly reorient lighting
        mesh.RecalculateNormals();
        return mesh;
    }
}