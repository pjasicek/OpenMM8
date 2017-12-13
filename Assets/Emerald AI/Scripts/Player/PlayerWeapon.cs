using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]

public class PlayerWeapon : MonoBehaviour {
	
	public GameObject bloodEffect;
	public bool useBloodEffect = false;
	public bool useHitEffect = false;
	public GameObject hitOtherEffect;
	public float attackDistance = 4.0f;
	public float attackDelay = 0.7f;
	int damage = 5;
	public int MinDamage = 5;
	public int MaxDamage = 10;
	public float attackTime = 1;
	public bool useImpactSounds = true;
	//public AudioClip[] impactSounds;
	public bool useImpactOtherSounds = true;
	public AudioClip[] impactSoundsOther;
	
	private bool calculatedHit = false;
	private Ray ray;
	private Ray ray2;
	private RaycastHit hit;
	public float timer;
	public float attackTimer;
	private AudioSource _audioSource;
	private bool audioDisabled = false;
	private float attackSoundTimer = 0;
	private Camera cam;

	public bool useAttackSound = false;
	public int attackSoundSize;
	public List<AudioClip> attackSounds = new List<AudioClip>();
	public List<bool> foldOutListAttack = new List<bool>();

	public GameObject enemyToSpawn;
	public GameObject allyToSpawn;
	public GameObject ShakeEffect;

	public int impactSoundSize;
	public List<AudioClip> impactSounds = new List<AudioClip>();
	public List<bool> foldOutList = new List<bool>();

	public int impactOtherSoundSize;
	public List<AudioClip> impactOtherSounds = new List<AudioClip>();
	public List<bool> foldOutListOther = new List<bool>();
	public List<string> HitTags = new List<string>();

	void Start () 
	{
		_audioSource = GetComponent<AudioSource>();
		damage = Random.Range(MinDamage, MaxDamage);

		if (_audioSource == null)
		{
			audioDisabled = true;
		}

		cam = GetComponent<Camera>();
	}

	void FixedUpdate () 
	{
		if(Input.GetMouseButton(0) && attackTimer >= attackTime && !calculatedHit)
		{
			ray = cam.ViewportPointToRay (new Vector3(0.5f,0.5f,0));
			
			if (Physics.Raycast(ray, out hit, attackDistance))
			{
				calculatedHit = true;
				//attackTimer = 0;
			}
			else
			{
				calculatedHit = false;
				timer = 0;
				attackTimer = 0;
			}
		}

		if(Input.GetMouseButton(0) && attackSoundTimer >= attackTime + attackDelay)
		{
			if (useAttackSound)
			{
				_audioSource.PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
				attackSoundTimer = 0;
			}
		}


	}

	void Update ()
	{
		if (!calculatedHit)
		{
			attackTimer += Time.deltaTime;
		}

		attackSoundTimer += Time.deltaTime;

		if (calculatedHit)
		{
			timer += Time.deltaTime;

			if (hit.collider == null)
			{
				//calculatedHit = false;
			}
			
			if (timer >= attackDelay)
			{
				if (hit.collider != null && hit.collider.gameObject.GetComponent<Emerald_Animal_AI>() != null)
				{
					if (HitTags.Contains(hit.collider.gameObject.tag))
					{
						if (useBloodEffect)
						{
							GameObject SpawnedEffect = Instantiate(bloodEffect, hit.point, Quaternion.identity) as GameObject;
							SpawnedEffect.transform.parent = transform.root;
						}
							
						if (!audioDisabled && useImpactSounds)
						{
							_audioSource.PlayOneShot(impactSounds[Random.Range(0,impactSounds.Count)]);
						}


						//1.3 removed DamageFromPlayer with Damage
						hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().Damage(damage);
					    
						if (hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().aggression > 2 && !hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().huntMode)
						{
							hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().MakeAttack(transform.parent.root.gameObject);
						}

						if (hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().aggression <= 1 && !hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().isFleeing)
						{
							hit.collider.gameObject.GetComponent<Emerald_Animal_AI>().MakeFlee(transform.parent.root.gameObject);
						}
					}
				}

				if (hit.collider != null && hit.collider.tag == "Untagged" || hit.collider != null && hit.collider.tag == "Terrain")
				{
					if (hitOtherEffect != null && useHitEffect)
					{
						Instantiate(hitOtherEffect, hit.point, Quaternion.LookRotation(hit.normal));
					}

					if (!audioDisabled && useImpactOtherSounds)
					{
						_audioSource.PlayOneShot(impactOtherSounds[Random.Range(0,impactOtherSounds.Count)]);
					}
				}

				damage = Random.Range(MinDamage, MaxDamage);

				attackTimer = 0; 
				calculatedHit = false;
				timer = 0;
			}
		}
	}
}
