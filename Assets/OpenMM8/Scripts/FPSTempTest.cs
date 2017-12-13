using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTempTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetKey(KeyCode.Q))
            return;

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -Vector3.up, out hit))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        Texture2D tex = rend.material.mainTexture as Texture2D;

        Mesh m = meshCollider.sharedMesh;

        int matIdx = -1;
        int[] hittedTriangle = new int[]
                    {
                        m.triangles[hit.triangleIndex * 3],
                        m.triangles[hit.triangleIndex * 3 + 1],
                        m.triangles[hit.triangleIndex * 3 + 2]
                    };
        for (int i = 0; i < m.subMeshCount; i++)
        {
            int[] subMeshTris = m.GetTriangles(i);
            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == hittedTriangle[0] &&
                    subMeshTris[j + 1] == hittedTriangle[1] &&
                    subMeshTris[j + 2] == hittedTriangle[2])
                {
                    matIdx = i;
                }
            }
        }

        sw.Stop();

        Debug.Log("Elapsed: " + sw.ElapsedMilliseconds);

        //Debug.Log("Mat: " + rend.materials[matIdx].name);
    }
}
