using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.Experimental.UIElements;
using System.Collections.Generic;

public class TalkableBuilding : Talkable
{
    private Dictionary<int, int[]> m_SubmeshTris = new Dictionary<int, int[]>();
    private Mesh m_BuildingMesh;
    private int[] m_BuildingTris;

    [Header("Building")]
    public float SoundVolume = 0.6f;
    public AudioClip EnterSound;
    public AudioClip LeaveSound;
    public AudioClip GreetSound;

    private void Start()
    {
        IsBuilding = true;

        m_BuildingMesh = GetComponent<MeshFilter>().mesh;
        m_BuildingTris = m_BuildingMesh.triangles;

        for (int i = 0; i < m_BuildingMesh.subMeshCount; i++)
        {
            m_SubmeshTris.Add(i, m_BuildingMesh.GetTriangles(i));
        }
    }

    public override bool CanInteract(GameObject interacter, RaycastHit interactRay)
    {
        // Has to be a player
        if (interacter.GetComponent<PlayerParty>() == null)
        {
            return false;
        }

        // Player has to aim at the door - its "t**a03[a|b|c]" texture

        Renderer hitRenderer = interactRay.transform.GetComponent<Renderer>();
        if (hitRenderer == null || hitRenderer.sharedMaterial == null || hitRenderer.sharedMaterial.mainTexture == null)
        {
            Debug.LogError("Invalid renderer on the house object ?");
            return false;
        }

        int triangleIdx1 = m_BuildingTris[interactRay.triangleIndex * 3];
        int triangleIdx2 = m_BuildingTris[interactRay.triangleIndex * 3 + 1];
        int triangleIdx3 = m_BuildingTris[interactRay.triangleIndex * 3 + 2];

        string texName = "oops";
        for (int i = 0; i < m_BuildingMesh.subMeshCount; i++)
        {
            int[] subMeshTris = m_SubmeshTris[i];

            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == triangleIdx1 &&
                    subMeshTris[j + 1] == triangleIdx2 &&
                    subMeshTris[j + 2] == triangleIdx3)
                {
                    texName = hitRenderer.materials[i].name;
                    break;
                }
            }
        }

        texName = texName.ToLower();
        if (texName.Contains("03a") || texName.Contains("03b") || texName.Contains("03c"))
        {
            return true;
        }

        return false;
    }
}
