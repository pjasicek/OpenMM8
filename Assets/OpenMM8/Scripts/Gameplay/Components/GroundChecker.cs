using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundChecker : MonoBehaviour
{
    //========== PUBLIC ==========
    public float GroundTexturePollInterval = 50.0f;
    public GameObject Ground;

    //========== PRIVATE ==========
    private Mesh GroundMesh;
    private int[] GroundTris;
    private Dictionary<int, int[]> SubmeshTris = new Dictionary<int, int[]>();
    string TextureBelow = "";
    private Renderer Renderer;

    // Use this for initialization
    void Start()
    {
        if (Ground == null)
        {
            Ground = GameObject.Find("_Ground_");
        }

        Assert.IsTrue(Ground != null, "Ground mesh was not specified nor was it found [_Ground_] !");

        GroundMesh = Ground.GetComponent<MeshFilter>().mesh;
        GroundTris = GroundMesh.triangles;

        for (int i = 0; i < GroundMesh.subMeshCount; i++)
        {
            SubmeshTris.Add(i, GroundMesh.GetTriangles(i));
        }

        InvokeRepeating("CheckTextureBelow", 0.0f, GroundTexturePollInterval / 1000.0f);
    }

    void OnTextureBelowChanged(string previousTexture, string currentTexture)
    {
        TextureBelow = currentTexture;

        //Debug.Log("Texture changed from: " + previousTexture.ToString() + " to: " + m_TextureBelow.ToString());
    }
	
	// Update is called once per frame
	void CheckTextureBelow()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            return;
        }

        Renderer hitRenderer = hit.transform.GetComponent<Renderer>();
        if (hitRenderer == null || hitRenderer.sharedMaterial == null || hitRenderer.sharedMaterial.mainTexture == null)
        {
            if (TextureBelow != "None")
            {
                OnTextureBelowChanged(TextureBelow, "None");
            }
            return;
        }

        if (!hit.collider.gameObject.Equals(Ground))
        {
            if (TextureBelow != "Other")
            {
                OnTextureBelowChanged(TextureBelow, "Other");
            }
            return;
        }

        int matIdx = -1;
        
        int triangleIdx1 = GroundTris[hit.triangleIndex * 3];
        int triangleIdx2 = GroundTris[hit.triangleIndex * 3 + 1];
        int triangleIdx3 = GroundTris[hit.triangleIndex * 3 + 2];

        for (int i = 0; i < GroundMesh.subMeshCount; i++)
        {
            int[] subMeshTris = SubmeshTris[i];

            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == triangleIdx1 &&
                    subMeshTris[j + 1] == triangleIdx2 &&
                    subMeshTris[j + 2] == triangleIdx3)
                {
                    matIdx = i;
                    if (TextureBelow != hitRenderer.materials[matIdx].name)
                    {
                        OnTextureBelowChanged(TextureBelow, hitRenderer.materials[matIdx].name);
                    }
                    return;
                }
            }
        }
    }
}
