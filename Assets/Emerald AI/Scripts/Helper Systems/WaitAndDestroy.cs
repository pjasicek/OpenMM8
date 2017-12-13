using UnityEngine;
using System.Collections;

public class WaitAndDestroy : MonoBehaviour {
	
	public AudioClip soundToPlay;
	float destroyTimer;

	void Update () 
	{
		destroyTimer += Time.deltaTime;

		if (destroyTimer >= soundToPlay.length) 
		{
			Destroy(this.gameObject);
		}
	}
}
