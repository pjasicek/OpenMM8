using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealthBars : MonoBehaviour 
{
	public Camera PlayerCamera;
	public int lookAwaySeconds = 5;
	public int lookDistance = 10;
	public GameObject HealthBarCanvas;
	public Slider healthBarSlider;
	public Text healthBarText;
	public string EmeraldAITag;

	bool currentlyLookingAtTarget = false; 
	bool deathTimerActive = false;
	float lookAwayTimer = 0;
	float deathTimer = 0;
	public float updateTimer = 0;
	RaycastHit hit;
	GameObject currentTarget;
	Emerald_Animal_AI currentEmeraldSystem;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Ray ray = PlayerCamera.ViewportPointToRay (new Vector3(0.5f,0.5f,0));

		if (Physics.Raycast(ray, out hit, lookDistance))
		{
			if (hit.collider.gameObject.tag == EmeraldAITag && updateTimer > 0.5f)
			{
				currentlyLookingAtTarget = true;
				currentTarget = hit.collider.gameObject;
				currentEmeraldSystem = currentTarget.GetComponent<Emerald_Animal_AI>();

				healthBarText.text = "==== " + currentEmeraldSystem.NPCName + " ====";
				healthBarSlider.maxValue = currentEmeraldSystem.startingHealth;

				updateTimer = 0;
			}

			if (hit.collider.gameObject.tag == EmeraldAITag)
			{
				updateTimer += Time.deltaTime;
				lookAwayTimer = 0;
			}

			if (hit.collider.gameObject.tag != EmeraldAITag)
			{
				lookAwayTimer += Time.deltaTime;

				if (lookAwayTimer > lookAwaySeconds)
				{
					currentlyLookingAtTarget = false;
					currentTarget = null;
				}
			}
		}
	

		if (deathTimerActive)
		{
			deathTimer += Time.deltaTime;

			if (deathTimer > 1)
			{
				HealthBarCanvas.SetActive(false);
				currentlyLookingAtTarget = false;
				deathTimerActive = false;
				currentTarget = null;
			}
		}

		if (currentlyLookingAtTarget && !deathTimerActive)
		{
			HealthBarCanvas.SetActive(true);
			healthBarSlider.value = currentEmeraldSystem.currentHealth;

			if (currentEmeraldSystem.currentHealth <= 0)
			{
				deathTimerActive = true;
			}
		}

		if (!currentlyLookingAtTarget)
		{
			HealthBarCanvas.SetActive(false);
		}
	}
}
