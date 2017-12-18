using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestOffset : MonoBehaviour 
{
    public Vector2 m_MinimapScale = new Vector2(0.2f, 0.2f);

    private Transform m_PlayerTransform;
    private RawImage m_MinimapImage;

	// Use this for initialization
	void Start () 
	{
        m_MinimapImage = GetComponent<RawImage>();
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () 
	{
        const float side = 650;

        Rect rect = m_MinimapImage.uvRect;
        rect.x = 0.35f - m_PlayerTransform.position.x / side;
        rect.y = 0.35f - m_PlayerTransform.position.z / side;
        
        m_MinimapImage.uvRect = rect;
    }
}
