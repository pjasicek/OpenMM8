using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkydomeResize : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
        transform.localScale = new Vector3(450.0f, 450.0f, 450.0f);
        transform.position = new Vector3(0.0f, 0.0f, 0.0f);
	}
}
