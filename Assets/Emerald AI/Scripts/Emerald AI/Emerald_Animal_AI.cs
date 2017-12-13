//Emerald AI by: Black Horizon Studios
//Version 1.3.5

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_5_5 || UNITY_5_6
using UnityEngine.AI;
#endif

[RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (AudioSource))]

[System.Serializable]
public class Emerald_Animal_AI : MonoBehaviour 
{
	public bool useRootMotion = true;
	public string IdleParameter = "Idle";
	public string Graze1Parameter = "Graze 1";
	public string Graze2Parameter = "Graze 2";
	public string Graze3Parameter = "Graze 3";
	public string WalkParameter = "Walk";
	public string RunParameter = "Run";
	public string DieParameter = "Die";
	public string Attack1Parameter = "Attack 1";
	public string Attack2Parameter = "Attack 2";
	public string Attack3Parameter = "Attack 3";
	public string RunAttackParameter = "Run Attack";
	public string IdleCombatParameter = "Combat Idle";
	public string HitParameter = "Hit";
	public bool useMecanim = false;
	public Animator AnimatorComponent;
	public RuntimeAnimatorController AnimatorController;
	public int AnimationType = 1;

	public int deactivateAISeconds = 40;
	float attackingTime;
	public bool UseSendMessage = false;
	public string SendMessageForAIDamage = "Damage";
	public string SendMessageForPlayerDamage = "DamagePlayer";
	public float takeDamageDelaySeconds = 0;
	public bool WaypointGenerated;
	public List<GameObject> potentialTargets = new List<GameObject>();
	public bool findNextTarget = false;
	public List<string> EmeraldTags = new List<string>();
	public string AITag;
	public string EmeraldObjectsTag = "Respawn";
	public LayerMask EmeraldLayerMask = 4;
	string EmeraldLayerMaskString;
	string PlayerLayerMaskString;
	public LayerMask PlayerLayerMask = 0;
	public bool useRunAttacks = false;
	public List<string> DisplayTags = new List<string>();
	public float walkRotateSpeed = 3;
	public float runRotateSpeed = 6;
	public float DistanceFromTarget;
	public bool useCustomAttackTransfrom = false;
	public Transform customAttackTransform;
	public bool hasUpdatedFromStart = false;
	public bool useCustomBloodTransform = false;
	public Transform customBloodTransform;
	public int herdRadius = 40;
	public int herdFollowRadius = 10;
	Collider [] CollidersInArea; //1.3.5
	//public List<Collider> CollidersInArea = new List<Collider>();
	public bool UseLayerMask = false;
	public bool startRangedFlee = false;
	public float rangedImpactDistance;
	public bool makePassivesFlee = false;
	public bool playerUsesSeparateLayer = false;

	public float idleAnimationSpeed = 1.0f;
	public float idleCombatAnimationSpeed = 1.0f;
	public float attackAnimationSpeed = 1.0f;
	public float grazeAnimationSpeed = 1.0f;
	public float dieAnimationSpeed = 1.0f;

	public int fleeRadius = 15;
	public int huntRadius = 15;
	public int wanderRange = 20;
	public int fleeRange = 25;
	public int turnSpeed = 800;
	public float stoppingDistance = 4;
	public int extraFleeSeconds = 25;
	
	public int grazeLengthMin = 2;
	public int grazeLengthMax = 6;
	
	public float walkSpeed = 1.0f;
	public float runSpeed = 4.0f;
	
	public float walkAnimationSpeed = 1.0f;
	public float runAnimationSpeed = 1.0f;
	
	public AnimationClip idleAnimation;
	public AnimationClip idleBattleAnimation;
	public AnimationClip graze1Animation;
	public AnimationClip graze2Animation;
	public AnimationClip graze3Animation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip turnAnimation;
	public AnimationClip deathAnimation;
	
	public float pathWidth = 0.25f;
	public Color pathColor = Color.green;
	
	public bool drawWaypoints = false;
	public bool drawPaths = false;
	public bool useDustEffect = false;
	
	public ParticleSystem dustEffect;
	
	public Color wanderRangeColor = new Color(0f, 0.8f, 0, 0.1f);
	public Color fleeRadiusColor = new Color(1.0f, 1.0f, 0, 0.1f);
	public Color huntRadiusColor = new Color(1.0f, 1.0f, 0, 0.1f);
	public Color fleeRangeColor = new Color(1.0f, 0, 0, 0.1f);
	public Color stoppingRangeColor = new Color(0, 1.0f, 0, 0.1f);
	
	public bool isFleeing = false;
	public bool isGrazing = false;
	private UnityEngine.AI.NavMeshAgent navMeshAgent;
	private float grazeTimer = 0;
	private int grazeLength = 10;
	public Vector3 startPosition;
	private Vector3 currentPlayerTransform;
	private float playerZPos;
	private Vector3 destination;
	private float pathWidthAdjusted;
	
	private GameObject currentWaypoint;
	private GameObject waypointHolder;
	private GameObject fleePoint;
	
	private LineRenderer line;
	private Transform target; 
	private ParticleSystem clone;
	private Animation anim;
	public SphereCollider triggerCollider;
	private BoxCollider boxCollider;
	
	public int aggression = 1;
	public int grazeAnimationNumber;
	public int totalGrazeAnimations;
	public Material pathMaterial;
	public float lineYOffSet;
	public bool useVisualRadius = true;
	public bool useAnimations = true;
	public bool rotateFlee = false;
	public Vector3 direction;
	public Quaternion playerRotation;
	public Quaternion predatorRotation;
	public float rotateTimer;
	public float fleeTimer;
	public bool startFleeTimer = false;
	
	public Terrain terrain;
	public GameObject terrainGameObeject;
	public float steepness;
	
	public bool huntMode = false;
	public GameObject currentAnimal;
	public int preySize = 2;
	public int predatorSize = 2;
	public int ChaseType = 1;
	public bool startHuntTimer = false;
	public float huntTimer = 30;
	public bool enableDebugLogs = false;
	public float attackTimer = 0;
	public float attackTime = 1;
	public float attackTimeMin = 1;
	public float attackTimeMax = 1;
	public float attackAnimationSpeedMultiplier = 1.5f;
	public bool withinAttackDistance = false;
	
	public int totalAttackAnimations;
	public int currentAttackAnimation;
	public AnimationClip currentAttackAnimationClip;
	public AnimationClip attackAnimation1;
	public AnimationClip attackAnimation2;
	public AnimationClip attackAnimation3;
	public AnimationClip attackAnimation4;
	public AnimationClip attackAnimation5;
	public AnimationClip attackAnimation6;
	public AnimationClip hitAnimation;
	public AnimationClip runAttackAnimation;
	
	public GameObject hitEffect;
	public bool damageDealt = false;
	public bool damageTaken = false;
	public bool useRunAttackAnimations = false;
	public int startingHealth = 15;
	public int currentHealth = 15;
	public GameObject deadObject;
	
	public int offSetPosition;
	public int offSetDistance;
	public bool currentlyBeingPursued = false;

	public int attackDamage = 5;
	public int attackDamageMin = 5;
	public int attackDamageMax = 5;

	public List<AudioClip> attackSounds = new List<AudioClip>();
	public List<AudioClip> injuredSounds = new List<AudioClip>();
	public List<AudioClip> impactSounds = new List<AudioClip>();

	public AudioClip weaponSound;
	public List<AudioSource> AudioSources = new List<AudioSource>();
	public AudioClip getHitSound;
	public AudioClip dieSound;
	public float minSoundPitch = 0.8f;
	public float maxSoundPitch = 1.2f;
	public float updateSpeedTimer = 0;
	public float updateSpeed = 0.1f;
	
	public float velocity;
	public bool attackWhileRunning = false;
	public bool attackWhileRunningEnabled = false;
	public bool isCoolingDown = false;
	public float coolDownTimer;
	public float coolDownSeconds = 0;
	public Quaternion lookRotation;
	public Quaternion originalLookRotation;
	
	public float freezeSecondsMin = 0.25f;
	public float freezeSecondsMax = 1;
	public float freezeSecondsTotal;
	public float freezeSecondsTimer = 0;
	public bool isFrozen = false;
	
	//Global Stats
	public bool systemOn = false;
	public Renderer objectsRender;
	
	public float updateSystemSpeed = 1;
	public float updateSystemTimer = 1;
	public float navMeshCountDownTimer = 0;
	public bool navMeshCountDown = false;
	public bool navMeshDisabled = true;
	
	public bool inHerd = false;
	public Transform animalToFollow;
	public int isAlpha;
	public int isAlphaOrNot = 2;
	public string animalNameType = "";
	public bool threatIsOutOfTigger = false;
	public List <GameObject> herdList = new List<GameObject>();
	public int herdNumber;
	public bool markInPack = false;
	public GameObject temp;
	public bool isDead = false;
	
	public GameObject fleeTarget;
	public bool calculateFlee = false; 
	public bool playSoundOnFlee = false;
	public AudioClip fleeSound;
	public Vector3 Direction;
	public bool distantFlee;
	public bool calculateWander = false;
	public float footStepSeconds = 0.15f;
	public float footStepSecondsWalk = 0.5f;
	public float runTimer = 0;
	public AudioClip runSound;
	public AudioClip walkSound;
	public int maxPackSize = 5;
	public bool packCreated = false;
	public int maxDistanceFromHerd = 100;
	public bool hasPack = false;
	public bool isExhausted = false;
	public float chaseTimer;
	public int chaseSeconds = 60;
	public bool herdFull = false;
	public bool waitingForHerd = false;
	public bool alignAI = true;
	public bool terrainFound = false;
	public Transform alignTarget;
	public bool alphaWaitForHerd = false;
	public bool attackingEnabled = false;
	
	public bool useInjuredSounds = false;
	public bool useImpactSounds = false;
	public bool useAttackSound = false;
	public bool useWeaponSound = false;
	public bool useRunSound = false;
	public bool useWalkSound = false;
	public bool autoGenerateAlpha = true;

	public bool useDeadReplacement = false;
	public bool animationPlayed = false;
	public bool isFollowing = false;
	public Transform followTransform;

	public bool isReadyForBreeding = false;
	public bool spawnedBreedEffect = false;
	public GameObject spawnedObject;
	public GameObject breedEffect;
	public bool mateFound = false;
	public Transform mateATransform;
	public Transform mateBTransform;
	public bool breedCoolDown = false;
	public float breedCoolDownTimer = 0;
	public float breedCoolDownSeconds = 60;		//How much time is needed before the animal can breed again
	public bool isBaby = false;
	public float breedEffectOffSetX = 0;
	public float breedEffectOffSetY = 0;
	public float breedEffectOffSetZ = 0;
	public Vector3 breedEffectOffSet;

	public float breedTimer = 0;
	public float breedSeconds = 5;		//How long does the breeding process take?

	public float cancelBreedTimer = 0;
	public float cancelBreedSeconds = 30;		//How many seconds does the animal have before the breeding process is canceled?

	public float babyTimer = 0;		
	public float babySeconds = 30;		//How long does it take for a baby to become an adult?

	public GameObject babyPrefabCommon;
	public GameObject babyPrefabUncommon;
	public GameObject babyPrefabRare;
	public GameObject babyPrefabSuperRare;

	public GameObject spawnedBabyObject;
	public GameObject fullGrownPrefab;
	public bool isBabyGiver = false;		//This is handled automatically. The first animal to be given the components to mate will have the baby.

	public List<AudioClip> animalSounds = new List<AudioClip>();
	public bool isPlayingAnimalSound = false;
	public bool useAnimalSounds = false;
	public float animalSoundTimer = 0;
	public float animalSoundMin = 4;
	public float animalSoundMax = 10;
	public float animalSoundWaitTime = 4;

	public double generatedOdds;
	public int AIType = 0;
	public Emerald_Animal_AI currentTargetSystem;
	public GameObject bloodEffect;
	public bool useBlood = false;
	public bool useRagdoll = false;

	public bool isPlayer = false;
	public float attackDelaySeconds = 0.5f;
	public int TabNumber;
	public int TabNumberAll;
	public bool returnsToStart = false;
	public bool useBreedEffect = false;

	public bool deathTrigger = false;
	public bool isTurning = false;
	public bool useTurnAnimation = false;
	public bool useDieSound = false;
	public float angle = 0;
	public int fleeType = 0;
	public bool targetInRange = false;
	public float deathAnimationTimer = 0;
	public bool deathAnimationFinished = false;
	public int UseBreeding = 1;
	public float followAlphaTimer = 0;

	public int commonOdds = 60;
	public int uncommonOdds = 25;
	public int rareOdds = 10;
	public int superRareOdds = 5;

	public float deathTimer = 0; 
	public int totalAIAround;
	int tempDamage;
	public float maxChaseDistance = 50;
	public bool returnBackToStartingPointProtection = false;
	public int maxFleeDistance = 40;
	public int rangedFleeDistance;
	public int rangedFleeAmount = 100;
	public bool canGetTired = false;
	public float fleeRandomnessTimer;
	public float fleeRandomness;
	public int stuck = 1;
	public string NPCName = "Deer";
	public bool autoCalculateDelaySeconds = false;
	public bool useHitAnimation = false;
	public float baseOffsetNav = 0;
	public int maximumWalkingVelocity = 30;
	public bool triggerColliderAutoGenerated = false;
	RaycastHit hit;
	GameObject tempSphere;
	
	void Awake()
	{
		startPosition = this.transform.position;
	
		if (!isFleeing)
		{
			grazeLength = Random.Range(grazeLengthMin, grazeLengthMax);
		}
		
		if (isFleeing)
		{
			grazeLength = Random.Range(grazeLengthMin, grazeLengthMax);
		}

		objectsRender = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

		if (triggerCollider == null)
		{
			tempSphere = new GameObject();
			tempSphere.name = "AI Trigger Collider";
			tempSphere.transform.parent = this.gameObject.transform;
			tempSphere.transform.localPosition = new Vector3(0,0,0);
			tempSphere.gameObject.layer = 2;
			tempSphere.AddComponent<SphereCollider>();
			triggerColliderAutoGenerated = true;
		}
	}


	void Start () 
	{
		EmeraldLayerMaskString = LayerMask.LayerToName(EmeraldLayerMask.value);
		PlayerLayerMaskString = LayerMask.LayerToName(PlayerLayerMask.value);

		//Apply and modify needed components on Start
		navMeshAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		triggerCollider = tempSphere.GetComponent<SphereCollider>();
		boxCollider = GetComponent<BoxCollider>();
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		gameObject.GetComponent<Rigidbody>().useGravity = false;

		//Multiple AudioSources need to be added to handle 
		//the amount of sounds that may be playing simultaneously
		//[0] dieSound, walkSound, runSound, injuredSounds
		//[1] weaponSound
		//[2] attackSounds, impactSounds
		for (int i = 0; i < 2; i++)
		{
			gameObject.AddComponent<AudioSource>();
		}

		foreach (AudioSource A in GetComponents<AudioSource>())
		{
			AudioSources.Add(A);
		}

		foreach (AudioSource A in AudioSources.ToArray())
		{
			A.minDistance = AudioSources[0].minDistance;
			A.maxDistance = AudioSources[0].maxDistance;
			A.spatialBlend = AudioSources[0].spatialBlend;
			A.rolloffMode = AudioSources[0].rolloffMode;
		}

		if (Physics.Raycast(new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z + 6), -Vector3.up, out hit)) 
		{
			if (hit.collider.gameObject.GetComponent<Terrain>() != null)
			{
				terrain = hit.collider.gameObject.GetComponent<Terrain>();
			}
			else
			{
				terrainFound = false;
				alignAI = false;
			}
		}

		//If you're using Emerald, your AI will have to have a tag according to your AI's tag settings. 
		if (this.gameObject.tag == "Untagged")
		{
			Debug.Log("Your AI's tag is marked as Untagged. You need to apply a proper tag name according to your AI's Tag Options.");
		}

		//Randomize our Animal's sounds so they aren't playing on Start
		if (useAnimalSounds)
		{
			animalSoundWaitTime = (int)Random.Range(animalSoundMin, animalSoundMax);
		}
	
		huntTimer = chaseSeconds;

		//Apply a LineRender if using the Draw Paths feature
		if (GetComponent<LineRenderer>() == null && drawPaths)
		{
			this.gameObject.AddComponent<LineRenderer>();
		}
			
