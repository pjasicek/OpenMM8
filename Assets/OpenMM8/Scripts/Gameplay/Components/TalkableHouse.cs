﻿using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.Experimental.UIElements;
using System.Collections.Generic;

public class TalkableHouse : Talkable
{
    private Dictionary<int, int[]> m_SubmeshTris = new Dictionary<int, int[]>();
    private Mesh m_HouseMesh;
    private int[] m_HouseTris;

    private void Start()
    {
        IsHouse = true;

        m_HouseMesh = GetComponent<MeshFilter>().mesh;
        Debug.Log("Mesh name: " + m_HouseMesh.name);
        Debug.Log("Is readable: " + m_HouseMesh.isReadable);
        m_HouseTris = m_HouseMesh.triangles;

        for (int i = 0; i < m_HouseMesh.subMeshCount; i++)
        {
            m_SubmeshTris.Add(i, m_HouseMesh.GetTriangles(i));
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

        int triangleIdx1 = m_HouseTris[interactRay.triangleIndex * 3];
        int triangleIdx2 = m_HouseTris[interactRay.triangleIndex * 3 + 1];
        int triangleIdx3 = m_HouseTris[interactRay.triangleIndex * 3 + 2];

        string texName = "oops";
        for (int i = 0; i < m_HouseMesh.subMeshCount; i++)
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
