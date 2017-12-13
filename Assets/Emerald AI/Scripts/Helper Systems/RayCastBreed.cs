using UnityEngine;
using System.Collections;

public class RayCastBreed : MonoBehaviour {
	
	public string animalBreedTag = "Ally";
	RaycastHit hit;
	Ray ray;
	Camera cam;
	bool foodEnabled = true;
	public GameObject foodObject;
	
	void Start ()
	{
		cam = GetComponent<Camera>();
		foodObject = GameObject.Find("FoodObject");
	}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			foodEnabled = !foodEnabled;
		}

		if(Input.GetMouseButtonDown(0))
		{
			ray = cam.ViewportPointToRay (new Vector3(0.5f,0.5f,0));

			if (Physics.Raycast(ray, out hit, 6.5f))
			{
				if (hit.collider.gameObject.tag == animalBreedTag)
					{
						if (hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().isFollowing && !hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().isReadyForBreeding && !hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().isBaby && !hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().breedCoolDown)
						{
							hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().isReadyForBreeding = true;
						}
					}

			}
		}

	}
}