		boxCollider.enabled = true;
		
		triggerCollider.isTrigger = true;
		triggerCollider.center = new Vector3(0,0,0);

		if (aggression == 0)
		{
			triggerCollider.radius = fleeRadius * transform.localScale.x;
		}
		
		if (aggression == 1)
		{
			triggerCollider.radius = fleeRadius * transform.localScale.x;
		}

		if (aggression == 2)
		{
			triggerCollider.radius = wanderRange;
		}
		
		if (aggression == 3)
		{
			triggerCollider.radius = huntRadius * transform.localScale.x;
		}

		if (aggression == 4)
		{
			triggerCollider.radius = huntRadius * transform.localScale.x;
		}
			
		//If Draw Waypoints is enabled, apply and adjust needed components.
		//Put all waypoints in a parent so they don't clutter the hierarchy
		if (drawWaypoints)
		{
			currentWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			currentWaypoint.gameObject.transform.localScale = new Vector3 (0.75f, 0.75f, 0.75f);
			currentWaypoint.GetComponent<Renderer>().material.color = pathColor;
			currentWaypoint.name = this.gameObject.name + " Waypoint";

			currentWaypoint.GetComponent<SphereCollider>().enabled = false;
			currentWaypoint.AddComponent<AlignWaypoint>();

			waypointHolder = GameObject.Find("Emerald Waypoints");

			if (waypointHolder == null)
			{
				waypointHolder = new GameObject("Emerald Waypoints");
			}

			if (waypointHolder != null)
			{
				currentWaypoint.transform.parent = waypointHolder.transform;
			}
		}

		//Apply the custom Line material for drawing the AI's path
		if (drawPaths)
		{
			line = GetComponent<LineRenderer>();
			line.material = pathMaterial;
		}

		//Set our AI's settings from the Editor
		navMeshAgent.angularSpeed = turnSpeed;
		navMeshAgent.stoppingDistance = stoppingDistance;
		navMeshAgent.baseOffset = baseOffsetNav;
		navMeshAgent.acceleration = 40;

		//1.3.5
		//stoppingDistance += 0.6f;

