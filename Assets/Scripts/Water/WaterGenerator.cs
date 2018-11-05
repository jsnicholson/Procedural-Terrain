using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGenerator : MonoBehaviour{

    public int planeResolution;
    public float waterHeight;

    private float planeScale;

    public void Start() {
        TerrainGenerator terrainGenerator = this.GetComponent<TerrainGenerator>();
        int terrainSize = terrainGenerator.chunkSize * terrainGenerator.mapSize;

        planeScale = terrainSize / planeResolution;

        GenerateWater();
    }

	public void GenerateWater() {
        GameObject waterPlane = new GameObject();
        MeshFilter meshFilter = waterPlane.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = waterPlane.AddComponent<MeshRenderer>();
        meshFilter.mesh = SplitVertexMeshGenerator.GeneratePlane(planeResolution).CreateMesh();
        waterPlane.transform.position = new Vector3(waterPlane.transform.position.x, waterHeight, waterPlane.transform.position.z);
        waterPlane.transform.localScale = new Vector3(planeScale, 1, planeScale);
    }
}
