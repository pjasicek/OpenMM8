using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapArrowRotator : MonoBehaviour 
{
    Transform m_PlayerTransform;
    RawImage m_ArrowImage;

	// Use this for initialization
	void Start () 
	{
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_ArrowImage = GetComponent<RawImage>();

	}
	
	// Update is called once per frame
	void Update () 
	{
        Vector3 currAngle = m_ArrowImage.rectTransform.eulerAngles;
        currAngle.z = - m_PlayerTransform.rotation.eulerAngles.y;
        m_ArrowImage.rectTransform.eulerAngles = currAngle;
	}
}
