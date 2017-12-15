using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FPSTempTest : MonoBehaviour
{
    //========== PUBLIC ==========
    public float m_GroundTexturePollInterval = 50.0f;
    public GameObject m_Ground;

    //========== PRIVATE ==========
    private Mesh m_GroundMesh;
    private int[] m_GroundTris;
    private Dictionary<int, int[]> m_SubmeshTris = new Dictionary<int, int[]>();
    string m_TextureBelow = "";
    private Renderer m_Renderer;

    // Use this for initialization
    void Start()
    {
        if (m_Ground == null)
        {
            m_Ground = GameObject.Find("_Ground_");
        }

        Assert.IsTrue(m_Ground != null, "Ground mesh was not specified nor was it found [_Ground_] !");

        m_GroundMesh = m_Ground.GetComponent<MeshFilter>().mesh;
        m_GroundTris = m_GroundMesh.triangles;

        for (int i = 0; i < m_GroundMesh.subMeshCount; i++)
        {
            m_SubmeshTris.Add(i, m_GroundMesh.GetTriangles(i));
        }

        InvokeRepeating("CheckTextureBelow", 0.0f, m_GroundTexturePollInterval / 1000.0f);
    }

    void OnTextureBelowChanged(string previousTexture, string currentTexture)
    {
        m_TextureBelow = currentTexture;

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
            if (m_TextureBelow != "None")
            {
                OnTextureBelowChanged(m_TextureBelow, "None");
            }
            return;
        }

        if (!hit.collider.gameObject.Equals(m_Ground))
        {
            if (m_TextureBelow != "Other")
            {
                OnTextureBelowChanged(m_TextureBelow, "Other");
            }
            return;
        }

        int matIdx = -1;
        
        int triangleIdx1 = m_GroundTris[hit.triangleIndex * 3];
        int triangleIdx2 = m_GroundTris[hit.triangleIndex * 3 + 1];
        int triangleIdx3 = m_GroundTris[hit.triangleIndex * 3 + 2];

        for (int i = 0; i < m_GroundMesh.subMeshCount; i++)
        {
            int[] subMeshTris = m_SubmeshTris[i];

            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == triangleIdx1 &&
                    subMeshTris[j + 1] == triangleIdx2 &&
                    subMeshTris[j + 2] == triangleIdx3)
                {
                    matIdx = i;
                    if (m_TextureBelow != hitRenderer.materials[matIdx].name)
                    {
                        OnTextureBelowChanged(m_TextureBelow, hitRenderer.materials[matIdx].name);
                    }
                    return;
                }
            }
        }
    }
}
