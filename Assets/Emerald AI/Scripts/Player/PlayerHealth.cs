using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof (AudioSource))]

public class PlayerHealth : MonoBehaviour 
{
	public float startingHealth = 100;
	public float currentHealth = 100;
	public Slider healthBar;
	public float healthRegen = 1.5f;
	public bool useHitSounds = false;
	public bool useHitDelay = false;
	public bool destroyPlayerOnDeath = false;
	//public AudioClip[] hitSounds;
	public float hitDelaySeconds = 0.5f;

	private float tempDamage = 0;
	private bool isDead = false;
	private AudioSource _audioSource;
	private float timer;

	public int hitSoundSize;
	public List<AudioClip> hitSounds = new List<AudioClip>();
	public List<bool> foldOutListHit = new List<bool>();

	void Awake ()
	{
		currentHealth = startingHealth;

		if (healthBar == null)
		{
			GameObject HB = GameObject.Find("HealthBar");
			if (HB != null){
				healthBar = HB.GetComponent<Slider>();
			}
		}

		if (healthBar != null)
		{
			healthBar.value = currentHealth * 0.01f;
		}

		_audioSource = GetComponent<AudioSource>();

	}

	void Update ()
	{
		timer += Time.deltaTime * healthRegen;

		if (timer >= 1 && currentHealth < 100 && !isDead)
		{
			currentHealth += 1;
			timer = 0;
		}

		if (healthBar != null)
		{
			healthBar.value = currentHealth * 0.01f;
		}
	}

	public void DamagePlayer (float damageTaken)
	{
			if (!useHitDelay && !isDead)
			{
				currentHealth -= damageTaken;

				if (useHitSounds && _audioSource != null)
				{
					_audioSource.PlayOneShot(hitSounds[Random.Range(0,hitSounds.Count)]);
				}
				
				if (currentHealth <= 0)
				{
					if (!destroyPlayerOnDeath)
					{
						Debug.Log("Player has died");
						isDead = true;
					}
					
					if (destroyPlayerOnDeath)
					{
						Destroy(gameObject);
					}
				}
			}

			if (useHitDelay && !isDead)
			{
				StartCoroutine(Delay());
				tempDamage = damageTaken;
			}



	}

	IEnumerator Delay() 
	{
		yield return new WaitForSeconds(hitDelaySeconds);

		if (useHitSounds && _audioSource != null)
		{
			_audioSource.PlayOneShot(hitSounds[Random.Range(0,hitSounds.Count)]);
		}

		currentHealth -= tempDamage;
		
		if (currentHealth <= 0)
		{
			if (!destroyPlayerOnDeath)
			{
				Debug.Log("Player has died");
				isDead = true;
			}
			
			if (destroyPlayerOnDeath)
			{
				Destroy(gameObject);
			}
		}
	}
}