		currentHealth = startingHealth;
		currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);

		//Check all of our animations to ensure that there are none missing. 
		//If there are, replace them with some of the animations already set. If these are also missing,
		//disable animations and Debug.Log them to the Unity console to help users apply the missing animations.
		if (useAnimations)
		{
			anim = GetComponent<Animation>();

			if (anim != null && AnimationType == 1)
			{
				if (walkAnimation != null && anim.GetClip(walkAnimation.name) != null){
				anim[walkAnimation.name].speed = walkAnimationSpeed;
				}
				else{
					useAnimations = false;
					if (walkAnimation == null){
					Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"A Walk Animation has not been assigned to the Walk Animation slot in Emerald Editor under Animation Options, animations have been disabled. To fix this, plaese assign one.");
					}
					else if (anim.GetClip(walkAnimation.name) == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"The Walk Animation you assigned in Emerald Editor under Animation Options cannot be found on your AI's Animation Component, animations have been disabled. To fix this, ensure that the animation is assigned to your AI's Animation Component.");
					}
				}

				if (runAnimation != null && anim.GetClip(runAnimation.name) != null){
				anim[runAnimation.name].speed = runAnimationSpeed;
				}
				else{
					useAnimations = false;
					if (runAnimation == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"A Run Animation has not been assigned to the Run Animation slot in Emerald Editor under Animation Options, animations have been disabled. To fix this, plaese assign one.");
					}
					else if (anim.GetClip(runAnimation.name) == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"The Run Animation you assigned in Emerald Editor under Animation Options cannot be found on your AI's Animation Component, animations have been disabled. To fix this, ensure that the animation is assigned to your AI's Animation Component.");
					}
				}

				if (idleAnimation != null && anim.GetClip(idleAnimation.name) != null){
				anim[idleAnimation.name].speed = idleAnimationSpeed;
				}
				else{
					useAnimations = false;
					if (idleAnimation == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"An Idle Animation has not been assigned to the Idle Animation slot in Emerald Editor under Animation Options, animations have been disabled. To fix this, plaese assign one.");
					}
					else if (anim.GetClip(idleAnimation.name) == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"The Idle Animation you assigned in Emerald Editor under Animation Options cannot be found on your AI's Animation Component, animations have been disabled. To fix this, ensure that the animation is assigned to your AI's Animation Component.");
					}
				}

				if (graze1Animation != null && anim.GetClip(graze1Animation.name) != null){
					anim[graze1Animation.name].speed = grazeAnimationSpeed;
				}
				else{
					useAnimations = false;
					if (graze1Animation == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"A Graze Animation has not been assigned to the Graze 1 Animation slot in Emerald Editor under Animation Options, animations have been disabled. To fix this, plaese assign one.");
					}
					else if (anim.GetClip(graze1Animation.name) == null){
						Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"The Graze 1 Animation you assigned in Emerald Editor under Animation Options cannot be found on your AI's Animation Component, animations have been disabled. To fix this, ensure that the animation is assigned to your AI's Animation Component.");
					}
				}

				if (graze2Animation != null && anim.GetClip(graze2Animation.name) != null)
				{
					anim[graze2Animation.name].speed = grazeAnimationSpeed;
				}
				else{
					graze2Animation = graze1Animation;
				}

				if (graze3Animation != null && anim.GetClip(graze3Animation.name) != null)
				{
					anim[graze3Animation.name].speed = grazeAnimationSpeed;
				}
				else{
					graze3Animation = graze1Animation;
				}

				if (!useDeadReplacement)
				{
					if (deathAnimation != null && anim.GetClip(deathAnimation.name) != null)
					{
						anim[deathAnimation.name].speed = dieAnimationSpeed;
						anim[deathAnimation.name].wrapMode = WrapMode.ClampForever;
					}
				}

				if (useHitAnimation)
				{
					if (hitAnimation != null && anim.GetClip(hitAnimation.name) != null)
					{
						anim[hitAnimation.name].wrapMode = WrapMode.Once;
					}
					else{
						useHitAnimation = false;
					}
				}

				if (useTurnAnimation){
					if (turnAnimation == null || anim.GetClip(turnAnimation.name) == null)
					{
						useTurnAnimation = false;
					}
				}

				if (aggression >= 3){

					if (useRunAttackAnimations)
					{
						if (runAttackAnimation != null && anim.GetClip(runAttackAnimation.name) != null)
						{
							anim[runAttackAnimation.name].wrapMode = WrapMode.Once;
						}
						else{
							useRunAttackAnimations = false;
						}
					}

					if (idleBattleAnimation != null && anim.GetClip(idleBattleAnimation.name) != null)
					{
						anim[idleBattleAnimation.name].speed = idleCombatAnimationSpeed;
					}
					else{
						idleBattleAnimation = idleAnimation;
					}

					if (attackAnimation1 != null && anim.GetClip(attackAnimation1.name) != null)
					{
						anim[attackAnimation1.name].wrapMode = WrapMode.Once;
						anim[attackAnimation1.name].speed = attackAnimationSpeed;
					}
					else{
						useAnimations = false;
						if (graze1Animation == null){
							Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"An Attack Animation has not been assigned to the Attack 1 Animation slot in Emerald Editor under Animation Options, animations have been disabled. To fix this, plaese assign one.");
						}
						else if (anim.GetClip(graze1Animation.name) == null){
							Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>"+"The Attack 1 Animation you assigned in Emerald Editor under Animation Options cannot be found on your AI's Animation Component, animations have been disabled. To fix this, ensure that the animation is assigned to your AI's Animation Component.");
						}
					}

					if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null)
					{
						anim[attackAnimation2.name].wrapMode = WrapMode.Once;
						anim[attackAnimation2.name].speed = attackAnimationSpeed;
					}

					if (attackAnimation3 != null && anim.GetClip(attackAnimation3.name) != null)
					{
						anim[attackAnimation3.name].wrapMode = WrapMode.Once;
						anim[attackAnimation3.name].speed = attackAnimationSpeed;
					}

					if (attackAnimation4 != null && anim.GetClip(attackAnimation4.name) != null)
					{
						anim[attackAnimation4.name].wrapMode = WrapMode.Once;
						anim[attackAnimation4.name].speed = attackAnimationSpeed;
					}

					if (attackAnimation5 != null && anim.GetClip(attackAnimation5.name) != null)
					{
						anim[attackAnimation5.name].wrapMode = WrapMode.Once;
						anim[attackAnimation5.name].speed = attackAnimationSpeed;
					}

					if (attackAnimation6 != null && anim.GetClip(attackAnimation6.name) != null)
					{
						anim[attackAnimation6.name].wrapMode = WrapMode.Once;
						anim[attackAnimation6.name].speed = attackAnimationSpeed;
					}
				}
			}

			//Setup our Mecanim components
			if (anim == null && AnimationType == 1)
			{
				Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>There is no Animation Component attached to your AI, animations have been disabled. If you would like to use animations, " +
				          "please assign an Animation Component, with your model's animations, and assign the apporptiate animations to the Emerald Editor under Animation Options.");
				useAnimations = false;
			}

			if (AnimationType == 2 && AnimatorComponent == null)
			{
				AnimatorComponent = GetComponent<Animator>();

				if (AnimatorComponent.runtimeAnimatorController == null){
					Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>There is no Runtime Animator Controller attached to your Animator Component, animations have been disabled. If you would like to use animations, " +
					          "please assign a Runtime Animator Controller to the Animator Component on this AI gameobject. You can use the one Emerald generated or you can use the default Runtime Controller called Emerald Controller.");
					useAnimations = false;
					useMecanim = false;
				}
				
				if (AnimatorComponent.avatar == null){
					Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>There is no Avatar attached to your Animator Component, animations have been disabled. If you would like to use animations, " +
					          "please assign your model's Avatar to the Animator Component on this AI gameobject.");
					useAnimations = false;
					useMecanim = false;
				}
			}

			if (AnimatorComponent == null && AnimationType == 2 || AnimatorController == null && AnimationType == 2)
			{
				if (AnimatorComponent == null && AnimatorController != null)
				{
					AnimatorComponent = GetComponent<Animator>();
				}
				if (AnimatorController == null){
					Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>There is no Animator Controller attached to your AI, animations have been disabled. If you would like to use animations, " +
					          "please generate an Animator Controller in the Emerald Editor under Animation Options with the 'Generate Animator Controller' button.");
					useAnimations = false;
					useMecanim = false;
				}
				if (AnimatorComponent == null){
					Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>There is no Animator Component attached to your AI, animations have been disabled. If you would like to use animations, " +
					          "please attach an Animator Component to your AI gameobject.");
					useAnimations = false;
					useMecanim = false;
				}
				else if (AnimatorComponent == null && AnimatorController == null)
				{
					useAnimations = false;
					useMecanim = false;
					Debug.Log("<color=red>Animation Error on the '" +gameObject.name+"' gameobject:</color>There is no Animator Component or Animator Controller attached to your AI, animations have been disabled. If you would like to use animations, " +
					          "please assign an Animator Component and generate an Animator Controller in the Emerald Editor under Animation Options.");
				}
			}

			if (AnimatorComponent != null && AnimationType == 2 && AnimatorController != null)
			{
				useMecanim = true;
				if (useRootMotion){
					AnimatorComponent.applyRootMotion = true;
				}
				else if (!useRootMotion){
					AnimatorComponent.applyRootMotion = false;
				}
			}

			if (anim != null)
			{
				if (AnimationType == 1)
				{
					anim.enabled = false;
				}
			}
		}
				
		if(useDustEffect)
		{
			clone = Instantiate(dustEffect, new Vector3 (transform.position.x, transform.position.y + 0.35f, transform.position.z + 1.0f), Quaternion.identity) as ParticleSystem;
			clone.transform.parent = transform;

			#if UNITY_5_5 || UNITY_5_6
			ParticleSystem.EmissionModule emission = clone.emission;
			emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
			#elif UNITY_5_3 || UNITY_5_4
			ParticleSystem.EmissionModule emission = clone.emission;
			emission.rate = new ParticleSystem.MinMaxCurve(0f);
			#elif UNITY_5_2
			clone.emissionRate = 0;
			#endif
		}
		
		if (drawPaths)
		{
			pathWidthAdjusted = pathWidth * 0.01f;
			
			line.SetWidth(pathWidthAdjusted, pathWidthAdjusted);
			line.SetColors(pathColor, pathColor);
		}
		
		if (drawPaths)
		{
			GetPath();
		}
		
		if (aggression == 2)
		{
			Wander();
		}

		//Setup our animations
		if (aggression >= 3)
		{
			if (useAnimations && !useMecanim){
				ApplyAttackAnimations();
			}
		}

		grazeAnimationNumber = Random.Range(1,totalGrazeAnimations+1);
		attackTime = Random.Range(attackTimeMin, attackTimeMax);

		//Setup or generate our Alphas 
		if (autoGenerateAlpha)
		{
			isAlpha = Random.Range(0,5);
			
			//Not Alpha
			if (isAlpha <= 3)
			{
				isAlpha = 0;
			}
			
			//Is Alpha
			if (isAlpha > 3)
			{
				isAlpha = 1;
			}
		}
		
		if (!autoGenerateAlpha)
		{
			if (isAlphaOrNot == 1)
			{
				isAlpha = 1;
			}
			
			if (isAlphaOrNot == 2)
			{
				isAlpha = 0;
			}
		}
		
		fleeTimer = (float)extraFleeSeconds;
		maxPackSize = maxPackSize - 1;

		AudioSources[0].enabled = false;
		AudioSources[1].enabled = false;
		AudioSources[2].enabled = false;
		triggerCollider.enabled = false;
		triggerCollider.enabled = true;
		navMeshAgent.enabled = false;
		
		//Waypoints need to be enabled in order for paths to be drawn
		if (drawPaths)
		{
			drawWaypoints = true;
		}
		
		if (alignAI)
		{
			originalLookRotation = transform.rotation; 
			alignTarget = this.transform;
		}
			
		//Try once more to get the terrain
		if (terrain == null)
		{
			if (Physics.Raycast(new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z), -Vector3.up, out hit)) 
			{
				if (hit.collider.gameObject.GetComponent<Terrain>() != null)
				{
					terrain = hit.collider.gameObject.GetComponent<Terrain>();
				}
				else
				{
					terrainFound = false;
					alignAI = false;
				}
			}
		}
	}

	//When the appropriate time is reached, play an animal sound. This only happens for Passive Animals
	public void PlayAnimalSound ()
	{
		AudioSources[1].PlayOneShot(animalSounds[Random.Range(0,animalSounds.Count)]);
		animalSoundWaitTime = (int)Random.Range(animalSoundMin, animalSoundMax);
		animalSoundTimer = 0;
		isPlayingAnimalSound = false;
	}

	//This follow function is used when a Player uses food to get an animal to follow them. This function can also be used for custom purposes, if desired.
	public void Follow ()
	{
		if (systemOn && !isFleeing && isFollowing)
		{
			if (Vector3.Distance(this.transform.position, followTransform.position) > stoppingDistance)
			{
				destination = new Vector3(followTransform.position.x, this.transform.position.y, followTransform.position.z);
				NewDestination(destination);
			}

			if (navMeshAgent.remainingDistance > stoppingDistance){
				if (useAnimations && !isCoolingDown)
				{
					if (!useMecanim) 
					{
						anim.CrossFade (walkAnimation.name);
					}

					if (useMecanim)
					{
						MecanimWalk();
					}
				}
			}

			if (navMeshAgent.remainingDistance <= stoppingDistance)
			{
				if (useAnimations)
				{
					if(!useMecanim){
						anim.CrossFade(idleAnimation.name);
					}
					else if(useMecanim){
						MecanimGraze1();
					}
				}
			}
			
			isGrazing = false;
			
			navMeshAgent.speed = walkSpeed;
			
			if (drawWaypoints)
			{
				currentWaypoint.transform.position = destination;
			}
		}
	}

	//The wandering function AI use to dynamically generate waypoints
	void Wander ()
	{
		if (systemOn && !isFleeing && !isFollowing && !isDead)
		{
			destination = startPosition + new Vector3(Random.Range ((int)-wanderRange * 0.5f - 2, (int)wanderRange * 0.5f + 2), 0, Random.Range ((int)-wanderRange * 0.5f - 2, (int)wanderRange * 0.5f + 2));
			navMeshAgent.ResetPath();

			if (terrainFound && alignAI)
			{
				destination.y = terrain.SampleHeight(destination);
				destination = new Vector3(destination.x, destination.y + terrain.transform.position.y, destination.z);
			}
				
			NewDestination(destination);

			huntMode = false;
			damageDealt = false;
			withinAttackDistance = false;
			attackWhileRunning = false;
			startHuntTimer = false;
			
			if (useAnimations && !isCoolingDown)
			{
				if (!useMecanim) 
				{
					anim.CrossFade (walkAnimation.name);
				}

				if (useMecanim)
				{
					MecanimWalk();
				}
			}
			
			isGrazing = false;
			
			navMeshAgent.speed = walkSpeed;
			
			if (drawWaypoints)
			{
				currentWaypoint.transform.position = destination;
			}
		}
	}

	//The fleeing function AI use. AI will generate waypoints in the opposite direction of their chaser to avoid danger.
	void Flee()
	{
		isGrazing = false;

		//If our velocity drops near 0, play our idle animation. This is to stop our AI from running or walking when not in motion
		if (useAnimations && !withinAttackDistance)
		{
			velocity = navMeshAgent.velocity.sqrMagnitude;

			if (!useMecanim)
			{
				if (velocity < 0.1f && anim.IsPlaying(runAnimation.name))
				{
					anim.CrossFade (idleAnimation.name);
				}
			}

			if (useMecanim)
			{
				if (velocity < 0.1f && AnimatorComponent.GetBool(RunParameter))
				{
					MecanimIdle();
				}
			}
		}

		//If our AI are in a herd, have the herd members have the same fleeing target as the Alpha
		if (isAlpha == 0 && inHerd && fleeTarget == null && aggression < 2)
		{
			fleeTarget = animalToFollow.GetComponent<Emerald_Animal_AI>().fleeTarget;
		}

		//If we become stuck, or our waypoint leads to nowhere, generate a new waypoint.
		if (fleeTarget != null && isAlpha == 0 && !inHerd || fleeTarget != null && isAlpha == 1)
		{
			if (stuck == 2)
			{
				fleeRandomnessTimer += Time.deltaTime;
			}

			if (fleeRandomnessTimer >= 0.25f)
			{
				fleeRandomness = Random.Range(-1,-15);
				fleeRandomnessTimer = 0;
				stuck = 1;
			}

			if (navMeshAgent.remainingDistance < 5)
			{
				navMeshAgent.ResetPath();

				Vector3 direction = (transform.position - fleeTarget.transform.position).normalized;
				destination = transform.position + direction * 80 + Random.insideUnitSphere * 40;
				NewDestination(destination);

				if (drawWaypoints)
				{
					currentWaypoint.transform.position = destination;
				}
			}

			if (Vector3.Distance(transform.position, fleeTarget.transform.position) < 5 && navMeshAgent.remainingDistance < 5)
			{
				destination = transform.position + transform.forward * 50;
				navMeshAgent.SetDestination(destination);
			}

			if (!navMeshAgent.hasPath && stuck == 1 || navMeshAgent.isPathStale && stuck == 1 || navMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid && stuck == 1 || navMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial && stuck == 1)
			{
				navMeshAgent.ResetPath();

				Vector3 direction = (fleeTarget.transform.position - transform.position).normalized;
				destination = transform.position + -direction * 50 + Random.insideUnitSphere * 40;
				NewDestination(destination);

				if (useAnimations)
				{
					if (!useMecanim) 
					{
						anim.CrossFade (idleAnimation.name);
					}

					if (useMecanim)
					{
						MecanimIdle();
					}
				}

				stuck = 2;
			}

			//If our path isn't blocked, continuously generate waypoints when within distance of 15 units or less
			if (navMeshAgent.hasPath && stuck == 1 || !navMeshAgent.isPathStale && stuck == 1 || navMeshAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathInvalid && stuck == 1 || navMeshAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathPartial && stuck == 1)
			{
				if (Vector3.Distance(transform.position, destination) < 15)
				{
					Vector3 direction = (fleeTarget.transform.position - transform.position).normalized;
					destination = transform.position + -direction * 50 + Random.insideUnitSphere * 40;
					NewDestination(destination);

					if (useAnimations && velocity > 0.1f && !isTurning)
					{
						navMeshAgent.speed = runSpeed;

						if (useAnimations)
						{
							if (!useMecanim) 
							{
								anim.CrossFade (runAnimation.name);
							}

							if (useMecanim)
							{
								MecanimRun();
							}
						}
					}
					
					if (!useAnimations)
					{
						navMeshAgent.speed = runSpeed;
					}

					if (drawWaypoints)
					{
						currentWaypoint.transform.position = destination;
					}
				}
			}

			if (useAnimations && velocity > 0.1f && !isTurning)
			{
				navMeshAgent.speed = runSpeed;

				if (useAnimations)
				{
					if (!useMecanim) 
					{
						anim.CrossFade (runAnimation.name);
					}

					if (useMecanim)
					{
						MecanimRun();
					}
				}
			}
		}
	}

	//The Breed function is what allows an AI to breed, if the proper conditions have been met. 
	//The function will roll a number to decide what animal will be bred. This is based off the user's percentages in the Emerald Editor.
	public void Breed ()
	{
		//If mate A is killed or lost, reset settings and start cooldown. 
		if (mateFound && !breedCoolDown && isBabyGiver && mateATransform == null)
		{
			isReadyForBreeding = false;
			spawnedBreedEffect = false;
			isBabyGiver = false;
			breedTimer = 0;
			Destroy(spawnedObject);
			Wander ();
			mateFound = false;
			breedCoolDown = true;
		}

		//If mate B is killed or lost, reset settings and start cooldown. 
		if (mateFound && !breedCoolDown && !isBabyGiver && mateBTransform == null)
		{
			isReadyForBreeding = false;
			spawnedBreedEffect = false;
			isBabyGiver = false;
			breedTimer = 0;
			Destroy(spawnedObject);
			Wander ();
			mateFound = false;
			breedCoolDown = true;
		}
			
		if (mateFound && !breedCoolDown && isBabyGiver && mateATransform != null)
		{
			destination = new Vector3(mateATransform.position.x + 1, mateATransform.position.z);
			NewDestination(destination);

			if (navMeshAgent.remainingDistance <= stoppingDistance)
			{
				breedTimer += Time.deltaTime;

				if (useAnimations)
				{
					if(!useMecanim){
						anim.CrossFade(idleAnimation.name);
					}
					else if(useMecanim){
						MecanimGraze1();
					}
				}

				if (breedTimer >= breedSeconds)
				{
					//Roll a random float between 0 and 1
					generatedOdds = System.Math.Round(Random.value,2);

					//Calculate our breeding odds by converting the Editor numbers to decimals then finding the percentage difference
					float CalculatedUncommon = (((uncommonOdds * 0.01f)) - 1) * -1;
					float CalculatedRare = (((rareOdds * 0.01f)) - 1) * -1;
					float CalculatedSuperRare = (((superRareOdds * 0.01f)) - 1) * -1;

					//Spawn babies according to calculated Editor odds
					if(generatedOdds <= commonOdds*0.01f || generatedOdds <= CalculatedUncommon) 
					{ 
						spawnedBabyObject = Instantiate(babyPrefabCommon, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), transform.rotation) as GameObject;
					}

					if(generatedOdds > CalculatedUncommon && generatedOdds < CalculatedRare) 
					{ 
						spawnedBabyObject = Instantiate(babyPrefabUncommon, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), transform.rotation) as GameObject;
					}

					if(generatedOdds >= CalculatedRare && generatedOdds < CalculatedSuperRare) 
					{ 
						spawnedBabyObject = Instantiate(babyPrefabRare, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), transform.rotation) as GameObject;
					}

					if(generatedOdds >= CalculatedSuperRare) 
					{ 
						spawnedBabyObject = Instantiate(babyPrefabSuperRare, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), transform.rotation) as GameObject;
					}

					//Set needed setting after successful breeding
					isReadyForBreeding = false;
					spawnedBreedEffect = false;
					isBabyGiver = false;
					breedTimer = 0;
					Destroy(spawnedObject);
					Wander ();
					mateATransform.gameObject.GetComponent<Emerald_Animal_AI>().isReadyForBreeding = false;
					mateATransform.gameObject.GetComponent<Emerald_Animal_AI>().spawnedBreedEffect = false;
					mateATransform.gameObject.GetComponent<Emerald_Animal_AI>().mateFound = false;
					mateATransform.gameObject.GetComponent<Emerald_Animal_AI>().breedCoolDown = true;
					Destroy(mateATransform.gameObject.GetComponent<Emerald_Animal_AI>().spawnedObject);
					mateFound = false;
					breedCoolDown = true;
				}
			}
		}
	}
	
	public void NewDestination(Vector3 targetPoint)
	{
		if (navMeshAgent.enabled)
		{
			navMeshAgent.SetDestination (targetPoint);
		}
	}
	
	//Calculates our path lines, if they are enabled
	void GetPath()
	{
		if (drawPaths)
		{
			line.SetPosition(0, new Vector3(transform.localPosition.x, transform.position.y + lineYOffSet, transform.position.z)); //set the line's origin
			DrawPath(navMeshAgent.path);
		}
	}
	
	//Draws our path lines
	public void DrawPath(UnityEngine.AI.NavMeshPath path)
	{
		if (drawPaths)
		{
			if(path.corners.Length < 1) 
				return;
			
			#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			line.SetVertexCount(path.corners.Length); 
			#elif UNITY_5_5 || UNITY_5_6
			line.positionCount = path.corners.Length;
			#endif
			
            try
            {
                for (int i = 1; i < path.corners.Length; i++)
                {
                    line.SetPosition(i, path.corners[i]);
                }
            }
            catch
            {

            }
		}
	}
	
	//The SystemOptimizerUpdater checks to see if our AI are still in view every few seconds.
	//If they are no longer in view, according to Unity's LOD System, disable all components until back in view.
	//Using the NavMesh timer, check to see that our AI are off screen for at least 10 seconds before deactivating.
	//If the AI are out of view, set systemOn to false, which stops Emerald from calculating
	void SystemOptimizerUpdater ()
	{
		updateSystemTimer += Time.deltaTime;			//Use this to keep track of the amount of seconds until we can update

		if (updateSystemTimer >= updateSystemSpeed && !isDead)			//If the update seconds have reached the amount of desired update seconds, update our system optimizer						
		{
			if (objectsRender.isVisible)					//If the AI's renderer is visible, enabled our components
			{
				systemOn = true;
				navMeshAgent.enabled = true;
				navMeshDisabled = false;
				triggerCollider.enabled = true;
				boxCollider.enabled = true;
				AudioSources[0].enabled = true;
				AudioSources[1].enabled = true;
				AudioSources[2].enabled = true;
				
				if (useAnimations && AnimationType == 1)
				{
					anim.enabled = true;
				}
					
				else if (useAnimations && AnimationType == 2)
				{
					AnimatorComponent.enabled = true;
				}
				
				navMeshCountDownTimer = 0;
				updateSystemTimer = 0;						//The updateSystemTimer has be ticked, restart.
			}
			
			if (!objectsRender.isVisible && !huntMode)				//If the AI's renderer is not visible, disable our components so that they aren't waisting performance when they aren't visible		
			{
				navMeshCountDown = true;				
				updateSystemTimer = 0;						//The updateSystemTimer has be ticked, restart.
			}
		}
		
		if (navMeshCountDown && !navMeshDisabled)	
		{													//If not visible, enabled our navMeshCountDown (This only allows the NavMesh component to be enabled when 15 seconds have passed)												
			navMeshCountDownTimer += Time.deltaTime;		//This is to avoid the NavMesh from being disabled from simply looking away from the AI for a second or two
			
			if (navMeshCountDownTimer >= deactivateAISeconds && !huntMode && !isFleeing)
			{
				navMeshAgent.enabled = false;
				AudioSources[0].enabled = false;
				AudioSources[1].enabled = false;
				AudioSources[2].enabled = false;
				systemOn = false;
				navMeshDisabled = true;

				triggerCollider.enabled = false;
				boxCollider.enabled = false;
				
				if (useAnimations)
				{
					if (!useMecanim)
					{
						anim.Stop();
						anim.enabled = false;
					}

					else if (useMecanim)
					{
						AnimatorComponent.enabled = false;
					}
				}
				
				navMeshCountDownTimer = 0;
				updateSystemTimer = 0;						//The updateSystemTimer has be ticked, restart.
			}
		}
	}
	
	void MainSystem ()
	{
		//Calls our following function, if the proper conditions have been met.
		if (isFollowing && !mateFound && !breedCoolDown && !isBaby && EmeraldTags.Contains(followTransform.gameObject.tag))
		{
			Follow ();

			if (followTransform.gameObject.activeSelf == false)
			{
				isFollowing = false;
			}
		}

		//If our AI is a baby, count to the necessary babySeconds. Once they are reached, transform into a full grown adult. 
		//If this causes an error, it's because you don't have an object in the fullGrownPrefab slot in the Editor.
		if (isBaby)
		{
			babyTimer += Time.deltaTime;

			if (babyTimer >= babySeconds)
			{
				Instantiate(fullGrownPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
				Destroy(gameObject);
			}
		}

		//If Use Animal Sounds is enabled, play a random animal sound according to the animal sound wait time.
		if (useAnimalSounds)
		{
			animalSoundTimer += Time.deltaTime;

			if (animalSoundTimer >= animalSoundWaitTime)
			{
				if (animalSounds.Count > 0)
				{
					PlayAnimalSound();
				}
			}
		}

		//Spawn our breeding effect, when an animal is breeding, if it is enabled.
		if (!isBaby && isReadyForBreeding && !breedCoolDown)
		{
			if (!spawnedBreedEffect)
			{
				boxCollider.enabled = false;
				spawnedObject = Instantiate(breedEffect, new Vector3(transform.position.x + breedEffectOffSet.x, transform.position.y + breedEffectOffSet.y, transform.position.z + breedEffectOffSet.z), Quaternion.Euler(0, 0, 0)) as GameObject;
				spawnedObject.transform.parent = this.transform;
				spawnedObject.transform.position = new Vector3(transform.position.x + breedEffectOffSet.x, transform.position.y + breedEffectOffSet.y, transform.position.z + breedEffectOffSet.z);
				boxCollider.enabled = true;
				spawnedBreedEffect = true;
			}

			if (navMeshAgent.remainingDistance <= stoppingDistance)
			{
				if (useAnimations)
				{
					if(!useMecanim){
						anim.CrossFade(idleAnimation.name);
					}
					else if(useMecanim){
						MecanimGraze1();
					}
				}
			}

			//If no animal is found when breed mode is activated by the time cancelBreedSeconds is met, cancel the breeding and set isReadyForBreeding to false
			cancelBreedTimer += Time.deltaTime;

			if (cancelBreedTimer >= cancelBreedSeconds)
			{
				cancelBreedTimer = 0;
				Destroy(spawnedObject);
				isReadyForBreeding = false;
			}

			//If the proper conditions are met, call a successful breed.
			Breed();
		}

		//After a successful breed, wait for the breedCoolDownSeconds to be met for it to be possible for an AI to breed again.
		if (breedCoolDown)
		{
			breedCoolDownTimer += Time.deltaTime;

			if (breedCoolDownTimer >= breedCoolDownSeconds)
			{
				breedCoolDownTimer = 0;
				breedTimer = 0;
				breedCoolDown = false;
			}
		}
			
		if (navMeshAgent.enabled && navMeshAgent.remainingDistance <= stoppingDistance && !isFleeing && aggression <= 2 || navMeshAgent.enabled && navMeshAgent.remainingDistance <= stoppingDistance && !huntMode && aggression >= 3) 
		{
			isGrazing = true;
		}

		//If our animal is grazing, and useAnimations is enabled, pick a random graze animation to play.
		if (isGrazing)
		{
			Graze();		
		}
		
		if (isFleeing && !rotateFlee && !inHerd && isAlpha == 0 || isFleeing && !rotateFlee && isAlpha == 1)
		{
			rotateTimer += Time.deltaTime;
			
			if (!isDead && rotateTimer <= 1f && navMeshAgent.enabled)
			{
				Flee();
			}
			
			if (rotateTimer > 1f)
			{
				rotateFlee = true;
			}
		}
			
		if (!isDead && navMeshAgent.enabled == true)
		{
			if (navMeshAgent.remainingDistance >= stoppingDistance && !isFleeing && !isCoolingDown && aggression <= 2 || navMeshAgent.remainingDistance >= stoppingDistance && !huntMode && !isCoolingDown && aggression >= 3)
			{
				if (useAnimations)
				{
					if (!useMecanim) 
					{
						anim.CrossFade (walkAnimation.name); //Updated
					}

					if (useMecanim)
					{
						MecanimWalk();
					}
				}
				
				isGrazing = false;
			}
		}

		//Footstep Sounds
		Footsteps();
		DustEffects();

		//If wandering, play our walk animation.
		if (!isFleeing && !isGrazing && !huntMode && !isCoolingDown && !isDead)
		{
			if (useAnimations)
			{
				if (!useMecanim) 
				{
					anim.CrossFade (walkAnimation.name);
				}

				if (useMecanim)
				{
					MecanimWalk();
				}
			}
			
			navMeshAgent.speed = walkSpeed;
		}

		//Draw our current waypoint path, if drawPaths is enabled.
		if (drawPaths)
		{
			UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
			navMeshAgent.CalculatePath(currentWaypoint.transform.position, path);
			GetPath();
		}

		//If an animal is using the Time based flee type, flee until the time is met. Once time is reached, the animal is exhausted.
		if (startFleeTimer && threatIsOutOfTigger && AIType == 0 && fleeType == 1)
		{
			fleeTimer -= Time.deltaTime;
			
			if (fleeTimer <= 0)
			{
				fleeTarget = null;
				isExhausted = true;
				isFleeing = false;
				calculateFlee = false;
				currentlyBeingPursued = false;
				threatIsOutOfTigger = false;
				startFleeTimer = false;
				distantFlee = false;

				if (isAlpha == 1)
				{
					foreach (GameObject G in herdList) 
					{
						if (G != null)
						{
							if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
							{
								G.GetComponent<Emerald_Animal_AI>().fleeTarget = null;
								G.GetComponent<Emerald_Animal_AI>().isFleeing = false;
								G.GetComponent<Emerald_Animal_AI>().calculateFlee = false;
								G.GetComponent<Emerald_Animal_AI>().distantFlee = false;
							}
						}
					}
				}
			}
		}

		//If an AI is using the Distance based flee type, flee until out of range of the chaser. Once out of range is reached, wander.
		if (isFleeing && fleeType == 0 && fleeTarget != null && threatIsOutOfTigger && !inHerd)
		{
			float distance = Vector3.Distance (this.transform.position, fleeTarget.transform.position);

			if (distance > maxFleeDistance)
			{
				fleeTarget = null;
				isFleeing = false;
				calculateFlee = false;
				currentlyBeingPursued = false;
				threatIsOutOfTigger = false;
				startFleeTimer = false;
				distantFlee = false;

				if (isAlpha == 1)
				{
					foreach (GameObject G in herdList) 
					{
						if (G != null)
						{
							if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
							{
								G.GetComponent<Emerald_Animal_AI>().fleeTarget = null;
								G.GetComponent<Emerald_Animal_AI>().isFleeing = false;
								G.GetComponent<Emerald_Animal_AI>().calculateFlee = false;
								G.GetComponent<Emerald_Animal_AI>().distantFlee = false;
							}
						}
					}
				}
			}
		}

		//If an AI is using the Distance based flee type, flee until out of range of the chaser. Once out of range is reached, wander.
		if (isFleeing && fleeTarget != null && startRangedFlee && animalToFollow == null)
		{
			float distance = Vector3.Distance (this.transform.position, fleeTarget.transform.position);

			if (distance > rangedFleeDistance)
			{
				fleeTarget = null;
				isFleeing = false;
				calculateFlee = false;
				currentlyBeingPursued = false;
				threatIsOutOfTigger = false;
				startFleeTimer = false;
				distantFlee = false;
				startRangedFlee = false;

				if (isAlpha == 1)
				{
					foreach (GameObject G in herdList) 
					{
						if (G != null)
						{
							if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
							{
								G.GetComponent<Emerald_Animal_AI>().fleeTarget = null;
								G.GetComponent<Emerald_Animal_AI>().isFleeing = false;
								G.GetComponent<Emerald_Animal_AI>().calculateFlee = false;
								G.GetComponent<Emerald_Animal_AI>().distantFlee = false;
							}
						}
					}
				}
			}
		}

		//Once an AI's huntTimer has reached 0, stop HuntMode and wander.
		if (startHuntTimer && !withinAttackDistance)
		{
			huntTimer -= Time.deltaTime;
			
			if (huntTimer <= 0)
			{
				if (isAlpha == 0 && !inHerd || isAlpha == 1)
				{
					//If returnsToStart is enabled, the AI will return back to its starting position and wander.
					if (returnsToStart)
					{
						ReturnBackToStartingPoint();
					}

					//If returnsToStart is disabled, the AI will wander at the position of when its HuntMode was disabled
					if (!returnsToStart)
					{
						startHuntTimer = false;
						huntMode = false;
						currentAnimal = null;
						navMeshAgent.speed = walkSpeed;
						huntTimer = chaseSeconds;
						Wander();
					}
				}

				//If the AI is in a herd, continue to follow their Alpha
				if (isAlpha == 0 && inHerd)
				{
					huntMode = false;
					startHuntTimer = false;
					isCoolingDown = true;
					huntTimer = chaseSeconds;
					FollowAlpha();
				}
			}
		}
		
		//This is a cool down system that happens after our animal has reached its max chase seconds and has unsuccessfully caught its prey
		if (isCoolingDown)
		{
			coolDownTimer += Time.deltaTime;
			navMeshAgent.speed = runSpeed;

			if (returnBackToStartingPointProtection && returnsToStart)
			{
				currentHealth = startingHealth;
			}
			
			if (useAnimations)
			{
				if (!useMecanim)
				{
					anim.CrossFade(runAnimation.name);
				}

				if (useMecanim)
				{
					MecanimRun();
				}
			}
			
			if (Vector3.Distance(navMeshAgent.destination, navMeshAgent.transform.position) < stoppingDistance)
			{
				isCoolingDown = false;
			}
		}
		
		//This is a cool down system that happens after our animal has reached its max chase seconds and unsuccessfully escaped its predator
		if (isExhausted && !isDead)
		{
			coolDownTimer += Time.deltaTime;
			
			if (inHerd && isAlpha == 0)
			{
				Vector3 NewPosition = new Vector3 (animalToFollow.position.x, animalToFollow.position.y, animalToFollow.position.z) + Random.insideUnitSphere * 40;
				NewDestination(NewPosition);
			}
			
			if (useAnimations)
			{
				if (!useMecanim) 
				{
					anim.CrossFade (walkAnimation.name);
				}

				if (useMecanim)
				{
					MecanimWalk();
				}
			}
			
			navMeshAgent.speed = walkSpeed;
			
			if (coolDownTimer >= coolDownSeconds)
			{
				isExhausted = false;
				coolDownTimer = 0;
			}
		}

		//If our current target is lost, refresh our trigger collider so our PickNewTarget function can update
		if (currentTargetSystem != null && isPlayer == false)
		{
			if (AIType == 1 && huntMode && !currentTargetSystem.enabled)
			{
				currentAnimal = null;
				triggerCollider.enabled = false;
				triggerCollider.enabled = true;
			}
		}

		//If huntMode is enabled, but our target is lost or killed, look for new targets or wander
		if (huntMode)			
		{
			if (currentTargetSystem == null && currentAnimal == null || currentAnimal == null)
			{
				currentAnimal = null;
				currentTargetSystem = null;
				huntMode = false;
				potentialTargets.Clear();
				triggerCollider.enabled = false;
				triggerCollider.enabled = true;
				PickNewTarget();

				/*
				!inHerd || isAlpha == 1
				if (inHerd && animalToFollow != null && navMeshAgent.enabled)
				{
					if (!isDead)
					{ 
						FollowAlpha();
					}
				}
				*/
			}
		}


		//Calculate our animations and distance checks for hunt mode
		if (huntMode && currentAnimal != null && !isCoolingDown && !isDead)
		{
			HuntModeCalculations();	
		}
		
		//What happens when our animal's health reaches 0
		if (currentHealth <= 0)
		{
			Dead();
		}
		
		if (huntMode && navMeshAgent != null && navMeshAgent.enabled && currentAnimal != null)
		{
			HuntMode();			//Call the HuntMode function, if huntMode is enabled.
		}
		
		if (isFrozen)
		{
			Frozen();			//Call the Frozen function, if isFrozen is enabled and all conditions are met
		}
		
		if (animalToFollow == null)
		{
			inHerd = false;
		}
	}

	void Update () 
	{
		//Checks to see if our AI are still in view every few seconds.
		SystemOptimizerUpdater ();						//If they are no longer in view, according to Unity's LOD System, disable all components until back in view.
		//Using the NavMesh timer, check to see that our AI are off screen for at least 15 seconds before deactivating.
		//If the AI are out of view, set systemOn to false, which stops Emerald from calculating

		//If systemOn is true, run the Main Emerald System
		if (systemOn)
		{
			MainSystem ();
			CheckSystem();

			//1.3.5 removed alignAI
			if (!withinAttackDistance && terrain != null)
			{
				if (alignAI){
				AlignNavMesh();
				}
				else if (!alignAI){
					if (useAnimations)
					{
						if (!useMecanim)
						{
							if (huntMode && !anim.IsPlaying(currentAttackAnimationClip.name))
							{
								navMeshAgent.speed = runSpeed;
							}
						}
						
						if (useMecanim)
						{
							if (huntMode && !AnimatorComponent.GetBool(Attack1Parameter))
							{
								navMeshAgent.speed = runSpeed;
							}
						}
					}
				}
			}

			if (calculateFlee && fleeTarget != null && isAlpha == 1 || calculateFlee && fleeTarget != null && !inHerd)
			{
				if (!isDead && navMeshAgent.enabled)
				{
					Flee();
				}
			}
				
			if (inHerd && isAlpha == 0 && animalToFollow != null && navMeshAgent != null && isFleeing && aggression < 2 || inHerd && isAlpha == 0 && animalToFollow != null && navMeshAgent != null && huntMode && aggression >= 3)
			{
				if (!isDead && navMeshAgent.enabled)
				{
					FollowAlpha();
				}
			}
			
			if (herdList.Count == maxPackSize)
			{
				herdFull = true;
			}
		}
	}
	
	//If an alpha is assigned, follow it with Random.insideUnitSphere to keep the herd spaced apart
	public void FollowAlpha ()
	{
		followAlphaTimer += Time.deltaTime;

		if (!isTurning && followAlphaTimer >= 1)
		{
			//Follow our alpha's position within in a randomized sphere that updates every second
			if (!huntMode)
			{
				Vector3 direction = (animalToFollow.position - transform.position).normalized;
				destination = transform.position + direction * 30 + Random.insideUnitSphere * herdFollowRadius;
				NewDestination(destination);
			}

			followAlphaTimer = 0;
		}

		if (useAnimations && velocity > 0.1f && !isTurning)
		{
			navMeshAgent.speed = runSpeed;

			if (!useMecanim) 
			{
				anim.CrossFade (runAnimation.name);
			}

			if (useMecanim)
			{
				MecanimRun();
			}
		}
		
		if (!useAnimations)
		{
			navMeshAgent.speed = runSpeed;
		}

		if (useAnimations && !withinAttackDistance)
		{
			velocity = navMeshAgent.velocity.sqrMagnitude;

			if (!useMecanim)
			{
				if (velocity < 0.1f && anim.IsPlaying(runAnimation.name))
				{
					anim.CrossFade (idleAnimation.name);
				}
			}

			if (useMecanim)
			{
				if (velocity < 0.1f && AnimatorComponent.GetBool(RunParameter))
				{
					MecanimIdle();
				}
			}
		}
	}		
	
	//Calculates our animal's alignment to the terrain. This feature is only available if a terrain
	//is found on start using a RayCast. If no terrain is found, this feature is automatically disabled.
	public void AlignNavMesh ()
	{
		Vector3 normal = CalculateRotation();
		Vector3 direction = navMeshAgent.steeringTarget - transform.position;
		direction.y = 0.0f;

		navMeshAgent.updateRotation = false;

		if(direction.magnitude > 0.1f && normal.magnitude > 0.1f) 
		{
			Quaternion quaternionLook = Quaternion.LookRotation(direction, Vector3.up);
			Quaternion quaternionNormal = Quaternion.FromToRotation(Vector3.up, normal);
			originalLookRotation = quaternionNormal * quaternionLook;
		}

		//Calculate our AI's angle so we can use it below.
		angle = Quaternion.Angle(transform.rotation, originalLookRotation);

		//Stop movement if our angle is greater than 50 degrees. This is to ensure that our AI have time to rotate before running. It also stops AI from running in place.
		//If our AI's angle is greater than 50 degrees, play our turn animation, if enabled.
			if (angle <= 50)
			{
				if (!huntMode && aggression >= 3)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * walkRotateSpeed);
				}

				if (huntMode && aggression >= 3)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * runRotateSpeed);
				}

				if (!isFleeing && aggression <= 2)
				{
					navMeshAgent.speed = walkSpeed;
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * walkRotateSpeed);
				}

				if (isFleeing && aggression <= 2)
				{
					navMeshAgent.speed = runSpeed;
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * runRotateSpeed);
				}

				if (useAnimations)
				{
					if (!useMecanim)
					{
						if (huntMode && !anim.IsPlaying(currentAttackAnimationClip.name))
						{
							navMeshAgent.speed = runSpeed;
						}
					}

					if (useMecanim)
					{
						if (huntMode && !AnimatorComponent.GetBool(Attack1Parameter))
						{
							navMeshAgent.speed = runSpeed;
						}
					}
				}
				
				if (!huntMode && aggression >= 3 && !isCoolingDown)
				{
					navMeshAgent.speed = walkSpeed;
				}

				isTurning = false;
			}
			
			if (angle > 50)
			{
				if (!huntMode && aggression >= 3)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * walkRotateSpeed);
				}

				if (huntMode && aggression >= 3)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * runRotateSpeed);
				}

				if (!isFleeing && aggression <= 2)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * walkRotateSpeed);
				}

				if (isFleeing && aggression <= 2)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * runRotateSpeed);
				}

				navMeshAgent.speed = walkSpeed;

				if (useAnimations && !useMecanim && useTurnAnimation && !huntMode)
				{
					anim.CrossFade(turnAnimation.name);
					isTurning = true;
				}
			}
	}

	//Find the angle of the terrain
	Vector3 CalculateRotation () 
	{
		Vector3 terrainLocalPos = transform.position - terrain.transform.position;
		Vector2 normalizedPos = new Vector2(terrainLocalPos.x / terrain.terrainData.size.x, terrainLocalPos.z / terrain.terrainData.size.z);
		return terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
	}
	
	public void CheckSystem()
	{
		//For some reason, if our trigger to start our flee timer was missed, set it to true
		if (threatIsOutOfTigger && isFleeing && !startFleeTimer && fleeType == 1)
		{
			startFleeTimer = true;
		}
		
		if (alphaWaitForHerd && isFleeing && isAlpha == 1 && hasPack && herdList[0] != null)
		{
			float distance = Vector3.Distance (this.transform.position, herdList[0].transform.position);
			
			if (distance >= maxDistanceFromHerd)
			{
				navMeshAgent.speed = 0;
				waitingForHerd = true;
			}
			
			if (distance < maxDistanceFromHerd)
			{
				navMeshAgent.speed = runSpeed;
				waitingForHerd = false;
			}
		}
	}

	//Assign Alpha Status if our Alpha dies.
	public void AssignAlphaStatus ()
	{
		foreach (GameObject G in herdList) 
		{
			if (G != null)
			{
				G.GetComponent<Emerald_Animal_AI>().animalToFollow = gameObject.transform;
				G.GetComponent<Emerald_Animal_AI>().fleeTarget = fleeTarget;
				G.GetComponent<Emerald_Animal_AI>().calculateFlee = calculateFlee;
			}
		}
	}
	
	//Frozen handles the freeze system for when a prey stops for a random amount of
	//seconds to simulate a stunned or cautious action. Each time this is trigger, it is recalculated
	public void Frozen ()
	{
		freezeSecondsTimer += Time.deltaTime;
		
		if (!isDead && freezeSecondsTimer >= freezeSecondsTotal && navMeshAgent.enabled)
		{
			isFleeing = true;
			currentlyBeingPursued = true;
			Flee();
			freezeSecondsTimer = 0;
			isFrozen = false;
		}
	}

	//Allows our AI to return to its starting position, if enabled.
	public void ReturnBackToStartingPoint ()
	{
		NewDestination(startPosition);
		huntMode = false;
		currentlyBeingPursued = false;
		startHuntTimer = false;
		isCoolingDown = true;
		navMeshAgent.speed = runSpeed;
		huntTimer = chaseSeconds;

		//Remove our target from the AI if they return to their starting position. This can be commented out if desired.
		currentAnimal = null;
		currentTargetSystem = null;

		//Update trigger collider to make sure no enemies are within range
		triggerCollider.enabled = false;
		triggerCollider.enabled = true;

		//Mecanim
		if (useAnimations)
		{
			if (!useMecanim) 
			{
				anim.CrossFade (runAnimation.name);
			}

			if (useMecanim)
			{
				MecanimRun();
			}
		}
	}

	//This function handles all hunt related trigger for both alphas, non-alphas, and animals in herds.
	//This also makes it possible for AI to attack from being shot or hit with a ranged object. 
	//It is possible to get this working with RFPS and UFPS, or any other camera/caracter systems, 
	//by passing your Player object as a parameter. This Player object should be the object that
	//your AI would normally detect as the player (your Player tagged object)
	public void MakeAttack (GameObject targetToSend)
	{
		if (!huntMode && aggression >= 3)
		{
			if (isAlpha == 1 || isAlpha == 0)
			{
				currentAnimal = targetToSend;
				maxChaseDistance = 150;
				targetInRange = true;
				isGrazing = false;
				huntMode = true;
				startHuntTimer = true;

				if (currentAnimal.GetComponent<Emerald_Animal_AI>() == null)
				{
					isPlayer = true;
				}

				HuntMode();

				triggerCollider.enabled = false;
				triggerCollider.enabled = true;
			}

			if (inHerd && isAlpha == 0 && currentAnimal != null)
			{
				animalToFollow.GetComponent<Emerald_Animal_AI>().currentAnimal = currentAnimal;
				animalToFollow.GetComponent<Emerald_Animal_AI>().maxChaseDistance = 150;
				animalToFollow.GetComponent<Emerald_Animal_AI>().targetInRange = true;
				animalToFollow.GetComponent<Emerald_Animal_AI>().isGrazing = false;
				animalToFollow.GetComponent<Emerald_Animal_AI>().huntMode = true;
				animalToFollow.GetComponent<Emerald_Animal_AI>().startHuntTimer = true;

				if (currentAnimal.GetComponent<Emerald_Animal_AI>() == null)
				{
					animalToFollow.GetComponent<Emerald_Animal_AI>().isPlayer = true;
				}

				animalToFollow.GetComponent<Emerald_Animal_AI>().HuntMode();
				animalToFollow.GetComponent<Emerald_Animal_AI>().AssignTargetToGroupAttack();
			}
				
			if (isAlpha == 1)
			{
				foreach (GameObject G in herdList) 
				{
					if (G != null)
					{
						if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
						{
							G.GetComponent<Emerald_Animal_AI>().currentAnimal = currentAnimal;
							G.GetComponent<Emerald_Animal_AI>().maxChaseDistance = 150;
							G.GetComponent<Emerald_Animal_AI>().targetInRange = true;
							G.GetComponent<Emerald_Animal_AI>().isGrazing = false;
							G.GetComponent<Emerald_Animal_AI>().huntMode = true;
							G.GetComponent<Emerald_Animal_AI>().startHuntTimer = true;

							if (currentAnimal.GetComponent<Emerald_Animal_AI>() == null)
							{
								G.GetComponent<Emerald_Animal_AI>().isPlayer = true;
							}

							G.GetComponent<Emerald_Animal_AI>().HuntMode();
						}
					}
				}
			}
		}
	}

	//This function handles all flee related trigger for both alphas, non-alphas, and animals in herds.
	//This make it possible for AI to flee from being shot or hit with a ranged object. 
	//It is possible to get this working with RFPS and UFPS, or any other camera/caracter systems, 
	//by passing your Player object as a parameter. This Player object should be the object that
	//your AI would normally detect as the player (your Player tagged object)
	public void MakeFlee (GameObject targetToSend)
	{
		if (!isFleeing && aggression < 2)
		{
			if (isAlpha == 1 || isAlpha == 0 && fleeTarget == null)
				{
					fleeTarget = targetToSend;

					if (!isFleeing)
					{
						startRangedFlee = true;
						rangedImpactDistance = Vector3.Distance (this.transform.position, fleeTarget.transform.position);
						rangedFleeDistance = (int)rangedImpactDistance + rangedFleeAmount;
					}

					navMeshAgent.enabled = true;
					isGrazing = false;
					navMeshAgent.ResetPath();
					
					isFleeing = true;
					currentlyBeingPursued = true;
					calculateFlee = true;
					freezeSecondsTimer = 0;
					isFrozen = false;
					Flee();
				}

				if (inHerd && isAlpha == 0 && animalToFollow != null)
				{
				
					if (!animalToFollow.GetComponent<Emerald_Animal_AI>().isFleeing)
					{
						animalToFollow.GetComponent<Emerald_Animal_AI>().rangedFleeDistance = rangedFleeDistance;
						animalToFollow.GetComponent<Emerald_Animal_AI>().fleeTarget = targetToSend;
						animalToFollow.GetComponent<Emerald_Animal_AI>().startRangedFlee = true;
						startRangedFlee = false;
					}
					

					animalToFollow.GetComponent<Emerald_Animal_AI>().fleeTarget = fleeTarget;
					animalToFollow.GetComponent<Emerald_Animal_AI>().isFleeing = true;
					animalToFollow.GetComponent<Emerald_Animal_AI>().calculateFlee = true;
					animalToFollow.GetComponent<Emerald_Animal_AI>().distantFlee = true;

					//Added to notify the leader to trigger the herd to run and assign fleeTarget
					animalToFollow.GetComponent<Emerald_Animal_AI>().AssignTargetToGroupFlee();
				}

				if (isAlpha == 1)
				{
					foreach (GameObject G in herdList) 
					{
						if (G != null)
						{
							if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
							{
								G.GetComponent<Emerald_Animal_AI>().fleeTarget = fleeTarget;
								G.GetComponent<Emerald_Animal_AI>().isFleeing = true;
								G.GetComponent<Emerald_Animal_AI>().calculateFlee = true;
								G.GetComponent<Emerald_Animal_AI>().distantFlee = true;
							}
						}
					}
				}
		}
	}

	//Assign flee information to the herd
	public void AssignTargetToGroupFlee ()
	{
		if (isAlpha == 1)
		{
			foreach (GameObject G in herdList) 
			{
				if (G != null)
				{
					if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
					{
						G.GetComponent<Emerald_Animal_AI>().fleeTarget = fleeTarget;
						G.GetComponent<Emerald_Animal_AI>().isFleeing = true;
						G.GetComponent<Emerald_Animal_AI>().calculateFlee = true;
						G.GetComponent<Emerald_Animal_AI>().distantFlee = true;
					}
				}
			}
		}
	}

	//Assign attack information to the herd
	public void AssignTargetToGroupAttack ()
	{
		if (isAlpha == 1)
		{
			foreach (GameObject G in herdList) 
			{
				if (G != null)
				{
					if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
					{
						G.GetComponent<Emerald_Animal_AI>().currentAnimal = currentAnimal;
						G.GetComponent<Emerald_Animal_AI>().maxChaseDistance = 150;
						G.GetComponent<Emerald_Animal_AI>().targetInRange = true;
						G.GetComponent<Emerald_Animal_AI>().isGrazing = false;
						G.GetComponent<Emerald_Animal_AI>().huntMode = true;
						G.GetComponent<Emerald_Animal_AI>().startHuntTimer = true;

						if (currentAnimal.GetComponent<Emerald_Animal_AI>() == null)
						{
							G.GetComponent<Emerald_Animal_AI>().isPlayer = true;
						}


						G.GetComponent<Emerald_Animal_AI>().HuntMode();
					}
				}
			}
		}
	}
	
	//Call this function if you want to damage this animal from an external script.
	public void Damage (int damageReceived)
	{
		if (currentHealth >= 1)
		{
			//If a defensive NPC is hit by the player, attack the player
			if (aggression == 4)
			{
				currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);
				aggression = 3;

				triggerCollider.enabled = false;
				triggerCollider.enabled = true;
			}

			if (useAnimations && useMecanim)
			{
				if (aggression >= 3 && !huntMode && !AnimatorComponent.GetBool(WalkParameter) && !inHerd)
				{
					MecanimIdleCombat();
				}
				else if (aggression >= 3 && !huntMode && AnimatorComponent.GetBool(WalkParameter) && !inHerd)
				{
					MecanimRun();
				}
			}

			StartCoroutine(DelayReceivedDamage());
			tempDamage = damageReceived;

		}
	}

	//Delay our hits based on our damageDelaySeconds to allow for proper timing
	IEnumerator DelayReceivedDamage () 
	{
		yield return new WaitForSeconds(takeDamageDelaySeconds);

		//If using hit animations, play our hit animation when the AI receives damage.
		if (useHitAnimation && useAnimations)
		{
			if (!useMecanim)
			{
				if (aggression > 2 && !anim.IsPlaying(currentAttackAnimationClip.name) && !isDead && !anim.IsPlaying(runAnimation.name) && !anim.IsPlaying(walkAnimation.name))
				{
					if (!useRunAttackAnimations)
					{
						anim.CrossFade(hitAnimation.name);
					}

					if (useRunAttackAnimations && !anim.IsPlaying(runAttackAnimation.name))
					{
						anim.CrossFade(hitAnimation.name);
					}
				}

				if (aggression <= 2 && useAnimations && !isDead)
				{
					anim.CrossFade(hitAnimation.name);
				}
			}
				
			if (useMecanim)
			{
				if (aggression > 2 && !AnimatorComponent.GetBool(Attack1Parameter) && !AnimatorComponent.GetBool(Attack2Parameter) && !AnimatorComponent.GetBool(Attack3Parameter) && useAnimations && !isDead)
				{
					MecanimHit();
				}

				if (aggression <= 2 && useAnimations && !isDead)
				{
					MecanimHit();
				}
			}
		}

		//Subtract our damage that we received from our attacker.
		currentHealth -= tempDamage;

		//Randomize our sound list pitches
		foreach (AudioSource A in AudioSources)
		{
			A.pitch = Random.Range(minSoundPitch, maxSoundPitch);
		}

		//If using hit effect, spawn our hit effect when the AI receives damage.
		if (useBlood && !isPlayer)
		{
			if (bloodEffect != null)
			{
				if (!useCustomBloodTransform)
				{
					GameObject BloodSpawn = Instantiate((GameObject)bloodEffect, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z) + Random.insideUnitSphere * 1.5f, transform.rotation) as GameObject;
					BloodSpawn.transform.parent = transform;
				}

				//Unused for now
				else if (useCustomBloodTransform && customBloodTransform != null)
				{
					GameObject BloodSpawn = Instantiate((GameObject)bloodEffect, customBloodTransform.position + Random.insideUnitSphere * 2, transform.rotation) as GameObject;
					BloodSpawn.transform.parent = transform;
				}
			}
		}

		//If using injured sounds, play our hit sound when the AI receives damage.
		if (useInjuredSounds)
		{
			if (injuredSounds.Count > 0)
			{
				AudioSources[1].PlayOneShot(injuredSounds[Random.Range(0,injuredSounds.Count)]);
			}
		}

		//If using impact sounds, play our hit sound when the AI receives damage.
		if (useImpactSounds)
		{
			if (impactSounds.Count > 0)
			{
				AudioSources[2].PlayOneShot(impactSounds[Random.Range(0,impactSounds.Count)]);
			}
		}
	}
	
	//This handles how a predator/NPC hunts. It will only stop to attack if its velocity is around a stationary speed. 
	void HuntMode ()
	{
		updateSpeedTimer += Time.deltaTime;
		isGrazing = false;

		if (!useCustomAttackTransfrom)
		{
			DistanceFromTarget = Vector3.Distance (currentAnimal.transform.position, transform.position); 
			DistanceFromTarget = Mathf.Round(DistanceFromTarget * 10) / 10;
		}

		/*
		else if (useCustomAttackTransfrom)
		{
			DistanceFromTarget = Vector3.Distance (transform.position, customAttackTransform.position);
		}
		*/

		if (DistanceFromTarget > maxChaseDistance)
		{
			if (!targetInRange)
			{
				currentAnimal = null;
				huntMode = false;
				damageDealt = false;
				withinAttackDistance = false;
				attackWhileRunning = false;
				startHuntTimer = false;
				navMeshAgent.speed = walkSpeed;
				Wander();
			}

			if (targetInRange)
			{
				isPlayer = false;
				potentialTargets.Clear();
				PickNewTarget();
			}
		}

		if (currentTargetSystem != null)
		{
			if (currentTargetSystem.currentHealth <= 0 || currentAnimal == null)
			{
				potentialTargets.Clear();
				currentAnimal = null;
				currentTargetSystem = null;
				triggerCollider.enabled = false;
				triggerCollider.enabled = true;
				PickNewTarget();
			}
		}

		if (useAnimations)
		{
			if (!useMecanim)
			{
				if (currentAttackAnimationClip != null && anim.IsPlaying(currentAttackAnimationClip.name))
				{
					navMeshAgent.speed = 0;
				}

				//1.3.5
				else if (currentAttackAnimationClip != null && !withinAttackDistance && !anim.IsPlaying(currentAttackAnimationClip.name))
				{
					if (!useRunAttackAnimations)
					{
						navMeshAgent.speed = runSpeed;
						anim.CrossFade(runAnimation.name);
					}
					
					else if (useRunAttackAnimations)
					{
						if (!anim.IsPlaying(runAttackAnimation.name))
						{
							navMeshAgent.speed = runSpeed;
							anim.CrossFade(runAnimation.name);
						}
					}
				}
			}

			if (useMecanim)
			{
				if (AnimatorComponent.GetBool(Attack1Parameter) && !AnimatorComponent.GetBool(RunParameter))
				{
					navMeshAgent.speed = 0;
				}

				if (AnimatorComponent.GetBool(Attack2Parameter) && !AnimatorComponent.GetBool(RunParameter))
				{
					navMeshAgent.speed = 0;
				}

				if (AnimatorComponent.GetBool(Attack3Parameter) && !AnimatorComponent.GetBool(RunParameter))
				{
					navMeshAgent.speed = 0;
				}
					
				if (!withinAttackDistance && AnimatorComponent.GetBool(IdleCombatParameter))
				{
					if (!useRunAttackAnimations)
					{
						navMeshAgent.speed = runSpeed;
						MecanimRun();
					}

					else if (useRunAttackAnimations)
					{
						if (!AnimatorComponent.GetBool(RunAttackParameter))
						{
							navMeshAgent.speed = runSpeed;
							MecanimRun();
						}
					}
				}
					
				if (AnimatorComponent.GetCurrentAnimatorStateInfo(0).IsName(HitParameter) && !AnimatorComponent.GetBool(Attack1Parameter) && !AnimatorComponent.GetBool(RunParameter))
				{
					if (!useRunAttackAnimations)
					{
						MecanimIdleCombat();
					}
					else if (useRunAttackAnimations)
					{
						MecanimIdleCombat();
					}
				}
			}
		}
	
		//If our velocity drops near 0, play our idle animation so the animal isn't running in place
		if (useAnimations && !withinAttackDistance && !attackWhileRunning)
		{
			velocity = navMeshAgent.velocity.sqrMagnitude;

			if (!useMecanim)
			{
				if (velocity < 0.1f && anim.IsPlaying(runAnimation.name))
				{
					anim.CrossFade(idleBattleAnimation.name);
				}
			}

			if (useMecanim)
			{
				if (velocity < 0.1f && AnimatorComponent.GetBool(RunParameter))
				{
					MecanimIdleCombat();
				}
			}
		}
		
		if (updateSpeedTimer >= updateSpeed)
		{
			velocity = navMeshAgent.velocity.sqrMagnitude;
			updateSpeedTimer = 0;
		}
			
		if (useRunAttacks)
		{
			if (velocity > 0.1f && navMeshAgent.remainingDistance <= stoppingDistance + 1)  
			{
				attackWhileRunning = true;
				AttackWhileRunning();
			}
				
			if (velocity < 0.1f || navMeshAgent.remainingDistance > stoppingDistance + 1) 
			{
				attackWhileRunning = false; 
			}
		}

		if (currentAnimal != null)
		{
			if (DistanceFromTarget > stoppingDistance && useAnimations && !withinAttackDistance )
			{
				if (!useMecanim)
				{
					if (useRunAttackAnimations)
					{
						if (!anim.IsPlaying(runAttackAnimation.name) && !anim.IsPlaying(currentAttackAnimationClip.name) && velocity > 0.1f && !anim.IsPlaying(runAnimation.name))
						{
							anim.CrossFade(runAnimation.name);
							navMeshAgent.speed = runSpeed;
						}
					}

					if (!useRunAttackAnimations)
					{
						if (currentAttackAnimationClip != null && !anim.IsPlaying(currentAttackAnimationClip.name) && velocity > 0.1f)
						{
							anim.CrossFade(runAnimation.name); //Updated
							navMeshAgent.speed = runSpeed;
						}
					}
				}
					
				if (useMecanim) 
				{
					if (useRunAttackAnimations)
					{
						if (!AnimatorComponent.GetBool(RunAttackParameter) && !AnimatorComponent.GetBool(Attack1Parameter) && velocity > 0.1f)
						{
							navMeshAgent.speed = runSpeed; 
							MecanimRun();
						}
					}

					if (!useRunAttackAnimations)
					{
						if (!AnimatorComponent.GetBool(Attack1Parameter) && velocity > 0.1f)
						{
							navMeshAgent.speed = runSpeed; 
							MecanimRun();
						}
					}
				}
			}
		}
	}
	
	//If our AI is within attacking distance, use this function to update its rotations
	//so that it is always facing the player.
	private void RotateTowards (Transform currentPlayer) 
	{
		//Align our AI with its target, while using the terrain angle
		if (alignAI)
		{
			Vector3 normal = CalculateRotation();
			Vector3 direction2 = currentAnimal.transform.position - transform.position;
			direction2.y = 0.0f;

			navMeshAgent.updateRotation = false;

			if(direction2.magnitude > 0.1f && normal.magnitude > 0.1f) 
			{
				Quaternion quaternionLook = Quaternion.LookRotation(direction2, Vector3.up);
				Quaternion quaternionNormal = Quaternion.FromToRotation(Vector3.up, normal);
				originalLookRotation = quaternionNormal * quaternionLook;
			}

			//Calculate our AI's angle so we can use it below.
			angle = Quaternion.Angle(transform.rotation, originalLookRotation);
			transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime * 8f);
		}

		//Align our AI with its target, if not using the terrain angle
		if (!alignAI)
		{
			Vector3 direction = (currentAnimal.transform.position - transform.position).normalized;
			lookRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8);

			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
			angle = Quaternion.Angle(transform.rotation, lookRotation);

			if (angle <= 22)
			{
				if (!huntMode)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime*2f);
					navMeshAgent.speed = walkSpeed;
				}

				if (huntMode)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime*8f);
					navMeshAgent.speed = runSpeed;
				}
	
				isTurning = false;
			}

			if (angle > 22 && angle < 50)
			{
				if (!huntMode)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime*2f);
					navMeshAgent.speed = walkSpeed;
				}

				if (huntMode)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime*8f);
					navMeshAgent.speed = runSpeed;
				}

				isTurning = false;
			}

			if (angle > 50)
			{
				if (!huntMode)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime*2f);
					navMeshAgent.speed = walkSpeed;
				}

				if (huntMode)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, originalLookRotation, Time.deltaTime*8f);
					navMeshAgent.speed = runSpeed;
				}
			}

		}
		
		if (drawWaypoints)
		{
			currentWaypoint.transform.position = currentAnimal.transform.position;
		}
	}

	//Check all of the attack animations. If there are any that are null, or not in the Animation Component,
	//assign the Attack1 Animation to avoid any errors.
	void ApplyAttackAnimations()
	{
		if (totalAttackAnimations == 1)
		{
			currentAttackAnimationClip = attackAnimation1;
		}
		
		if (totalAttackAnimations == 2)
		{
			if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null){
				currentAttackAnimationClip = attackAnimation2;
			}
			else{
				attackAnimation2 = attackAnimation1;
			}
		}
		
		if (totalAttackAnimations == 3)
		{
			if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null){
				currentAttackAnimationClip = attackAnimation2;
			}
			else{
				attackAnimation2 = attackAnimation1;
			}

			if (attackAnimation3 != null && anim.GetClip(attackAnimation3.name) != null){
				currentAttackAnimationClip = attackAnimation3;
			}
			else{
				attackAnimation3 = attackAnimation1;
			}
		}
		
		if (totalAttackAnimations == 4)
		{
			if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null){
				currentAttackAnimationClip = attackAnimation2;
			}
			else{
				attackAnimation2 = attackAnimation1;
			}
			
			if (attackAnimation3 != null && anim.GetClip(attackAnimation3.name) != null){
				currentAttackAnimationClip = attackAnimation3;
			}
			else{
				attackAnimation3 = attackAnimation1;
			}
			if (attackAnimation4 != null && anim.GetClip(attackAnimation4.name) != null){
				currentAttackAnimationClip = attackAnimation4;
			}
			else{
				attackAnimation4 = attackAnimation1;
			}
		}
		
		if (totalAttackAnimations == 5)
		{
			if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null){
				currentAttackAnimationClip = attackAnimation2;
			}
			else{
				attackAnimation2 = attackAnimation1;
			}
			
			if (attackAnimation3 != null && anim.GetClip(attackAnimation3.name) != null){
				currentAttackAnimationClip = attackAnimation3;
			}
			else{
				attackAnimation3 = attackAnimation1;
			}
			if (attackAnimation4 != null && anim.GetClip(attackAnimation4.name) != null){
				currentAttackAnimationClip = attackAnimation4;
			}
			else{
				attackAnimation4 = attackAnimation1;
			}
			if (attackAnimation5 != null && anim.GetClip(attackAnimation5.name) != null){
				currentAttackAnimationClip = attackAnimation5;
			}
			else{
				attackAnimation5 = attackAnimation1;
			}
		}
		
		if (totalAttackAnimations == 6)
		{
			if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null){
				currentAttackAnimationClip = attackAnimation2;
			}
			else{
				attackAnimation2 = attackAnimation1;
			}
			
			if (attackAnimation3 != null && anim.GetClip(attackAnimation3.name) != null){
				currentAttackAnimationClip = attackAnimation3;
			}
			else{
				attackAnimation3 = attackAnimation1;
			}
			if (attackAnimation4 != null && anim.GetClip(attackAnimation4.name) != null){
				currentAttackAnimationClip = attackAnimation4;
			}
			else{
				attackAnimation4 = attackAnimation1;
			}
			if (attackAnimation5 != null && anim.GetClip(attackAnimation5.name) != null){
				currentAttackAnimationClip = attackAnimation5;
			}
			else{
				attackAnimation5 = attackAnimation1;
			}
			if (attackAnimation6 != null && anim.GetClip(attackAnimation6.name) != null){
				currentAttackAnimationClip = attackAnimation6;
			}
			else{
				attackAnimation6 = attackAnimation1;
			}
		}
	}

	//Attack while stationary handles all our attack related mechanics
	//such as animations, damage, and sounds.
	void AttackWhileStationary ()
	{
		if (useAnimations)
		{
			if (currentAnimal != null && withinAttackDistance)
			{
				RotateTowards(currentAnimal.transform);
			}
		}
		
		if (!useAnimations)
		{
			if (currentAnimal != null && withinAttackDistance)
			{
				RotateTowards(currentAnimal.transform);
			}
		}
		
		if (drawWaypoints)
		{
			currentWaypoint.transform.position = currentAnimal.transform.position;
		}
		
		attackTimer += Time.deltaTime;
		

		if (attackTimer >= attackTime)
		{
			if (useAnimations)
			{
				if (!attackWhileRunning)
				{
					if (!useMecanim)
					{
						//Play our attack animations based on the amount of attack animations being used.
						if (currentAttackAnimation == 1)
						{
							currentAttackAnimationClip = attackAnimation1;
							anim.CrossFade(attackAnimation1.name);
						}
						
						if (currentAttackAnimation == 2)
						{
							currentAttackAnimationClip = attackAnimation2;
							if (attackAnimation2 != null && anim.GetClip(attackAnimation2.name) != null){
								currentAttackAnimationClip = attackAnimation2;
								anim.CrossFade(attackAnimation2.name);
							}
							else{
								currentAttackAnimationClip = attackAnimation1;
								anim.CrossFade(attackAnimation1.name);
							}
						}
						
						if (currentAttackAnimation == 3)
						{
							currentAttackAnimationClip = attackAnimation3;
							if (attackAnimation3 != null && anim.GetClip(attackAnimation3.name) != null){
								currentAttackAnimationClip = attackAnimation3;
								anim.CrossFade(attackAnimation3.name);
							}
							else{
								currentAttackAnimationClip = attackAnimation1;
								anim.CrossFade(attackAnimation1.name);
							}
						}
						
						if (currentAttackAnimation == 4)
						{
							currentAttackAnimationClip = attackAnimation4;
							if (attackAnimation4 != null && anim.GetClip(attackAnimation4.name) != null){
								currentAttackAnimationClip = attackAnimation4;
								anim.CrossFade(attackAnimation4.name);
							}
							else{
								currentAttackAnimationClip = attackAnimation1;
								anim.CrossFade(attackAnimation1.name);
							}
						}
						
						if (currentAttackAnimation == 5)
						{
							currentAttackAnimationClip = attackAnimation5;
							if (attackAnimation5 != null && anim.GetClip(attackAnimation5.name) != null){
								currentAttackAnimationClip = attackAnimation5;
								anim.CrossFade(attackAnimation5.name);
							}
							else{
								currentAttackAnimationClip = attackAnimation1;
								anim.CrossFade(attackAnimation1.name);
							}
						}
						
						if (currentAttackAnimation == 6)
						{
							currentAttackAnimationClip = attackAnimation6;
							if (attackAnimation6 != null && anim.GetClip(attackAnimation6.name) != null){
								currentAttackAnimationClip = attackAnimation6;
								anim.CrossFade(attackAnimation6.name);
							}
							else{
								currentAttackAnimationClip = attackAnimation1;
								anim.CrossFade(attackAnimation1.name);
							}
						}
					}

					//Call our attack functions based on the amount of attack animations being used.
					if (useMecanim)
					{
						if (currentAttackAnimation == 1)
						{
							MecanimAttack1();
						}

						if (currentAttackAnimation == 2)
						{
							MecanimAttack2();
						}

						if (currentAttackAnimation == 3)
						{
							MecanimAttack3();
						}

						AnimatorComponent.SetBool (RunParameter, false);
						AnimatorComponent.SetBool (WalkParameter, false);
						AnimatorComponent.SetBool (IdleParameter, false);
						AnimatorComponent.SetBool (IdleCombatParameter, false);
						AnimatorComponent.SetBool (HitParameter, false);
						AnimatorComponent.SetBool(RunAttackParameter, false);
					}
					
					if (!damageDealt && withinAttackDistance && !attackWhileRunning)
					{
							if (!isPlayer && currentAnimal != null)
							{
								if (useAttackSound)
								{
									if (attackSounds.Count > 0)
									{
										AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
									}
								}

								if (useWeaponSound)
								{
									if (weaponSound != null)
									{
										AudioSources[1].PlayOneShot(weaponSound);
									}
								}

								if (currentTargetSystem == null && currentAnimal != null)
								{
									currentTargetSystem = currentAnimal.gameObject.GetComponent<Emerald_Animal_AI>();
								}
								attackDamage = Random.Range(attackDamageMin, attackDamageMax);
								StartCoroutine(AttackDelay());
							}

							if (isPlayer)
							{
								if (currentAnimal == null)
								{
									triggerCollider.enabled = false;
									triggerCollider.enabled = true;
								}
								
								if (useAttackSound)
								{
									if (attackSounds.Count > 0)
									{
										AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
									}
								}
								
								if (useWeaponSound)
								{
									if (weaponSound != null)
									{
										AudioSources[1].PlayOneShot(weaponSound);
									}
								}
								
								attackDamage = Random.Range(attackDamageMin, attackDamageMax);
								StartCoroutine(AttackDelay());
							}
							
							damageDealt = true;
					}
				}
			}
			
			if (!useAnimations)
			{
				if (!damageDealt && withinAttackDistance && !attackWhileRunning)
				{
					if (!isPlayer && currentAnimal != null)
					{
						if (useAttackSound)
						{
							if (attackSounds.Count > 0)
							{
								AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
							}
						}

						if (useWeaponSound)
						{
							if (weaponSound != null)
							{
								AudioSources[1].PlayOneShot(weaponSound);
							}
						}

						if (currentTargetSystem == null && currentAnimal != null)
						{
							currentTargetSystem = currentAnimal.gameObject.GetComponent<Emerald_Animal_AI>();
						}

						attackDamage = Random.Range(attackDamageMin, attackDamageMax);
						StartCoroutine(AttackDelay());
					}

					if (isPlayer)
					{
						if (currentAnimal == null)
						{
							triggerCollider.enabled = false;
							triggerCollider.enabled = true;
						}

						if (useAttackSound)
						{
							if (attackSounds.Count > 0)
							{
								AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
							}
						}

						if (useWeaponSound)
						{
							if (weaponSound != null)
							{
								AudioSources[1].PlayOneShot(weaponSound);
							}
						}

						attackDamage = Random.Range(attackDamageMin, attackDamageMax);
						StartCoroutine(AttackDelay());
					}

					damageDealt = true;
				}
			}
		}
			
		if (!useMecanim)
		{
			if (useAnimations && attackTimer >= attackTime)
			{
				anim.CrossFadeQueued(idleBattleAnimation.name);

				if (anim.IsPlaying(idleBattleAnimation.name) && attackTimer >= currentAttackAnimationClip.length/attackAnimationSpeed)
				{
					attackTime = Random.Range(attackTimeMin, attackTimeMax);

					//Automatically adjust our attack speeds in case they go below the adjusted attack animaiyion length
					if (attackTimeMin < currentAttackAnimationClip.length/attackAnimationSpeed)
					{
						attackTimeMin = currentAttackAnimationClip.length/attackAnimationSpeed;
					}

					if (attackTimeMax < currentAttackAnimationClip.length/attackAnimationSpeed)
					{
						attackTimeMax = currentAttackAnimationClip.length/attackAnimationSpeed;
					}

					currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);

					damageDealt = false;
					attackTimer = 0;
				}
			}
		}

		if (useMecanim)
		{
			if (useAnimations && attackTimer >= attackTime + AnimatorComponent.GetCurrentAnimatorClipInfo(0)[0].clip.length / AnimatorComponent.GetCurrentAnimatorStateInfo(0).speed)
			{
				MecanimIdleCombat();

				attackTime = Random.Range(attackTimeMin, attackTimeMax);
				currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);

				foreach (AudioSource A in AudioSources)
				{
					A.pitch = Random.Range(minSoundPitch, maxSoundPitch);
				}

				damageDealt = false;
				attackTimer = 0;
			}
		}
		
		if (!useAnimations && attackTimer >= attackTime)
		{
			currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);
			withinAttackDistance = false;

			attackTime = Random.Range(attackTimeMin, attackTimeMax);
			damageDealt = false;
			attackTimer = 0;
		}
	}

	//The AttackDelay function delays the call to trigger damaging an AI or player.
	//This is done both automatically and manually to help animations reach their 
	//optimal position before sending a successful attack. The automatic delay is
	//calculated by getting the base animation length, dividing it by the customized attack speed, 
	//and lastly triggering a damage call at ~28% of the recalculated delay. 
	IEnumerator AttackDelay ()
	{
		if (useAnimations && AnimationType == 1)
		{
			if (autoCalculateDelaySeconds)
			{
				yield return new WaitForSeconds((currentAttackAnimationClip.length/attackAnimationSpeed)/4f);
			}
			else if (!autoCalculateDelaySeconds)
			{
				yield return new WaitForSeconds(attackDelaySeconds);
			}
		}

		else if (useAnimations && AnimationType == 2)
		{
			if (autoCalculateDelaySeconds)
			{
				yield return new WaitForSeconds(AnimatorComponent.GetCurrentAnimatorClipInfo(0).Length/3.5f);
			}
			else if (!autoCalculateDelaySeconds)
			{
				yield return new WaitForSeconds(attackDelaySeconds);
			}
		}

		else if (!useAnimations)
		{
			yield return new WaitForSeconds(0.1f);
		}

		//Make sure our target is still within range before sending a successfull attack
		if (withinAttackDistance && !attackWhileRunning || !withinAttackDistance && attackWhileRunning)
		{
			if (isPlayer)
			{
				//Applies damage to our player. This can be any desired function as long as long
				//the function has 1 parameter. If the function has more than 1 parameter, 
				//you will need to pass you will need to call the function directly.

				currentAnimal.SendMessage(SendMessageForPlayerDamage, attackDamage);
				//currentAnimal.GetComponent<CharacterHealth>().Damage();

				//If you would like your AI to damage the UFPS player damage script, uncomment the code below by removing the // and comment out the currentAnimal.GetComponent<PlayerHealth>() portions using //
				//currentAnimal.GetComponent<vp_FPPlayerDamageHandler>().Damage(attackDamage);
			}
			else if (!isPlayer && currentTargetSystem != null)
			{
				currentAnimal.SendMessage("Damage", attackDamage);
			}
		}
	}
	
	void AttackWhileRunning ()
	{
		attackTimer += Time.deltaTime;
		
		if (drawWaypoints)
		{
			currentWaypoint.transform.position = currentAnimal.transform.position;
		}
			
		if (attackTimer >= attackTime) 
		{		
			if (!damageDealt && !withinAttackDistance && attackWhileRunning)
			{				
				if (!isPlayer && currentAnimal != null)
				{
					if (useAttackSound)
					{
						if (attackSounds.Count > 0)
						{
							AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
						}
					}

					if (useWeaponSound)
					{
						if (weaponSound != null)
						{
							AudioSources[1].PlayOneShot(weaponSound);
						}
					}

					if (currentTargetSystem == null && currentAnimal != null)
					{
						currentTargetSystem = currentAnimal.gameObject.GetComponent<Emerald_Animal_AI>();
					}

					attackDamage = Random.Range(attackDamageMin, attackDamageMax);
					StartCoroutine(AttackDelay());
				}

				if (isPlayer)
				{
					if (currentAnimal == null)
					{
						triggerCollider.enabled = false;
						triggerCollider.enabled = true;
					}

					if (useAttackSound)
					{
						if (attackSounds.Count > 0)
						{
							AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
						}
					}

					if (useWeaponSound)
					{
						if (weaponSound != null)
						{
							AudioSources[1].PlayOneShot(weaponSound);
						}
					}

					attackDamage = Random.Range(attackDamageMin, attackDamageMax);
					StartCoroutine(AttackDelay());
				}

				damageDealt = true;
			}

			if (useAnimations)
			{	
				//Legacy
				if (!useMecanim && useRunAttackAnimations)
				{
					//Play the run attack animation and wait for it to finish.
					//Once it's finished, blend back to the run animation
					anim.CrossFade(runAttackAnimation.name);

					if (attackTimer >= attackTime)
					{
						anim.CrossFadeQueued(runAnimation.name);

						if (anim.IsPlaying(runAnimation.name) && attackTimer >= runAttackAnimation.length)
						{
							attackTime = Random.Range(attackTimeMin, attackTimeMax);

							//Automatically adjust our attack speeds in case they go below the adjusted attack animaiyion length
							//This prevents animations skipping, repeating, or overlapping.
							if (attackTimeMin < runAttackAnimation.length)
							{
								attackTime = runAttackAnimation.length+1;
							}

							if (attackTimeMax < runAttackAnimation.length)
							{
								attackTime = runAttackAnimation.length+1;
							}

							currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);

							damageDealt = false;
							attackTimer = 0;
						}
					}
				}

				if (useMecanim && useRunAttackAnimations)
				{
					//Play the run attack animation and wait for it to finish.
					//Once it's finished, blend back to the run animation
					MecanimRunAttack();

					if (AnimatorComponent.GetCurrentAnimatorStateInfo(0).IsName(RunAttackParameter))
					{
						MecanimRun();

						if (!AnimatorComponent.GetBool(RunAttackParameter) && attackTimer >= attackTime + 0.5f)
						{
							attackTime = Random.Range(attackTimeMin, attackTimeMax);
							currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);

							damageDealt = false;
							attackTimer = 0;
						}
					}
				}

				if (!useMecanim && !useRunAttackAnimations)
				{
					if (attackTimer >= attackTime)
					{
						attackTime = Random.Range(attackTimeMin, attackTimeMax);
						currentAttackAnimation = Random.Range(1, totalAttackAnimations+1);

						damageDealt = false;
						attackTimer = 0;
					}
				}
			}

			if (!useAnimations)
			{
				if (!damageDealt && !withinAttackDistance && attackWhileRunning)
				{				
					if (!isPlayer && currentAnimal != null)
					{
						if (useAttackSound)
						{
							if (attackSounds.Count > 0)
							{
								AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
							}
						}

						if (useWeaponSound)
						{
							if (weaponSound != null)
							{
								AudioSources[1].PlayOneShot(weaponSound);
							}
						}

						if (currentTargetSystem == null && currentAnimal != null)
						{
							currentTargetSystem = currentAnimal.gameObject.GetComponent<Emerald_Animal_AI>();
						}

						attackDamage = Random.Range(attackDamageMin, attackDamageMax);
						StartCoroutine(AttackDelay());
					}

					if (isPlayer)
					{
						if (currentAnimal == null)
						{
							triggerCollider.enabled = false;
							triggerCollider.enabled = true;
						}

						if (useAttackSound)
						{
							if (attackSounds.Count > 0)
							{
								AudioSources[2].PlayOneShot(attackSounds[Random.Range(0,attackSounds.Count)]);
							}
						}

						if (useWeaponSound)
						{
							if (weaponSound != null)
							{
								AudioSources[1].PlayOneShot(weaponSound);
							}
						}

						attackDamage = Random.Range(attackDamageMin, attackDamageMax);
						StartCoroutine(AttackDelay());
					}

					damageDealt = true;
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other) 
	{
		//If the triggered object is our a follow tag, and the animal is a farm animal, follow.
		if (EmeraldTags.Contains(other.gameObject.tag) && aggression == 2 && !isExhausted && AIType == 0 && UseBreeding == 1)
		{
			followTransform = other.gameObject.transform;
			isFollowing = true;
		}
		
		//Generates our herd system
		//If the other gameobject is the same animal and they are an alpha, follow heard
		if (!isFleeing && other.gameObject.GetComponent<Emerald_Animal_AI>() != null && !inHerd && isAlpha == 0 && other.gameObject.GetComponent<Emerald_Animal_AI>().isAlpha == 1 && other.gameObject.GetComponent<Emerald_Animal_AI>().animalNameType == animalNameType && other.gameObject.GetComponent<Emerald_Animal_AI>().herdFull == false)
		{
			if (other.gameObject.GetComponent<Emerald_Animal_AI>().herdList.Count <= other.gameObject.GetComponent<Emerald_Animal_AI>().maxPackSize)
			{
				animalToFollow = other.gameObject.transform;
				inHerd = true;
			}
			
			if (other.gameObject.GetComponent<Emerald_Animal_AI>().packCreated == false)
			{
				//Add (Alpha) to the alpha's name
				other.gameObject.name = other.gameObject.name + " (Alpha)";
				other.gameObject.GetComponent<Emerald_Animal_AI>().packCreated = true;
			}
		}
		
		//Assign List of memebers of herd for alpha
		if (!isFleeing && other.gameObject.GetComponent<Emerald_Animal_AI>() != null && other.gameObject.GetComponent<Emerald_Animal_AI>().animalToFollow == this.gameObject.transform && other.gameObject.GetComponent<Emerald_Animal_AI>().markInPack == false && isAlpha == 1 && other.gameObject.GetComponent<Emerald_Animal_AI>().isAlpha == 0)
		{
			//Limits the number of animals that can be in a pack to what's set with the maxPackSize
			if (herdList.Count <= maxPackSize && !herdFull)
			{
				other.gameObject.GetComponent<Emerald_Animal_AI>().markInPack = true;
				hasPack = true;
				herdList.Add(other.gameObject);

				if (huntMode && targetInRange)
				{
					other.GetComponent<Emerald_Animal_AI>().potentialTargets = potentialTargets;
					other.GetComponent<Emerald_Animal_AI>().PickNewTarget();
				}

				triggerCollider.enabled = false;
				triggerCollider.enabled = true;
			}
		}

		if (isReadyForBreeding && !breedCoolDown && animalNameType != null && animalNameType != "")
		{
			if (other.gameObject.tag == this.gameObject.tag)
			{
				if (isReadyForBreeding && !mateFound && other.gameObject.GetComponent<Emerald_Animal_AI>().animalNameType == animalNameType && other.gameObject.GetComponent<Emerald_Animal_AI>().isReadyForBreeding)
				{
					if (!other.gameObject.GetComponent<Emerald_Animal_AI>().mateFound)
					{
						mateATransform = other.gameObject.transform;
						mateFound = true;
						isBabyGiver = true;
						other.gameObject.GetComponent<Emerald_Animal_AI>().mateFound = true;
						other.gameObject.GetComponent<Emerald_Animal_AI>().mateBTransform = this.gameObject.transform;
					}
				}
			}
		}
			
		//Only get Emerald Unity tagged objects then handle individual logic 
		if (other.gameObject.tag == EmeraldObjectsTag || EmeraldTags.Contains(other.gameObject.tag))
		{
			if (currentAnimal == null && aggression == 3 && !huntMode && !isCoolingDown && other.gameObject.tag != "Untagged")
			{
				if (EmeraldTags.Contains(other.gameObject.tag))
				{
					targetInRange = true;
					PickNewTarget ();
				}
				else if (EmeraldTags.Contains(other.gameObject.GetComponent<Emerald_Animal_AI>().AITag))
				{
					targetInRange = true;
					PickNewTarget ();
				}
			}

			else if (fleeTarget == null && aggression < 2 && !isExhausted && !isDead)
			{
				if (other.gameObject.tag == EmeraldObjectsTag && EmeraldTags.Contains(other.gameObject.GetComponent<Emerald_Animal_AI>().AITag))
				{
					navMeshAgent.enabled = true;
					navMeshAgent.ResetPath();
					MakeFlee(other.gameObject);
				}
				else if (EmeraldTags.Contains(other.gameObject.tag))
				{
					navMeshAgent.enabled = true;
					navMeshAgent.ResetPath();
					MakeFlee(other.gameObject);
				}
			}
		}
	}

	void OnTriggerExit(Collider other) 
	{
		if (aggression <= 1 && isFleeing)
		{
			//Out of danger. If this AI is an alpha, tell the herd
			if (other.gameObject == fleeTarget)
			{
				startFleeTimer = true;
				threatIsOutOfTigger = true;

				if (isAlpha == 1)
				{
					foreach (GameObject G in herdList) 
					{
						if (G != null)
						{
							if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
							{
								/*
								G.GetComponent<Emerald_Animal_AI>().fleeTarget = null;
								G.GetComponent<Emerald_Animal_AI>().isFleeing = false;
								G.GetComponent<Emerald_Animal_AI>().calculateFlee = false;
								G.GetComponent<Emerald_Animal_AI>().distantFlee = false;
								*/

								G.GetComponent<Emerald_Animal_AI>().startFleeTimer = true;
								G.GetComponent<Emerald_Animal_AI>().threatIsOutOfTigger = true;
							}
						}
					}
				}
			}
		}
	

		if (aggression > 2 && huntMode)
		{
			//Out of attack range. If this AI is an alpha, tell the herd
			if (other.gameObject == currentAnimal)
			{
				targetInRange = false;

				if (isAlpha == 1)
				{
					foreach (GameObject G in herdList) 
					{
						if (G != null)
						{
							if (!G.GetComponent<Emerald_Animal_AI>().isExhausted)
							{
								G.GetComponent<Emerald_Animal_AI>().currentAnimal = null;
								G.GetComponent<Emerald_Animal_AI>().targetInRange = false;
								G.GetComponent<Emerald_Animal_AI>().isGrazing = true;
								G.GetComponent<Emerald_Animal_AI>().huntMode = false;
								G.GetComponent<Emerald_Animal_AI>().startHuntTimer = false;
							}
						}
					}
				}
			}
		}

		//If the triggered object is our a follow tag, and the animal is a farm animal, follow.
		if (EmeraldTags.Contains(other.gameObject.tag) && aggression == 2 && AIType == 0)
		{
			isFollowing = false;
			isGrazing = true;
			Wander();
		}
	}

	//Find colliders within range using a Physics.OverlapSphere. Mask the Physics.OverlapSphere to only 2 layers. 
	//One for Emerald AI objects and one for the Player. This will allow the Physics.OverlapSphere to only get relevent colliders.
	//Once found, use Emerald's custom tag system to find matches for potential targets. Once found, apply them to a list for potential targets.
	//Finally, search through each target in the list and set the nearest one as our current target.
	public void PickNewTarget ()
	{
		if (targetInRange)
		{
			if (potentialTargets.Count == 0)
			{	
				if (!playerUsesSeparateLayer)
				{
					Collider[] C = Physics.OverlapSphere(transform.position, huntRadius * transform.localScale.x, LayerMask.GetMask(EmeraldLayerMaskString));
					CollidersInArea = C;
				}

				if (playerUsesSeparateLayer)
				{
					Collider[] C = Physics.OverlapSphere(transform.position, huntRadius * transform.localScale.x, LayerMask.GetMask(EmeraldLayerMaskString, PlayerLayerMaskString)); 
					CollidersInArea = C;
				}

				foreach (Collider C in CollidersInArea)
				{
					if (C.gameObject != this.gameObject && !potentialTargets.Contains(C.gameObject))
					{
						if (C.gameObject.GetComponent<Emerald_Animal_AI>() != null)
						{
							if (EmeraldTags.Contains(C.gameObject.GetComponent<Emerald_Animal_AI>().AITag))
							{
								potentialTargets.Add(C.gameObject);
							}
						}
							
						if (EmeraldTags.Contains(C.gameObject.tag))
						{
							potentialTargets.Add(C.gameObject);
						}
					}
				}
			}

			//If no targets have been found, have AI wander.
			if (potentialTargets.Count == 0){
				Wander ();
			}

			Vector3 distanceBetween; 
			float previousDistance = Mathf.Infinity; 
			float currentDistance;  

			foreach (GameObject target in potentialTargets.ToArray())
			{
				if (target != null){ //Added 1.3.5
					distanceBetween = target.transform.position - transform.position;
					currentDistance = distanceBetween.sqrMagnitude;

					if (currentDistance < previousDistance)
					{
						currentAnimal = target.gameObject;

						if (currentAnimal.GetComponent<Emerald_Animal_AI>() != null)
						{
							currentTargetSystem = currentAnimal.gameObject.GetComponent<Emerald_Animal_AI>();
							isPlayer = false;
						}
						else
						{
							isPlayer = true;
						}
						
						navMeshAgent.speed = runSpeed;

						previousDistance = currentDistance;
						MakeAttack(currentAnimal);
					}
				}
			}

		}
	}

	//If enabled, this function handles our dust effects that happen while running.
	void DustEffects ()
	{
		//If our animal is playing its run animation, and useDustEffect is enabled, set the particles to 10
		if (useAnimations)
		{
			if (!useMecanim)
			{
				if (anim.IsPlaying(runAnimation.name) && useDustEffect)
				{
					#if UNITY_5_5 || UNITY_5_6
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rateOverTime = new ParticleSystem.MinMaxCurve(10);
					#elif UNITY_5_3 || UNITY_5_4
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rate = new ParticleSystem.MinMaxCurve(10);
					#elif UNITY_5_2
					clone.emissionRate = 10;
					#endif
				}
			}

			if (useMecanim)
			{
				if (AnimatorComponent.GetBool(RunParameter) && useDustEffect)
				{
					#if UNITY_5_5 || UNITY_5_6
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rateOverTime = new ParticleSystem.MinMaxCurve(10);
					#elif UNITY_5_3 || UNITY_5_4
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rate = new ParticleSystem.MinMaxCurve(10);
					#elif UNITY_5_2
					clone.emissionRate = 10;
					#endif
				}
			}
		}

		//If our animal is not playing its run animation, and useDustEffect is enabled, set the particles to 0
		if (useAnimations)
		{
			if (!useMecanim) 
			{
				if (!anim.IsPlaying (runAnimation.name) && useDustEffect || useDustEffect && waitingForHerd || velocity < 0.1f && useDustEffect) {
					#if UNITY_5_5 || UNITY_5_6
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
					#elif UNITY_5_3 || UNITY_5_4
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rate = new ParticleSystem.MinMaxCurve(0f);
					#elif UNITY_5_2
					clone.emissionRate = 0;
					#endif
				}
			}

			if (useMecanim) 
			{
				if (!AnimatorComponent.GetBool(RunParameter) && useDustEffect || useDustEffect && waitingForHerd || velocity < 0.1f && useDustEffect) {
					#if UNITY_5_5 || UNITY_5_6
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
					#elif UNITY_5_3 || UNITY_5_4
					ParticleSystem.EmissionModule emission = clone.emission;
					emission.rate = new ParticleSystem.MinMaxCurve(0f);
					#elif UNITY_5_2
					clone.emissionRate = 0;
					#endif
				}
			}
		}
	}
		
	//Handles most of our ranged based Hunting/Attack mechanics
	void HuntModeCalculations ()
	{
		//1.3.5
		//destination = currentAnimal.transform.position;
		//navMeshAgent.destination = destination; 

		if (useAnimations && DistanceFromTarget > stoppingDistance && velocity > 0.1f)
		{
			if (!useMecanim) 
			{
				if (useRunAttackAnimations)
				{
					if (!anim.IsPlaying(runAttackAnimation.name) && currentAttackAnimationClip != null && !anim.IsPlaying(currentAttackAnimationClip.name) && !anim.IsPlaying(runAnimation.name))
					{
						anim.CrossFade(runAnimation.name);
					}
				}

				else if (!useRunAttackAnimations)
				{
					if (currentAttackAnimationClip != null && !anim.IsPlaying(currentAttackAnimationClip.name) && !anim.IsPlaying(runAnimation.name))
					{
						anim.CrossFade(runAnimation.name);
					}
				}
			}

			if (useMecanim)
			{
				MecanimRun();
			}
		}
			
		if (useHitAnimation && useAnimations)
		{
			if (!useMecanim)
			{
				if (currentAttackAnimationClip != null && !anim.IsPlaying(hitAnimation.name) && !anim.IsPlaying(currentAttackAnimationClip.name) && DistanceFromTarget <= stoppingDistance && velocity < 0.1f)
				{
					if (!attackWhileRunning)
					{
						anim.CrossFade(idleBattleAnimation.name);
					}
				}
			}

			if (useMecanim)
			{
				if (!AnimatorComponent.GetBool(HitParameter) && !AnimatorComponent.GetBool(Attack1Parameter) && DistanceFromTarget <= stoppingDistance && velocity < 0.1f)
				{
					if (!attackWhileRunning)
					{
						if (!useRunAttackAnimations)
						{
							MecanimIdleCombat();
						}
						else if (useRunAttackAnimations)
						{
							MecanimIdleCombat();
						}
					}
				}
			}
		}

		if (!useHitAnimation && useAnimations)
		{
			if (!useMecanim)
			{
				if (currentAttackAnimationClip != null && !anim.IsPlaying(currentAttackAnimationClip.name) && DistanceFromTarget <= stoppingDistance && velocity < 0.1f)
				{
					if (!attackWhileRunning)
					{
						anim.CrossFade(idleBattleAnimation.name);
					}
				}
			}

			if (useMecanim)
			{
				if (!AnimatorComponent.GetBool(Attack1Parameter) && DistanceFromTarget <= stoppingDistance && velocity < 0.1f)
				{
					if (!attackWhileRunning)
					{
						if (!useRunAttackAnimations)
						{
							MecanimIdleCombat();
						}
						else if (useRunAttackAnimations)
						{
							MecanimIdleCombat();
						}
					}
				}
			}
		}
			

		if (DistanceFromTarget > stoppingDistance) 
		{
			withinAttackDistance = false;

			if (!useCustomAttackTransfrom)
			{
				destination = currentAnimal.transform.position;
			}

			//Unused for now
			else if (useCustomAttackTransfrom)
			{
				destination = customAttackTransform.position;

			}
				
			navMeshAgent.destination = destination; 
		}
			
		if (DistanceFromTarget <= stoppingDistance && !attackWhileRunning) //Mathf.Round(stoppingDistance * 10) / 10
		{
			withinAttackDistance = true;
		}

		//Debug.Log(navMeshAgent.remainingDistance);
			
		//Attack while stationary, if within range.
		if (useAnimations)
		{
			if (!useMecanim)
			{
				if (withinAttackDistance && !attackWhileRunning && currentAnimal != null) 
				{
					AttackWhileStationary();
				}
			}

			if (useMecanim)
			{
				if (withinAttackDistance && !attackWhileRunning && currentAnimal != null || AnimatorComponent.GetBool(Attack1Parameter) && currentAnimal != null) //Added && currentAnimal != null
				{
					AttackWhileStationary();
				}
			}
		}
		else if (!useAnimations)
		{
			if (withinAttackDistance && currentAnimal != null) 
			{
				AttackWhileStationary();
			}
		}
	}


	void Graze ()
	{
		grazeTimer += Time.deltaTime;

		if (useAnimations && useMecanim && isFollowing)
		{
			MecanimIdle();
		}

		if (useAnimations && !isFollowing)
		{
			if (grazeAnimationNumber == 1)
			{
				if (!useMecanim)
				{
					anim.CrossFade(graze1Animation.name);
				}

				if (useMecanim) 
				{
					MecanimGraze1();
				}
			}

			if (grazeAnimationNumber == 2)
			{
				if (!useMecanim)
				{
					anim.CrossFade(graze2Animation.name);
				}

				if (useMecanim) 
				{
					MecanimGraze2();
				}
			}

			if (grazeAnimationNumber == 3)
			{
				if (!useMecanim)
				{
					anim.CrossFade(graze3Animation.name);
				}

				if (useMecanim) 
				{
					MecanimGraze3();
				}
			}
		}
			
		if (grazeTimer >= grazeLength && !isFollowing)
		{
			grazeAnimationNumber = Random.Range(1,totalGrazeAnimations+1);

			if (inHerd && animalToFollow != null)
			{
				Vector3 NewPosition = new Vector3 (animalToFollow.position.x, animalToFollow.position.y, animalToFollow.position.z) + Random.insideUnitSphere * herdRadius;
				NewDestination(NewPosition);
			}

			if (!inHerd || isAlpha == 1)
			{
				if (!isDead)
				{
					Wander();
				}
			}

			isGrazing = false;
			grazeTimer = 0;

		}
	}

	//If the proper conditions are met, play our running sounds for our AI
	void Footsteps ()
	{
		if (isFleeing && !isGrazing && systemOn && !isExhausted && velocity > 0.1f && aggression < 2 || huntMode && !isGrazing && systemOn && !isCoolingDown && velocity > 0.1f && aggression >= 3) 
		{
			if (useAnimations && !withinAttackDistance && !attackWhileRunning)
			{
				if (!useMecanim)
				{
					if (anim.IsPlaying(runAnimation.name) && useRunSound)
					{
						runTimer += Time.deltaTime;

						if (runTimer >= footStepSeconds && systemOn)
						{
							AudioSources[1].PlayOneShot(runSound);
							runTimer = 0;
						}
					}
				}

				if (useMecanim)
				{
					if (AnimatorComponent.GetBool(RunParameter) && useRunSound)
					{
						runTimer += Time.deltaTime;

						if (runTimer >= footStepSeconds && systemOn)
						{
							AudioSources[1].PlayOneShot(runSound);
							runTimer = 0;
						}
					}
				}
			}
		}
			
		//If the proper conditions are met, play our walk sounds for NPCs.
		if (useAnimations)
		{
			if (!useMecanim)
			{
				if (!isFleeing && anim.IsPlaying(walkAnimation.name) && useWalkSound && !huntMode)
				{
					runTimer += Time.deltaTime;

					if (runTimer >= footStepSecondsWalk && systemOn)
					{
						AudioSources[1].PlayOneShot(walkSound);
						runTimer = 0;
					}
				}
			}

			if (useMecanim)
			{
				if (!isFleeing && AnimatorComponent.GetBool(WalkParameter) && useWalkSound && !huntMode)
				{
					runTimer += Time.deltaTime;

					if (runTimer >= footStepSecondsWalk && systemOn)
					{
						AudioSources[1].PlayOneShot(walkSound);
						runTimer = 0;
					}
				}
			}
		}
	}

	//Disable unneeded components and change our tag so the AI is no longer a target
	void Dead ()
	{
		navMeshAgent.enabled = false;
		this.gameObject.tag = "Untagged";
		triggerCollider.gameObject.tag = "Untagged";
		triggerCollider.enabled = false;

		boxCollider.isTrigger = true;
		boxCollider.enabled = false;

		withinAttackDistance = false;
		attackWhileRunning = false;

		huntMode = false;
		attackTimer = 0;
		isGrazing = false;
		isFleeing = false;
		Destroy(currentWaypoint);

		if (useDustEffect)
		{
			clone.gameObject.SetActive(false);
		}

		if (isAlpha == 1 && herdList.Count >= 1 && !isDead)
		{
			herdNumber = Random.Range(0, herdList.Count);		//If our alpha dies, and it's in a herd, assign alpha status to random memeber in herd and remove alpha from List.

			if (herdList[herdNumber].GetComponent<Emerald_Animal_AI>() != null)
			{
				herdList[herdNumber].GetComponent<Emerald_Animal_AI>().isAlpha = 1;
				herdList[herdNumber].GetComponent<Emerald_Animal_AI>().herdList = herdList;
				herdList[herdNumber].GetComponent<Emerald_Animal_AI>().fleeTarget = fleeTarget;
				herdList[herdNumber].GetComponent<Emerald_Animal_AI>().calculateFlee = calculateFlee; 
				herdList[herdNumber].GetComponent<Emerald_Animal_AI>().animalToFollow = null;
				herdList[herdNumber].GetComponent<Emerald_Animal_AI>().AssignAlphaStatus();

				herdList.Remove(herdList[herdNumber]);
			}

			isDead = true;
		}

		if (inHerd && isAlpha == 0)
		{
			animalToFollow.GetComponent<Emerald_Animal_AI>().herdList.Remove(this.gameObject);
		}

		if (!isPlayer && currentAnimal != null)
		{
			foreach (GameObject C in potentialTargets)
			{
				if (C != null && C.GetComponent<Emerald_Animal_AI>() != null) //1.3.5 added c != null
				{
					C.GetComponent<Emerald_Animal_AI>().potentialTargets.Remove(gameObject);
				}
			}

			currentTargetSystem.PickNewTarget();
			currentTargetSystem.currentAnimal = null;
			currentTargetSystem.currentTargetSystem = null;
		}

		if (useDeadReplacement)
		{
			isDead = true;
			if (deadObject != null){
				Instantiate(deadObject, transform.position, transform.rotation);	//If the AI is out of health, instantiate its deadObject replacement.
			}
			else if (deadObject == null){
				Debug.Log("<color=red>Error on the "+gameObject.name+" game object:</color>You have Use Dead Replacement enabled, but did not assign an object to the Dead Object slot in the Emerald Editor under Health Options. " +
					"To fix this, plaese assign one or disable Use Dead Replacement and use an animation instead.");
			}
			//Destroy(this.gameObject); 1.3.5									//Destroy the current AI to instaniate the deadObject replacement.
			gameObject.SetActive(false);
		}


		//When dying, we only want the code to be called once.
		if (!useDeadReplacement)
		{
			if (!deathTrigger)
			{
				if (useDieSound)
				{
					AudioSources[1].PlayOneShot(dieSound);
				}

				if (GetComponent<EmeraldLootSystem>() != null)
				{
					GetComponent<EmeraldLootSystem>().GenerateLoot();
				}
					
				deathTrigger = true;
			}

			if (useDustEffect)
			{
				#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
				ParticleSystem.EmissionModule emission = clone.emission;
				emission.enabled = false;
				#else
				clone.enableEmission = false;
				#endif
			}

			if (useAnimations && !deathAnimationFinished)
			{
				deathAnimationTimer += Time.deltaTime;

				if (!useMecanim)
				{
					anim.CrossFade(deathAnimation.name);

					if (deathAnimationTimer >= deathAnimation.length + 3)
					{
						deathAnimationFinished = true;
						anim.enabled = false;
					}
				}

				if (useMecanim)
				{
					MecanimDie();

					if (AnimatorComponent.GetBool(DieParameter))
					{
						if (deathAnimationTimer >= AnimatorComponent.GetCurrentAnimatorClipInfo(0)[0].clip.length)
						{
							AnimatorComponent.enabled = false; //Updated
						}
					}

					if (deathAnimationTimer >= 5)
					{
						deathAnimationFinished = true;
						AnimatorComponent.enabled = false;
					}
				}
			}

			isDead = true;
			huntMode = false;
			attackTimer = 0;
			isGrazing = false;
			isFleeing = false;

			deathTimer += Time.deltaTime;

			//If using die sound, wait to deactivate until the die sound has played.
			if (dieSound != null)
			{
				if (useDieSound && deathTimer >= dieSound.length + 0.5f)
				{
					foreach (AudioSource A in AudioSources.ToArray())
					{
						A.enabled = false;
					}

					GetComponent<Emerald_Animal_AI>().enabled = false; 
				}
			}

			//If not using a die sound, disable after 2 seconds to ensure everyrthing has been deactivated.
			if (!useDieSound && deathTimer >= 1 || dieSound == null)
			{
				foreach (AudioSource A in AudioSources.ToArray())
				{
					A.Stop();
					A.enabled = false;
				}

				GetComponent<Emerald_Animal_AI>().enabled = false; 
			}
		}
	}

	public void MecanimIdle ()
	{
		AnimatorComponent.SetBool (IdleParameter, true);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
	
	}

	public void MecanimGraze1 ()
	{
		AnimatorComponent.SetBool (Graze1Parameter, true);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
	}

	public void MecanimGraze2 ()
	{
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, true);
		AnimatorComponent.SetBool (Graze3Parameter, false);
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
	}

	public void MecanimGraze3 ()
	{
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, true);
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
	}

	public void MecanimWalk ()
	{
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, true);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
	}

	public void MecanimRun ()
	{
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, true);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
	}

	public void MecanimIdleCombat ()
	{
		AnimatorComponent.SetBool(IdleCombatParameter, true);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(IdleParameter, false);
		AnimatorComponent.SetBool (Attack1Parameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
	}

	public void MecanimHit ()
	{
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, true);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
	}

	public void MecanimAttack1 ()
	{
		AnimatorComponent.SetBool(Attack1Parameter, true);
	}

	public void MecanimAttack2 ()
	{
		AnimatorComponent.SetBool(Attack2Parameter, true);
	}

	public void MecanimAttack3 ()
	{
		AnimatorComponent.SetBool(Attack3Parameter, true);
	}

	public void MecanimRunAttack ()
	{
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, true);
	}

	public void MecanimDie ()
	{
		AnimatorComponent.SetBool(DieParameter, true);
		AnimatorComponent.SetBool (IdleParameter, false);
		AnimatorComponent.SetBool(WalkParameter, false);
		AnimatorComponent.SetBool(RunParameter, false);
		AnimatorComponent.SetBool (IdleCombatParameter, false);
		AnimatorComponent.SetBool (HitParameter, false);
		AnimatorComponent.SetBool(Attack1Parameter, false);
		AnimatorComponent.SetBool(Attack2Parameter, false);
		AnimatorComponent.SetBool(Attack3Parameter, false);
		AnimatorComponent.SetBool(RunAttackParameter, false);
		AnimatorComponent.SetBool (Graze1Parameter, false);
		AnimatorComponent.SetBool (Graze2Parameter, false);
		AnimatorComponent.SetBool (Graze3Parameter, false);
	}
}
	
