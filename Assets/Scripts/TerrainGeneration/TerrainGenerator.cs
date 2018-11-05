using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public bool autoUpdate = false;

    public int chunkSize;
    private int chunkVertexSize;
    public int mapSize;

    public float noiseScale;

    public int seed;
    public Vector2 offset;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    
    public float meshHeightMultiplier;

    public Material terrainMaterial;

    [SerializeField]
    SerializableDictionary<Vector2, TerrainChunk> terrainChunkDictionary = new SerializableDictionary<Vector2, TerrainChunk>();

    public void OnValidate() {
        chunkSize = Mathf.Max(1, chunkSize);
        mapSize = Mathf.Max(1, mapSize);
        octaves = Mathf.Max(0, octaves);
        lacunarity = Mathf.Max(1, lacunarity);

        chunkVertexSize = chunkSize + 1;
    }

    public void GenerateMap() {
        RegionMap regions = new RegionMap();
        ColourKey sand = new ColourKey(new Color(0.93f, 0.85f, 0.68f), 0.0f, 0.3f, "sand");
        ColourKey dirt = new ColourKey(new Color(0.52f, 0.36f, 0.22f), 0.5f, 0.65f, "dirt");
        ColourKey stone = new ColourKey(new Color(0.36f, 0.36f, 0.36f), 0.85f, 1.0f, "stone");
        regions.AddColourKey(sand);
        regions.AddColourKey(dirt);
        regions.AddColourKey(stone);

        // Calculate the top left corner of the grid in world space
        float topLeftX = ((mapSize / 2.0f) - 0.5f) * -chunkSize;
        float topLeftZ = ((mapSize / 2.0f) - 0.5f) * chunkSize;

        int worldSize = (chunkVertexSize * mapSize) - 1;

        float[,] noiseMap = Noise.GenerateNoiseMap(worldSize, worldSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] falloffMap = FalloffGenerator.GenerateCircularFalloffMap(worldSize);

        for (int y = 0; y < worldSize; y++) {
            for (int x = 0; x < worldSize; x++) {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
            }
        }

        if(terrainChunkDictionary.Count > mapSize * mapSize) {
            terrainChunkDictionary.Clear();

            // Have to create a temporary child list for foreach as the 'in Transform' keywords
            // does not handle destroying objects in editor well
            List<Transform> childList = transform.Cast<Transform>().ToList<Transform>();
            foreach (Transform child in childList) {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }

        float[,] currentNoiseChunk;
        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                float gridX = -Mathf.Floor(mapSize / 2.0f) + x;
                float gridY = Mathf.Floor(mapSize / 2.0f) - y;
                Vector2 gridCoord = new Vector2(gridX, gridY);

                int startX, startY, endX, endY;
                startX = x * chunkSize;
                endX = startX + chunkSize;
                startY = y * chunkSize;
                endY = startY + chunkSize;

                if(mapSize > 1) {
                    currentNoiseChunk = Noise.TakeNoiseChunk(noiseMap, startX, startY, endX, endY);
                } else {
                    currentNoiseChunk = noiseMap;
                }
            
                MeshData chunkMesh = SplitVertexMeshGenerator.GenerateHeightMesh(currentNoiseChunk, meshHeightMultiplier);
                Vector3 chunkWorldPosition = new Vector3(topLeftX + (x * chunkSize), 0, topLeftZ + (y * -chunkSize));
                chunkMesh.SetColours(ColourGenerator.GenerateColourMap(currentNoiseChunk, regions));

                if (terrainChunkDictionary.ContainsKey(gridCoord)) {
                    terrainChunkDictionary[gridCoord].updateChunk(chunkMesh, chunkWorldPosition);
                } else {
                    terrainChunkDictionary.Add(gridCoord, new TerrainChunk(gridCoord, chunkWorldPosition, this.transform, chunkMesh, terrainMaterial));
                }
            }
        }
    }

    public class TerrainChunk {
        GameObject chunk;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        MeshCollider meshCollider;

        public TerrainChunk(Vector2 gridCoord, Vector3 worldCoord, Transform parent, MeshData meshData, Material mat) {
            chunk = new GameObject("TerrainChunk (" + gridCoord.x + "," + gridCoord.y + ")");
            meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshFilter = chunk.AddComponent<MeshFilter>();
            meshRenderer.material = mat;
            chunk.transform.parent = parent;
            chunk.transform.position = worldCoord;
            updateChunk(meshData, worldCoord);
        }

        public void updateChunk(MeshData meshData, Vector3 worldCoord) {
            meshFilter.mesh = meshData.CreateMesh();
            chunk.transform.position = worldCoord;
        }

        public void DestroyChunk() {
            Destroy(this.chunk);
        }
    }
}