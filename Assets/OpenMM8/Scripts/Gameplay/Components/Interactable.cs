using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InteractFilter
{
    AllowAll,
    ByTexture,
    ByTriangles,
    DenyAll,
}

abstract public class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    public InteractSelector InteractSelector;

    // For the ByTexture filter
    private MeshInfo m_MeshInfo;

    protected void Start()
    {
        if (InteractSelector.FilterType == InteractFilter.ByTexture)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.mesh == null)
            {
                Debug.LogError(gameObject.name + ": InteractFilter.ByTexture was set with no MeshFilter/Mesh !");
                InteractSelector.FilterType = InteractFilter.DenyAll;
            }
            else
            {
                Renderer renderer = GetComponent<Renderer>();
                m_MeshInfo = MeshInfo.Create(meshFilter.mesh, renderer);
                if (m_MeshInfo == null)
                {
                    Debug.LogError("Failed to initialize MeshInfo.");
                    InteractSelector.FilterType = InteractFilter.DenyAll;
                }
            }
        }
    }

    public bool TryInteract(GameObject interacter, RaycastHit interactRay)
    {
        if (InteractSelector.FilterType == InteractFilter.DenyAll)
        {
            return false;
        }

        if (InteractSelector.FilterType == InteractFilter.ByTexture)
        {
            m_MeshInfo.DrawTriangleGizmo(interactRay.triangleIndex);
            Debug.LogError("Clicked Triangle Index: " + interactRay.triangleIndex);

            Material mat = m_MeshInfo.GetMaterialAtTriangleIndex(interactRay.triangleIndex);
            if (mat == null)
            {
                return false;
            }

            string texName = mat.name;
            texName = texName.ToLower().Split(' ')[0];

            bool hasTexture = false;
            foreach (string allowedTexName in InteractSelector.AllowedTextures)
            {
                if (texName.EndsWith(allowedTexName))
                {
                    hasTexture = true;
                    break;
                }
            }

            if (!hasTexture)
            {
                return false;
            }
        }
        else if (InteractSelector.FilterType == InteractFilter.ByTriangles)
        {
            // Only specified triangle(s) on this game object can be interacted with
            if (!InteractSelector.AllowedTriangles.Contains(interactRay.triangleIndex))
            {
                return false;
            }
        }

        if (!CanInteract(interacter, interactRay))
        {
            return false;
        }

        return Interact(interacter, interactRay);
    }

    abstract protected bool Interact(GameObject interacter, RaycastHit interactRay);
    virtual protected bool CanInteract(GameObject interacter, RaycastHit interactRay)
    {
        return true;
    }
}

[System.Serializable]
public class InteractSelector
{
    public InteractFilter FilterType = InteractFilter.AllowAll;
    public List<int> AllowedTriangles = new List<int>();
    public List<string> AllowedTextures = new List<string>();
}

public class MeshInfo
{
    private Dictionary<int, int[]> m_SubmeshTris = new Dictionary<int, int[]>();
    private Mesh m_Mesh;
    private Renderer m_Renderer;
    private Transform m_Transform;
    private int[] m_Tris;

    static public MeshInfo Create(Mesh mesh, Renderer renderer)
    {
        if (mesh == null || renderer == null)
        {
            return null;
        }

        MeshInfo mi = new MeshInfo();

        mi.m_Mesh = mesh;
        mi.m_Renderer = renderer;
        mi.m_Transform = renderer.gameObject.transform;
        mi.m_Tris = mesh.triangles;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            mi.m_SubmeshTris.Add(i, mesh.GetTriangles(i));
        }

        return mi;
    }

    public Material GetMaterialAtTriangleIndex(int triangleIdx)
    {
        int triangleIdx1 = m_Tris[triangleIdx * 3];
        int triangleIdx2 = m_Tris[triangleIdx * 3 + 1];
        int triangleIdx3 = m_Tris[triangleIdx * 3 + 2];

        for (int i = 0; i < m_Mesh.subMeshCount; i++)
        {
            int[] subMeshTris = m_SubmeshTris[i];

            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == triangleIdx1 &&
                    subMeshTris[j + 1] == triangleIdx2 &&
                    subMeshTris[j + 2] == triangleIdx3)
                {
                    return m_Renderer.materials[i];
                }
            }
        }

        return null;
    }

    public void DrawTriangleGizmo(int triangleIdx)
    {
        Vector3[] vertices = m_Mesh.vertices;
        int[] triangles = m_Mesh.triangles;
        Vector3 p0 = vertices[triangles[triangleIdx * 3 + 0]];
        Vector3 p1 = vertices[triangles[triangleIdx * 3 + 1]];
        Vector3 p2 = vertices[triangles[triangleIdx * 3 + 2]];
        p0 = m_Transform.TransformPoint(p0);
        p1 = m_Transform.TransformPoint(p1);
        p2 = m_Transform.TransformPoint(p2);
        Debug.DrawLine(p0, p1, Color.green, 60.0f, false);
        Debug.DrawLine(p1, p2, Color.green, 60.0f, false);
        Debug.DrawLine(p2, p0, Color.green, 60.0f, false);
    }
}