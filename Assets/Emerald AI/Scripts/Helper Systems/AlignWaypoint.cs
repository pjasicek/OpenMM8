using UnityEngine;
using System.Collections;

public class AlignWaypoint : MonoBehaviour 
{
	public bool CheckWaypoint = false;

	public Terrain terrain;
	public GameObject terrainGameObeject;
	public Quaternion originalLookRotationWaypoint;
	public bool terrainFound = false;

	void Start ()
	{
		originalLookRotationWaypoint = transform.rotation;
		terrainGameObeject = GameObject.Find("Terrain");

		/*
		if (terrainGameObeject != null)
		{
			terrainFound = true;
		}

		if (terrainFound)
		{
			terrain  = terrainGameObeject.GetComponent<Terrain>();
		}
		*/
	}

	void Update () 
	{
		if (Terrain.activeTerrain == true)
		{
			float wayPointHeight = Terrain.activeTerrain.SampleHeight(transform.position);
			transform.position = new Vector3(transform.position.x, wayPointHeight + 0.05f, transform.position.z);
		}

		/*
		if (CheckWaypoint)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotationWaypoint, Time.deltaTime*100);

			if (transform.rotation == originalLookRotationWaypoint)
			{
				CheckWaypoint = false;
			}
		}
		*/
	}


	
	/*
	public void CheckSteepness ()
	{
		Vector3 normal = CalculateRotation();
		Vector3 direction = transform.position;
		direction.y = 0.0f;
		
		if(direction.magnitude > 0.1f && normal.magnitude > 0.1f) 
		{
			Quaternion quaternionLook = Quaternion.LookRotation(direction, Vector3.up);
			Quaternion quaternionNormal = Quaternion.FromToRotation(Vector3.up, normal);
			originalLookRotationWaypoint = quaternionNormal * quaternionLook;
		}
		
		CheckWaypoint = true;
	}

	Vector3 CalculateRotation () 
	{
		Vector3 terrainLocalPos = transform.position - terrain.transform.position;
		Vector2 normalizedPos = new Vector2(terrainLocalPos.x / terrain.terrainData.size.x, terrainLocalPos.z / terrain.terrainData.size.z);
		return terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
	}
	*/
}
