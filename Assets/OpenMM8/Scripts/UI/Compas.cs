using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compas : MonoBehaviour 
{
    private Transform m_PlayerTransform;
    private RawImage m_CompasImage;

    // Use this for initialization
    void Start()
    {
        m_CompasImage = GetComponent<RawImage>();
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Rect rect = m_CompasImage.uvRect;
        rect.x = (m_PlayerTransform.eulerAngles.y - 30) / 360.0f;

        m_CompasImage.uvRect = rect;
    }
}
