using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDisplay : MonoBehaviour {

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Renderer textureRender;

    public void DrawMesh(MeshData meshData) {
        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    public void DrawTexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
