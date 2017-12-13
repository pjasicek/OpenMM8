//Emerald - Animal AI by: Black Horizon Studios
//Version 1.3.2

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;

#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

#if UNITY_5_5 || UNITY_5_6
using UnityEditor.AI;
#endif

[CustomEditor(typeof(Emerald_Animal_AI))] 
//[CanEditMultipleObjects]
[System.Serializable]
public class Emerald_Animal_AI_Editor : Editor 
{

	bool showHelpOptions = true;

	/*
	enum ChaseType
	{
		_Time = 1,
		Distance = 2
	}
	*/

	//1.3 Added
	enum AnimationType
	{
		Legacy = 1,
		Mecanim = 2
	}

	//Animal Only
	enum AgressionDropDown
	{
		Cowardly = 1,
		Passive = 2,
		Hostile = 3,
		Defensive = 4
	}

	enum AgressionNPCDropDown
	{
		Cowardly = 0,
		Passive = 2,
		Hostile = 3,
		Defensive = 4
	}

	enum Alpha
	{
		Yes = 1,
		No = 2
	}

	enum AnimalOrNPC
	{
		Animal = 0,
		NPC = 1
		//Companion = 2
	}

	enum FleeType
	{
		Distance = 0,
		_Time = 1
		//Companion = 2
	}

	enum UseBreeding
	{
		Yes = 1,
		No = 2
	}

	enum NumberOfAttackAnimations
	{
		_1 = 1,
		_2 = 2,
		_3 = 3
	}

	enum NumberOfGrazeAnimations
	{
		_1 = 1,
		_2 = 2,
		_3 = 3
	}
	
	AnimationType editorAnimationType = AnimationType.Legacy;
	NumberOfAttackAnimations editorNumberOfAttackAnimations = NumberOfAttackAnimations._1;
	NumberOfGrazeAnimations editorNumberOfGrazeAnimations = NumberOfGrazeAnimations._1;

	AgressionNPCDropDown editorAgressionNPC = AgressionNPCDropDown.Hostile;
	AgressionDropDown editorAgression = AgressionDropDown.Cowardly;

	Alpha editorAlpha = Alpha.No;
	FleeType editorFleeType = FleeType._Time;

	AnimalOrNPC editorAnimalOrNPC = AnimalOrNPC.Animal;
	UseBreeding editorUseBreeding = UseBreeding.No;
	//ChaseType editorChaseType = ChaseType._Time;

	public string tagStr = "";

	public string[] TabString = new string[] {"Behavior Options", "Herd Options", "Attack Options", "Health Options", "Sound Options", "Pathfinding Options", "Range Options", "Animation Options", "Movement Options", "Tag Options", "Effects Options", "Breeding Options", "Show All Options"};
	//public string[] TabAllString = new string[] {"Show All Options"};


	SerializedObject temp;
	SerializedObject tabTemp;

	//Int Serialized Properties
	SerializedProperty RangedFleeAmountProp;
	SerializedProperty HerdRadiusProp;
	SerializedProperty HerdFollowRadiusProp;
	SerializedProperty AnimationTypeProp;
	SerializedProperty TabNumberProp;
	SerializedProperty TabNumberAllProp;
	SerializedProperty AggressionProp;
	SerializedProperty AITypeProp;
	SerializedProperty CurrentHealthProp;
	SerializedProperty AttackDamageMinProp;
	SerializedProperty AttackDamageMaxProp;
	SerializedProperty IsAlphaOrNotProp;
	SerializedProperty MaxPackSizeProp;
	SerializedProperty MaxDistanceFromHerdProp;
	SerializedProperty StartingHealthProp;
	SerializedProperty ExtraFleeSecondsProp;
	SerializedProperty MaxFleeDistanceProp;
	SerializedProperty FleeTypeProp;
	SerializedProperty FleeRadiusProp;
	SerializedProperty ChaseSecondsProp;
	SerializedProperty HuntRadiusProp;
	SerializedProperty WanderRangeProp;
	SerializedProperty GrazeLengthMinProp;
	SerializedProperty GrazeLengthMaxProp;
	SerializedProperty TotalGrazeAnimationsProp;
	SerializedProperty TotalAttackAnimationsProp;
	SerializedProperty UseBreedingProp;
	SerializedProperty CommonOddsProp;
	SerializedProperty UncommonOddsProp;
	SerializedProperty RareOddsProp;
	SerializedProperty SuperRareOddsProp;
	//SerializedProperty MaximumWalkingVelocityProp;

	//String Serialized Properties
	//SerializedProperty SendMessageForAIDamageProp;
	SerializedProperty SendMessageForPlayerDamageProp;
	SerializedProperty NPCNameProp;
	SerializedProperty AnimalTypeProp;

	//Float Serialized Properties
	SerializedProperty WalkRotateSpeedProp;
	SerializedProperty RunRotateSpeedProp;
	SerializedProperty takeDamageDelaySecondsProp;
	SerializedProperty AttackTimeMinProp;
	SerializedProperty AttackTimeMaxProp;
	SerializedProperty AttackDelaySecondsProp;
	SerializedProperty FootStepSecondsWalkProp;
	SerializedProperty FootStepSecondsProp;
	SerializedProperty MinSoundPitchProp;
	SerializedProperty MaxSoundPitchProp;
	SerializedProperty PathWidthProp;
	SerializedProperty LineYOffSetProp;
	SerializedProperty UpdateSpeedProp;
	SerializedProperty DeactivateAISecondsProp;
	SerializedProperty StoppingDistanceProp;
	SerializedProperty CoolDownSecondsProp;
	//SerializedProperty MaxChaseDistanceProp;
	SerializedProperty FreezeSecondsMinProp;
	SerializedProperty FreezeSecondsMaxProp;
	SerializedProperty WalkSpeedProp;
	SerializedProperty RunSpeedProp;
	SerializedProperty WalkAnimationSpeedProp;
	SerializedProperty RunAnimationSpeedProp;
	SerializedProperty IdleAnimationSpeedProp;
	SerializedProperty IdleCombatAnimationSpeedProp;
	SerializedProperty AttackAnimationSpeedProp;
	SerializedProperty GrazeAnimationSpeedProp;
	SerializedProperty DieAnimationSpeedProp;
	SerializedProperty BreedSecondsProp;
	SerializedProperty BreedCoolDownSecondsProp;
	SerializedProperty BabySecondsProp;
	SerializedProperty BaseOffsetNavProp;
	
	//Bool Serialized Properties
	SerializedProperty PlayerUsesSeparateLayerProp;
	SerializedProperty HerdFullProp;
	SerializedProperty AutoGenerateAlphaProp;
	SerializedProperty AlphaWaitForHerdProp;
	SerializedProperty AutoCalculateDelaySecondsProp;
	SerializedProperty UseDeadReplacementProp;
	SerializedProperty UseWeaponSoundProp;
	SerializedProperty UseAttackSoundProp;
	SerializedProperty UseInjuredSoundsProp;
	SerializedProperty UseImpactSoundsProp;
	SerializedProperty UseAnimalSoundProp;
	SerializedProperty UseDustEffectProp;
	SerializedProperty UseDieSoundProp;
	SerializedProperty PlaySoundOnFleeProp;
	SerializedProperty UseWalkSoundProp;
	SerializedProperty UseRunSoundProp;
	SerializedProperty DrawWaypointsProp;
	SerializedProperty DrawPathsProp;
	SerializedProperty AlignAIProp;
	SerializedProperty UseVisualRadiusProp;
	SerializedProperty ReturnsToStartProp;
	SerializedProperty ReturnBackToStartingPointProtectionProp;
	SerializedProperty UseAnimationsProp;
	SerializedProperty UseTurnAnimationProp;
	SerializedProperty UseRunAttackAnimationsProp;
	SerializedProperty UseRunAttacksProp;
	SerializedProperty UseBloodProp;
	SerializedProperty IsBabyProp;
	SerializedProperty UseBreedEffectProp;
	SerializedProperty UseHitAnimationProp;
	SerializedProperty UseRootMotionProp;
	
	//Object Serialized Properties
	SerializedProperty DeadObjectProp;
	SerializedProperty WeaponSoundProp;
	SerializedProperty DieSoundProp;
	SerializedProperty FleeSoundProp;
	SerializedProperty RunSoundProp;
	SerializedProperty PathMaterialProp;
	SerializedProperty WalkSoundProp;
	SerializedProperty IdleAnimationProp;
	SerializedProperty IdleBattleAnimationProp;
	SerializedProperty Graze1AnimationProp;
	SerializedProperty Graze2AnimationProp;
	SerializedProperty Graze3AnimationProp;
	SerializedProperty WalkAnimationProp;
	SerializedProperty RunAnimationProp;
	SerializedProperty TurnAnimationProp;
	SerializedProperty HitAnimationProp;
	SerializedProperty AttackAnimation1Prop;
	SerializedProperty AttackAnimation2Prop;
	SerializedProperty AttackAnimation3Prop;
	SerializedProperty AttackAnimation4Prop;
	SerializedProperty AttackAnimation5Prop;
	SerializedProperty AttackAnimation6Prop;
	SerializedProperty RunAttackAnimationProp;
	SerializedProperty DeathAnimationProp;
	SerializedProperty BloodEffectProp;
	SerializedProperty DustEffectProp;
	SerializedProperty FullGrownPrefabProp;
	SerializedProperty BreedEffectProp;
	SerializedProperty BabyPrefabCommonProp;
	SerializedProperty BabyPrefabUncommonProp;
	SerializedProperty BabyPrefabRareProp;
	SerializedProperty BabyPrefabSuperRareProp;

	
	//List
	SerializedProperty AttackSoundProp;

	//Color
	SerializedProperty PathColorProp;
	SerializedProperty FleeRadiusColorProp;
	SerializedProperty HuntRadiusColorProp;
	SerializedProperty WanderRangeColorProp;
	SerializedProperty StoppingRangeColorProp;

	SerializedProperty serializedProperty;

	//Vector3
	SerializedProperty BreedEffectOffSetProp;

	//SerializedObject _Object;
	

	void OnEnable () 
	{
		//Emerald_Animal_AI self = (Emerald_Animal_AI)target;

		temp = new SerializedObject(targets);
		tabTemp = new SerializedObject(target);

		//Int Serialized Properties
		RangedFleeAmountProp = tabTemp.FindProperty ("rangedFleeAmount");
		HerdRadiusProp = tabTemp.FindProperty ("herdRadius");
		HerdFollowRadiusProp = tabTemp.FindProperty ("herdFollowRadius");
		AnimationTypeProp = tabTemp.FindProperty ("AnimationType");
		TabNumberProp = tabTemp.FindProperty ("TabNumber");
		//TabNumberAllProp = temp.FindProperty ("TabNumberAll");
		AggressionProp = temp.FindProperty ("aggression");
		AITypeProp = temp.FindProperty ("AIType");
		CurrentHealthProp = temp.FindProperty ("currentHealth");
		AttackDamageMinProp = temp.FindProperty ("attackDamageMin");
		AttackDamageMaxProp = temp.FindProperty ("attackDamageMax");
		IsAlphaOrNotProp = temp.FindProperty ("isAlphaOrNot");
		MaxPackSizeProp = temp.FindProperty ("maxPackSize");
		MaxDistanceFromHerdProp = temp.FindProperty ("maxDistanceFromHerd");
		StartingHealthProp = temp.FindProperty ("startingHealth");
		ExtraFleeSecondsProp = temp.FindProperty ("extraFleeSeconds");
		MaxFleeDistanceProp = temp.FindProperty ("maxFleeDistance");
		FleeTypeProp = temp.FindProperty ("fleeType");
		FleeRadiusProp = temp.FindProperty ("fleeRadius");
		ChaseSecondsProp = temp.FindProperty ("chaseSeconds");
		HuntRadiusProp = temp.FindProperty ("huntRadius");
		WanderRangeProp = temp.FindProperty ("wanderRange");
		GrazeLengthMinProp = temp.FindProperty ("grazeLengthMin");
		GrazeLengthMaxProp = temp.FindProperty ("grazeLengthMax");
		TotalGrazeAnimationsProp = temp.FindProperty ("totalGrazeAnimations");
		TotalAttackAnimationsProp = temp.FindProperty ("totalAttackAnimations");
		UseBreedingProp = temp.FindProperty ("UseBreeding");
		CommonOddsProp = temp.FindProperty ("commonOdds");
		UncommonOddsProp = temp.FindProperty ("uncommonOdds");
		RareOddsProp = temp.FindProperty ("rareOdds");
		SuperRareOddsProp = temp.FindProperty ("superRareOdds");
		//ChaseTypeProp = temp.FindProperty ("ChaseType");
		//MaximumWalkingVelocityProp = temp.FindProperty ("maximumWalkingVelocity");

		//Float Serialized Properties
		WalkRotateSpeedProp = temp.FindProperty ("walkRotateSpeed");
		RunRotateSpeedProp = temp.FindProperty ("runRotateSpeed");
		takeDamageDelaySecondsProp = temp.FindProperty ("takeDamageDelaySeconds");
		AttackTimeMinProp = temp.FindProperty ("attackTimeMin");
		AttackTimeMaxProp = temp.FindProperty ("attackTimeMax");
		AttackDelaySecondsProp = temp.FindProperty ("attackDelaySeconds");
		FootStepSecondsWalkProp = temp.FindProperty ("footStepSecondsWalk");
		FootStepSecondsProp = temp.FindProperty ("footStepSeconds");
		MinSoundPitchProp = temp.FindProperty ("minSoundPitch");
		MaxSoundPitchProp = temp.FindProperty ("maxSoundPitch");
		PathWidthProp = temp.FindProperty ("pathWidth");
		LineYOffSetProp = temp.FindProperty ("lineYOffSet");
		UpdateSpeedProp = temp.FindProperty ("updateSpeed");
		DeactivateAISecondsProp = temp.FindProperty ("deactivateAISeconds");
		StoppingDistanceProp = temp.FindProperty ("stoppingDistance");
		CoolDownSecondsProp = temp.FindProperty ("coolDownSeconds");
		//MaxChaseDistanceProp = temp.FindProperty ("maxChaseDistance");
		FreezeSecondsMinProp = temp.FindProperty ("freezeSecondsMin");
		FreezeSecondsMaxProp = temp.FindProperty ("freezeSecondsMax");
		WalkSpeedProp = temp.FindProperty ("walkSpeed");
		RunSpeedProp = temp.FindProperty ("runSpeed");
		WalkAnimationSpeedProp = temp.FindProperty ("walkAnimationSpeed");
		RunAnimationSpeedProp = temp.FindProperty ("runAnimationSpeed");
		IdleAnimationSpeedProp = temp.FindProperty ("idleAnimationSpeed");
		IdleCombatAnimationSpeedProp = temp.FindProperty ("idleCombatAnimationSpeed");
		AttackAnimationSpeedProp = temp.FindProperty ("attackAnimationSpeed");
		GrazeAnimationSpeedProp = temp.FindProperty ("grazeAnimationSpeed");
		DieAnimationSpeedProp = temp.FindProperty ("dieAnimationSpeed");
		BreedSecondsProp = temp.FindProperty ("breedSeconds");
		BreedCoolDownSecondsProp = temp.FindProperty ("breedCoolDownSeconds");
		BabySecondsProp = temp.FindProperty ("babySeconds");
		BaseOffsetNavProp = temp.FindProperty ("baseOffsetNav");

		//String Serialized Properties
		//SendMessageForAIDamageProp = temp.FindProperty ("SendMessageForAIDamage");
		SendMessageForPlayerDamageProp = temp.FindProperty ("SendMessageForPlayerDamage");
		NPCNameProp = temp.FindProperty ("NPCName");
		AnimalTypeProp = temp.FindProperty ("animalNameType");

		//Bool Serialized Properties
		PlayerUsesSeparateLayerProp = temp.FindProperty ("playerUsesSeparateLayer");
		HerdFullProp = temp.FindProperty ("herdFull");
		AutoGenerateAlphaProp = temp.FindProperty ("autoGenerateAlpha");
		AlphaWaitForHerdProp = temp.FindProperty ("alphaWaitForHerd");
		AutoCalculateDelaySecondsProp = temp.FindProperty ("autoCalculateDelaySeconds");
		UseDeadReplacementProp = temp.FindProperty ("useDeadReplacement");
		UseWeaponSoundProp = temp.FindProperty ("useWeaponSound");
		UseAttackSoundProp = temp.FindProperty ("useAttackSound");
		UseAnimalSoundProp = temp.FindProperty ("useAnimalSounds");
		UseInjuredSoundsProp = temp.FindProperty ("useInjuredSounds");
		UseImpactSoundsProp = temp.FindProperty ("useImpactSounds");
		UseDustEffectProp = temp.FindProperty ("useDustEffect");
		UseDieSoundProp = temp.FindProperty ("useDieSound");
		PlaySoundOnFleeProp = temp.FindProperty ("playSoundOnFlee");
		UseWalkSoundProp = temp.FindProperty ("useWalkSound");
		UseRunSoundProp = temp.FindProperty ("useRunSound");
		DrawWaypointsProp = temp.FindProperty ("drawWaypoints");
		DrawPathsProp = temp.FindProperty ("drawPaths");
		AlignAIProp = temp.FindProperty ("alignAI");
		UseVisualRadiusProp = temp.FindProperty ("useVisualRadius");
		ReturnsToStartProp = temp.FindProperty ("returnsToStart");
		ReturnBackToStartingPointProtectionProp = temp.FindProperty ("returnBackToStartingPointProtection");
		UseAnimationsProp = temp.FindProperty ("useAnimations");
		UseTurnAnimationProp = temp.FindProperty ("useTurnAnimation");
		UseRunAttackAnimationsProp = temp.FindProperty ("useRunAttackAnimations");
		UseRunAttacksProp = temp.FindProperty ("useRunAttacks");
		UseBloodProp = temp.FindProperty ("useBlood");
		IsBabyProp = temp.FindProperty ("isBaby");
		UseBreedEffectProp = temp.FindProperty ("useBreedEffect");
		UseHitAnimationProp = temp.FindProperty ("useHitAnimation");
		UseRootMotionProp = temp.FindProperty ("useRootMotion");


		//Object Serialized Properties
		DeadObjectProp = temp.FindProperty ("deadObject");
		WeaponSoundProp = temp.FindProperty ("weaponSound");
		DieSoundProp = temp.FindProperty ("dieSound");
		FleeSoundProp = temp.FindProperty ("fleeSound");
		RunSoundProp = temp.FindProperty ("runSound");
		PathMaterialProp = temp.FindProperty ("pathMaterial");
		WalkSoundProp = temp.FindProperty ("walkSound");
		IdleAnimationProp = temp.FindProperty ("idleAnimation");
		IdleBattleAnimationProp = temp.FindProperty ("idleBattleAnimation");
		Graze1AnimationProp = temp.FindProperty ("graze1Animation");
		Graze2AnimationProp = temp.FindProperty ("graze2Animation");
		Graze3AnimationProp = temp.FindProperty ("graze3Animation");
		WalkAnimationProp = temp.FindProperty ("walkAnimation");
		RunAnimationProp = temp.FindProperty ("runAnimation");
		TurnAnimationProp = temp.FindProperty ("turnAnimation");
		HitAnimationProp = temp.FindProperty ("hitAnimation");
		AttackAnimation1Prop = temp.FindProperty ("attackAnimation1");
		AttackAnimation2Prop = temp.FindProperty ("attackAnimation2");
		AttackAnimation3Prop = temp.FindProperty ("attackAnimation3");
		AttackAnimation4Prop = temp.FindProperty ("attackAnimation4");
		AttackAnimation5Prop = temp.FindProperty ("attackAnimation5");
		AttackAnimation6Prop = temp.FindProperty ("attackAnimation6");
		RunAttackAnimationProp = temp.FindProperty ("runAttackAnimation");
		DeathAnimationProp = temp.FindProperty ("deathAnimation");
		BloodEffectProp = temp.FindProperty ("bloodEffect");
		DustEffectProp = temp.FindProperty ("dustEffect");
		FullGrownPrefabProp = temp.FindProperty ("fullGrownPrefab");
		BreedEffectProp = temp.FindProperty ("breedEffect");
		BabyPrefabCommonProp = temp.FindProperty ("babyPrefabCommon");
		BabyPrefabUncommonProp = temp.FindProperty ("babyPrefabUncommon");
		BabyPrefabRareProp = temp.FindProperty ("babyPrefabRare");
		BabyPrefabSuperRareProp = temp.FindProperty ("babyPrefabSuperRare");

		//Color Serialized Properties
		PathColorProp = temp.FindProperty ("pathColor");
		FleeRadiusColorProp = temp.FindProperty ("fleeRadiusColor");
		HuntRadiusColorProp = temp.FindProperty ("huntRadiusColor");
		WanderRangeColorProp = temp.FindProperty ("wanderRangeColor");
		StoppingRangeColorProp = temp.FindProperty ("stoppingRangeColor");

		//Vector3
		BreedEffectOffSetProp = temp.FindProperty ("breedEffectOffSet");
	}

	public override void OnInspectorGUI () 
	{
		temp.Update ();
		Emerald_Animal_AI self = (Emerald_Animal_AI)target;


		float thirdOfScreen = Screen.width/3-10;
		int sizeOfHideButtons = 18;

		EditorGUILayout.LabelField("Emerald - Animal AI System Editor", EditorStyles.boldLabel);
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("AI Info", EditorStyles.boldLabel);


		EditorGUILayout.HelpBox("Name: " + NPCNameProp.stringValue, MessageType.None, true);

		if (!HerdFullProp.boolValue){
		EditorGUILayout.HelpBox("Type: " + AnimalTypeProp.stringValue, MessageType.None, true);
		}

		EditorGUILayout.HelpBox("Health: " + CurrentHealthProp.intValue + " points", MessageType.None, true);

		if (AggressionProp.intValue > 2)
		{
			EditorGUILayout.HelpBox("Damage: " + AttackDamageMinProp.intValue + " - " + AttackDamageMaxProp.intValue + " points", MessageType.None, true);
		}
		if (AggressionProp.intValue <= 2)
		{
			EditorGUILayout.HelpBox("Damage: None", MessageType.None, true);
		}

		if (AggressionProp.intValue == 1 || AggressionProp.intValue == 0)
		{
			EditorGUILayout.HelpBox("Aggression: " + "Coward", MessageType.None, true);
		}
		
		if (AggressionProp.intValue == 2)
		{
			EditorGUILayout.HelpBox("Aggression: " + "Passive", MessageType.None, true);
		}
		
		if (AggressionProp.intValue == 3)
		{
			EditorGUILayout.HelpBox("Aggression: " + "Hotsile", MessageType.None, true);
		}
		
		if (AggressionProp.intValue == 4)
		{
			EditorGUILayout.HelpBox("Aggression: " + "Defensive", MessageType.None, true);
		}
			
		string displayString = "";


		for (int i = 0; i < self.EmeraldTags.Count; i++) 
		{
			displayString += self.EmeraldTags[i] + ", ";
		}

		if (AggressionProp.intValue < 2)
		{
			EditorGUILayout.HelpBox("Flees From: " + displayString, MessageType.None, true);
		}

		if (AggressionProp.intValue == 3)
		{
			EditorGUILayout.HelpBox("Attacks: " + displayString, MessageType.None, true);
		}

		if (AggressionProp.intValue == 4)
		{
			EditorGUILayout.HelpBox("Attacks: A Defensive AI will only attack after being attacked first and does not rely on Tags." , MessageType.None, true);
		}

		if (AggressionProp.intValue == 1 && AITypeProp.intValue == 1)
		{
			AggressionProp.intValue = 0;
		}

		if (AggressionProp.intValue == 0 && AITypeProp.intValue == 0)
		{
			AggressionProp.intValue = 1;
		}

		/*
		if (AggressionProp.intValue >= 3 && AITypeProp.intValue == 0)
		{
			EditorGUILayout.HelpBox("Hunts: " + displayString, MessageType.None, true);
		}
		*/

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		//EditorGUILayout.Space();

		EditorGUILayout.LabelField("AI Option Tabs", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		if (showHelpOptions == true)
		{
			EditorGUILayout.HelpBox("The AI Option Tabs allow you to individually view each option as one tab rather than having one huge list. However, the Show All Options tab will allow everything to be viewed as one list, if desired.", MessageType.None, true);
		}

		EditorGUILayout.Space();


		tabTemp.Update ();

		TabNumberProp.intValue = GUILayout.SelectionGrid (TabNumberProp.intValue, TabString, 2);

		tabTemp.ApplyModifiedProperties ();



		if (AggressionProp.intValue < 3 && TabNumberProp.intValue == 2)
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.HelpBox("Passive and Coward animals cannot attack. If you'd like this AI to attack, make it either Defensive or Aggressive under the AI Setup Options tab.", MessageType.Warning, true);

			GUI.color = Color.red;
			//TabStrings[2] = "Attack";
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if (TabNumberProp.intValue == 0 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.LabelField("AI Setup", EditorStyles.boldLabel);

			editorAnimalOrNPC = (AnimalOrNPC)AITypeProp.intValue;
			editorAnimalOrNPC = (AnimalOrNPC)EditorGUILayout.EnumPopup("AI Type", editorAnimalOrNPC);
			AITypeProp.intValue = (int)editorAnimalOrNPC;

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("AI Type determins whether your an Animal or NPC. For any wildlife AI, the Animal setting should be used. For any NPC AI, enemies, humanoid, creatures, etc. the NPC setting should be used.", MessageType.None, true);
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			//Animal Behavior
			if (AITypeProp.intValue == 0)
			{
				editorAgression = (AgressionDropDown)AggressionProp.intValue;
				editorAgression = (AgressionDropDown)EditorGUILayout.EnumPopup("Animal Behavior Type", editorAgression);
				AggressionProp.intValue = (int)editorAgression;
			

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Behavior Type determins whether your Animal is Cowardly, Passive, Hostile, or Defensive.", MessageType.None, true);
				}
				
				if (showHelpOptions == true && AggressionProp.intValue == 1)
				{
					EditorGUILayout.HelpBox("A Cowardly Animal will flee when in range of its Flee Tags and will not fight back.", MessageType.None, true);
				}
				
				if (showHelpOptions == true && AggressionProp.intValue == 2)
				{
					EditorGUILayout.HelpBox("A Passive Animal will never attack or flee. Even if hit, it will continue to wander around within its Wandering Range.", MessageType.None, true);
				}
				
				if (showHelpOptions == true && AggressionProp.intValue == 3)
				{
					EditorGUILayout.HelpBox("A Hostile Animal will automatically attack when in range of its Attack Tags.", MessageType.None, true);
				}
				
				if (showHelpOptions == true && AggressionProp.intValue == 4)
				{
					EditorGUILayout.HelpBox("A Defensive Animal will only attack when attacked first. If a defensize animal is attacked first, it will attempt to attack and kill the attacker.", MessageType.None, true);
				}

				EditorGUILayout.Space();
			}

			//NPC Behavior
			if (AITypeProp.intValue == 1)
			{
				editorAgressionNPC = (AgressionNPCDropDown)AggressionProp.intValue;
				editorAgressionNPC = (AgressionNPCDropDown)EditorGUILayout.EnumPopup("NPC Behavior Type", editorAgressionNPC);
				AggressionProp.intValue = (int)editorAgressionNPC;
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Behavior Type determins whether your NPC is Cowardly, Passive, Hostile, or Defensive.", MessageType.None, true);
				}
				
				if (showHelpOptions == true && AggressionProp.intValue == 1)
				{
					EditorGUILayout.HelpBox("A Cowardly NPC will flee when in range of its Flee Tags and will not fight back.", MessageType.None, true);
				}

				if (showHelpOptions == true && AggressionProp.intValue == 2)
				{
					EditorGUILayout.HelpBox("A Passive NPC will never attack or flee. Even if hit, it will continue to wander around within its Wandering Range.", MessageType.None, true);
				}

				if (showHelpOptions == true && AggressionProp.intValue == 3)
				{
					EditorGUILayout.HelpBox("A Hostile NPC will automatically attack when in range of its Attack Tags.", MessageType.None, true);
				}

				if (showHelpOptions == true && AggressionProp.intValue == 4)
				{
					EditorGUILayout.HelpBox("A Defensive NPC will only attack when attacked first. If a defensize NCP is attacked first, it will attempt to attack and kill the attacker.", MessageType.None, true);
				}

				EditorGUILayout.Space();

			}

			EditorGUILayout.Space();

			if (AITypeProp.intValue == 1)
			{
				EditorGUILayout.LabelField("NPC Name", EditorStyles.miniLabel);
				NPCNameProp.stringValue = EditorGUILayout.TextField(NPCNameProp.stringValue, GUILayout.MaxHeight(75));

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The name of your NPC can be useful for calling the name from an external script for things like quests, dialogue, UI, etc.", MessageType.None, true);
				}

				EditorGUILayout.Space();
			}

			if (AITypeProp.intValue == 0)
			{
				EditorGUILayout.LabelField("Animal Name", EditorStyles.miniLabel);
				NPCNameProp.stringValue = EditorGUILayout.TextField(NPCNameProp.stringValue, GUILayout.MaxHeight(75));

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The name of your Animal can be useful for calling the name from an external script for things like quests, dialogue, UI, etc.", MessageType.None, true);
				}

				EditorGUILayout.Space();
			}

			EditorGUILayout.Space();
		}

		if (TabNumberProp.intValue == 1 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Herd Options", EditorStyles.boldLabel);

			EditorGUILayout.Space();

			if (AggressionProp.intValue != 2)
			{
				HerdFullProp.boolValue = EditorGUILayout.Toggle ("Disable Herds?",HerdFullProp.boolValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Disable Herds stops the AI from dynamically forming a herd, group, or pack with any other AI with the same type.", MessageType.None, true);
				}
			}

			if (AggressionProp.intValue == 2)
			{
				EditorGUILayout.HelpBox("Passive AI do not dynamically form herds as they tend to be used for farm animals and wandering NPCs.", MessageType.Warning, true);
			}

			EditorGUILayout.Space();

			if (!HerdFullProp.boolValue && AggressionProp.intValue != 2)
			{
				if (AITypeProp.intValue == 1)
				{
					EditorGUILayout.LabelField("NPC Type", EditorStyles.miniLabel);
					AnimalTypeProp.stringValue = EditorGUILayout.TextField(AnimalTypeProp.stringValue, GUILayout.MaxHeight(75));

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The NPC Type helps with Emerald dynamically forming groups based on your NPC Name Type. So, only NPCs with the same NPC Name Type will create groups.", MessageType.None, true);
					}

					EditorGUILayout.Space();
					EditorGUILayout.Space();
				}

				if (AITypeProp.intValue == 0)
				{
					EditorGUILayout.LabelField("Animal Type Name", EditorStyles.miniLabel);
					AnimalTypeProp.stringValue = EditorGUILayout.TextField(AnimalTypeProp.stringValue, GUILayout.MaxHeight(75));

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The Animal Type helps with Emerald dynamically forming herds and packs based on your Animal Name Type. So, only animals with the same Animal Name Type will create herds or packs.", MessageType.None, true);
					}
				
					EditorGUILayout.Space();
					EditorGUILayout.Space();
				}

				AutoGenerateAlphaProp.boolValue = EditorGUILayout.Toggle ("Auto Generate Alpha?",AutoGenerateAlphaProp.boolValue);

				EditorGUILayout.Space();

				if (!AutoGenerateAlphaProp.boolValue)
				{
					editorAlpha = (Alpha)IsAlphaOrNotProp.intValue;
					editorAlpha = (Alpha)EditorGUILayout.EnumPopup("Is this animal an Alpha?", editorAlpha);
					IsAlphaOrNotProp.intValue = (int)editorAlpha;
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("If Auto Generate Alpha is enabled, alphas will be generated automatically. There is a 1 in 5 chance of an animal being an alpha. If this option is disabled, you can customize which animals are alphas manually.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				MaxPackSizeProp.intValue = EditorGUILayout.IntSlider ("Max Pack Size", MaxPackSizeProp.intValue, 1, 10);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Controls the max pack size for this animal, if they're generated to become an alpha.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				HerdRadiusProp.intValue = EditorGUILayout.IntSlider ("Herd Radius", HerdRadiusProp.intValue, 10, 100);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Herd Radius controls how close this AI will graze around its Alpha. The higher the value, the more space is between this AI and the Alpha. This is useful to help keep herds from being too close to each other.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				HerdFollowRadiusProp.intValue = EditorGUILayout.IntSlider ("Herd Follow Radius", HerdFollowRadiusProp.intValue, 10, 100);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Herd Follow Radius controls how close this AI will follow its Alpha. The higher the value, the more space is between this AI and the Alpha. This is useful to help keep herds from being too close to each other.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				AlphaWaitForHerdProp.boolValue = EditorGUILayout.Toggle ("Alpha Waits for Herd?",AlphaWaitForHerdProp.boolValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("This toggles whether or not you want the alpha to wait for its herd if a member's distance from the alpha becomes too far away.", MessageType.None, true);
				}

				if (AlphaWaitForHerdProp.boolValue)
				{
					EditorGUILayout.Space();
	
					//EditorGUILayout.PropertyField(MaxDistanceFromHerdProp, true);

					MaxDistanceFromHerdProp.intValue = EditorGUILayout.IntSlider ("Max Distance From Herd", MaxDistanceFromHerdProp.intValue, 1, 100);

					/*
					if (MaxDistanceFromHerdProp.intValue > 100)
					{
						MaxDistanceFromHerdProp.intValue = 100;
					}
					*/


					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("This controls when the alpha will wait for its herd. This happens when this distance is met.", MessageType.None, true);
					}
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}

		if (TabNumberProp.intValue == 2 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			if (AggressionProp.intValue > 2)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				
				EditorGUILayout.LabelField("Attack Options", EditorStyles.boldLabel);

				EditorGUILayout.Space();

				if (AggressionProp.intValue >= 3)
				{
					//AttackDamageMinProp.intValue = EditorGUILayout.IntSlider ("Attack Damage Min", AttackDamageMinProp.intValue, 0, 200);
					AttackDamageMinProp.intValue = EditorGUILayout.IntField("Attack Damage Min", AttackDamageMinProp.intValue);
					//AttackDamageMaxProp.intValue = EditorGUILayout.IntSlider ("Attack Damage Max", AttackDamageMaxProp.intValue, 0, 200);
					AttackDamageMaxProp.intValue = EditorGUILayout.IntField("Attack Damage Max", AttackDamageMaxProp.intValue);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Attack Damage calls the Damage function within Emerald. The amount set above is the damage the animal will do.", MessageType.None, true);
					}
				}
				
				EditorGUILayout.Space();

				EditorGUILayout.HelpBox("The Attack Speeds are both automatically adjusted if they happen to be lower than your current attack animation with the set attack animation speed. This ensures that your attack animation will always play and not be cut off.", MessageType.Info, true);
				AttackTimeMinProp.floatValue = EditorGUILayout.Slider ("Attack Speed Min", (float)AttackTimeMinProp.floatValue, 0.5f, 6.0f);
				AttackTimeMaxProp.floatValue = EditorGUILayout.Slider ("Attack Speed Max", (float)AttackTimeMaxProp.floatValue, 1.0f, 6.0f);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Attack Speed controls how fast your AI can attack. Emerald calculates your AI's animations to match your attack speed. Note: The legnth of your AI's attack animation is applied to your attack speed.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				UseRunAttacksProp.boolValue = EditorGUILayout.Toggle ("Use Run Attacks", UseRunAttacksProp.boolValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Use Run Attacks controls whether or not your AI will attack their opponent while running. Enabling this options also allows the option to add a run attack animation in the Animation Options.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				AutoCalculateDelaySecondsProp.boolValue = EditorGUILayout.Toggle ("Auto Calculate Delay Seconds?",AutoCalculateDelaySecondsProp.boolValue);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Auto Calculate Delay Seconds will calcuate the optimum delay for your attack animations to hit your target.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				if (!AutoCalculateDelaySecondsProp.boolValue)
				{
					EditorGUILayout.Space();
					
					AttackDelaySecondsProp.floatValue = EditorGUILayout.Slider ("Attack Delay", (float)AttackDelaySecondsProp.floatValue, 0f, 2.0f);
					
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The Attack Delay (meassured in seconds) controls the delay that triggers a damage call. This is useful for if your animations need some time to reach the attacker.", MessageType.None, true);
					}
				}

				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Player Damage Function Call", EditorStyles.miniLabel);
				SendMessageForPlayerDamageProp.stringValue = EditorGUILayout.TextField(SendMessageForPlayerDamageProp.stringValue, GUILayout.MaxHeight(75));

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Player Damage Function Call allows you to call a custom function for damaging your player.", MessageType.None, true);
				}

				EditorGUILayout.HelpBox("Note: Make sure the function exists. If it doesn't, there will be an error. You can always revert by using the default player damage call 'DamagePlayer'.", MessageType.Info, true);

				EditorGUILayout.Space();
			}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
		}


		if (TabNumberProp.intValue == 3 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Health Options", EditorStyles.boldLabel);

			StartingHealthProp.intValue = EditorGUILayout.IntField("Starting Health", StartingHealthProp.intValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Starting Health is the health that your AI will start with.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			CurrentHealthProp.intValue = EditorGUILayout.IntField("Current Health", CurrentHealthProp.intValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Current Health is the current health your AI has. This is tracked in real time.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			takeDamageDelaySecondsProp.floatValue = EditorGUILayout.Slider ("Receive Damage Delay", (float)takeDamageDelaySecondsProp.floatValue, 0f, 2.0f);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Receive Damage Delay (meassured in seconds) controls the delay that triggers a damage call. This is useful for if your animations need some time to reach the attacker.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can set your AI's health. When its health reaches 0, the animal will die and it will spawn a dead replacement.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			UseDeadReplacementProp.boolValue = EditorGUILayout.Toggle ("Use Dead Replacement?",UseDeadReplacementProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Use Dead Replacement determines whether or not your AI will use a dead object replacement on death. Note: If set to true, this will disable the death animation from being used under the Animation Options.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			if (UseDeadReplacementProp.boolValue)
			{
				//Here
				//bool deadObject  = !EditorUtility.IsPersistent (self);
				DeadObjectProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Dead Object", DeadObjectProp.objectReferenceValue, typeof(GameObject), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Here you can set your animal's dead GameObject replacement.", MessageType.None, true);
				}
			}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
		}


		if (TabNumberProp.intValue == 4 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Sound Options", EditorStyles.boldLabel);

			EditorGUILayout.Space();

			if (AggressionProp.intValue >= 3)
			{
				UseWeaponSoundProp.boolValue = EditorGUILayout.Toggle ("Use Weapon Sound?",UseWeaponSoundProp.boolValue);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Weapon Sound is the sound that plays when your AI attacks. This is different than the attack sound. A weapon sound can be something like a swoosh or swing sound effect to simulate the sound of your AI's weapon making a noise. The pitch of these sounds are also varied based on your sound pitch randomness.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				
				if (UseWeaponSoundProp.boolValue)
				{
					//bool weaponSound  = !EditorUtility.IsPersistent (self);
					WeaponSoundProp.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField ("Weapon Sound", WeaponSoundProp.objectReferenceValue, typeof(AudioClip), true);
					
					EditorGUILayout.Space();
				}

				EditorGUILayout.Space();

				UseAttackSoundProp.boolValue = EditorGUILayout.Toggle ("Use Attack Sound?",UseAttackSoundProp.boolValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Use Attacks Sounds determines whether or not your AI will make a sound when it attacks.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Attack Sounds");
				if (UseAttackSoundProp.boolValue)
				{
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Below you can add an attack sound. For each sound you add, Emerald will randomly play it as one of the possible sounds the AI will make while it is attacking. This can be grunts, shouts, roars, growls, etc. If desired, slots can be left blank so a sound isn't playing every time.", MessageType.None, true);
					}

						if(self.attackSounds != null)
						{
							if(self.attackSounds.Count > 0)
							{
								EditorGUILayout.Space ();
								EditorGUILayout.Space ();

								for (int j = 0; j < self.attackSounds.Count; ++j)
								{
									EditorGUILayout.BeginHorizontal ();
									self.attackSounds[j] = (AudioClip)EditorGUILayout.ObjectField ("Attack Sound " + (j + 1), self.attackSounds[j], typeof(AudioClip), true);
									EditorGUILayout.EndHorizontal ();

									GUILayout.BeginHorizontal();
									GUILayout.Space(75);
									if (GUILayout.Button("Remove Sound"))
									{
										self.attackSounds.RemoveAt(j);
										--j;
									}
									GUILayout.Space(75);
									GUILayout.EndHorizontal();

									EditorGUILayout.Space ();
									EditorGUILayout.Space ();
								}
							}

							if(self.attackSounds.Count == 0)
							{
							EditorGUILayout.HelpBox("There are currently no Attack Sounds for this object. No sounds will play unless you have at least 1 attack sound assigned.", MessageType.Info);
							}

							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							if (GUILayout.Button("Add Sound", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
							{
								self.attackSounds.Add(new AudioClip());
							}
							GUILayout.EndHorizontal();
						}
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			UseInjuredSoundsProp.boolValue = EditorGUILayout.Toggle ("Use Injured Sound?",UseInjuredSoundsProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Use Injured Sounds determine whether or not your AI will make a sound when it receives damage.", MessageType.None, true);
			}

			if (UseInjuredSoundsProp.boolValue)
			{
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Below you can add an injured sound. For each sound you add, Emerald will randomly play it as one of the possible sounds the AI will make when receiving damage. This can be grunts, shouts, roars, growls, etc. If desired, slots can be left blank so a sound isn't playing every time.", MessageType.None, true);
				}

				EditorGUILayout.Space();	
				EditorGUILayout.Space();
				
				if (UseInjuredSoundsProp.boolValue)
				{
					if(self.injuredSounds != null)
					{
						if(self.injuredSounds.Count > 0)
						{
							EditorGUILayout.Space ();
							EditorGUILayout.Space ();

							for (int j = 0; j < self.injuredSounds.Count; ++j)
							{
								EditorGUILayout.BeginHorizontal ();
								self.injuredSounds[j] = (AudioClip)EditorGUILayout.ObjectField ("Injured Sound " + (j + 1), self.injuredSounds[j], typeof(AudioClip), true);
								EditorGUILayout.EndHorizontal ();

								GUILayout.BeginHorizontal();
								GUILayout.Space(75);
								if (GUILayout.Button("Remove Sound"))
								{
									self.injuredSounds.RemoveAt(j);
									--j;
								}
								GUILayout.Space(75);
								GUILayout.EndHorizontal();

								EditorGUILayout.Space ();
								EditorGUILayout.Space ();
							}
						}

						if(self.injuredSounds.Count == 0)
						{
							EditorGUILayout.HelpBox("There are currently no Injured Sounds for this object. No sounds will play unless you have at least 1 injured sound assigned.", MessageType.Info);
						}

						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Add Sound", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
						{
							self.injuredSounds.Add(new AudioClip());
						}
						GUILayout.EndHorizontal();
					}
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			UseImpactSoundsProp.boolValue = EditorGUILayout.Toggle ("Use Impact Sound?",UseImpactSoundsProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Use Impact Sounds determine whether or not your AI will make a sound when it receives damage.", MessageType.None, true);
			}


			if (UseImpactSoundsProp.boolValue)
			{
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Below you can add an imact sound. For each sound you add, Emerald will randomly play it as one of the possible sounds the AI will make when receiving damage. This can be blood impacts, body impacts, sword hits, armor clanks, etc. If desired, slots can be left blank so a sound isn't playing every time.", MessageType.None, true);
				}

				if(self.impactSounds != null)
				{
					if(self.impactSounds.Count > 0)
					{
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();

						for (int j = 0; j < self.impactSounds.Count; ++j)
						{
							EditorGUILayout.BeginHorizontal ();
							self.impactSounds[j] = (AudioClip)EditorGUILayout.ObjectField ("Impact Sound " + (j + 1), self.impactSounds[j], typeof(AudioClip), true);
							EditorGUILayout.EndHorizontal ();

							GUILayout.BeginHorizontal();
							GUILayout.Space(75);
							if (GUILayout.Button("Remove Sound"))
							{
								self.impactSounds.RemoveAt(j);
								--j;
							}
							GUILayout.Space(75);
							GUILayout.EndHorizontal();

							EditorGUILayout.Space ();
							EditorGUILayout.Space ();
						}
					}

					if(self.impactSounds.Count == 0)
					{
						EditorGUILayout.HelpBox("There are currently no Impact Sounds for this object. No sounds will play unless you have at least 1 impact sound assigned.", MessageType.Info);
					}

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Add Sound", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
					{
						self.impactSounds.Add(new AudioClip());
					}
					GUILayout.EndHorizontal();
				}

			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (AggressionProp.intValue == 2 && AITypeProp.intValue == 0)
			{
				UseAnimalSoundProp.boolValue = EditorGUILayout.Toggle ("Use Animal Sound?",UseAnimalSoundProp.boolValue);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Use Animal Sounds determine whether or not your AI will make a sound as it wanders.", MessageType.None, true);
				}


				//Adding Animal Sounds
				if (UseAnimalSoundProp.boolValue)
				{
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Below you can add an animal sound. For each sound you add, Emerald will randomly play it as one of the possible sounds the AI will make while it is wandering.", MessageType.None, true);
					}

					if(self.animalSounds != null)
					{
						if(self.animalSounds.Count > 0)
						{
							EditorGUILayout.Space ();
							EditorGUILayout.Space ();

							for (int j = 0; j < self.animalSounds.Count; ++j)
							{
								EditorGUILayout.BeginHorizontal ();
								self.animalSounds[j] = (AudioClip)EditorGUILayout.ObjectField ("Animal Sound " + (j + 1), self.animalSounds[j], typeof(AudioClip), true);
								EditorGUILayout.EndHorizontal ();

								GUILayout.BeginHorizontal();
								GUILayout.Space(75);
								if (GUILayout.Button("Remove Sound"))
								{
									self.animalSounds.RemoveAt(j);
									--j;
								}
								GUILayout.Space(75);
								GUILayout.EndHorizontal();

								EditorGUILayout.Space ();
								EditorGUILayout.Space ();
							}
						}

						if(self.animalSounds.Count == 0)
						{
							EditorGUILayout.HelpBox("There are currently no Animal Sounds for this object. No sounds will play unless you have at least 1 impact sound assigned.", MessageType.Info);
						}

						GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Add Sound", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
						{
							self.animalSounds.Add(new AudioClip());
						}
						GUILayout.EndHorizontal();
					}

				}
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
		



			UseDieSoundProp.boolValue = EditorGUILayout.Toggle ("Use Die Sound",UseDieSoundProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Controls whether or not a sound will be played on death.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			if (UseDieSoundProp.boolValue)
			{
				DieSoundProp.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField ("Die Sound", DieSoundProp.objectReferenceValue, typeof(AudioClip), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The sound the animal uses when it dies. Note: This only works if your animal isn't using a dead replacement. If your animal is using a dead replacement, put your dead sound on the dead replacement.", MessageType.None, true);
				}

				EditorGUILayout.Space();
			}

			EditorGUILayout.Space();


			if (AggressionProp.intValue <= 1)
			{
				PlaySoundOnFleeProp.boolValue = EditorGUILayout.Toggle ("Play Flee Sound?",PlaySoundOnFleeProp.boolValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Flee Sound is the sound the AI will use when it is triggered to flee", MessageType.None, true);
				}

				EditorGUILayout.Space();

				if (PlaySoundOnFleeProp.boolValue)
				{
					//bool fleeSound  = !EditorUtility.IsPersistent (self);
					FleeSoundProp.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField ("Flee Sound", FleeSoundProp.objectReferenceValue, typeof(AudioClip), true);
				}
			}

			EditorGUILayout.Space();

			if (AITypeProp.intValue <= 4)
			//if (AITypeProp.intValue <= 1 && AggressionProp.intValue != 3)
			{
				UseWalkSoundProp.boolValue = EditorGUILayout.Toggle ("Use Walk Sound?",UseWalkSoundProp.boolValue);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The sound the AI uses when it walks.", MessageType.None, true);
				}
				
				
				EditorGUILayout.Space();
				
				if (UseWalkSoundProp.boolValue)
				{
					//bool walkSound  = !EditorUtility.IsPersistent (self);
					WalkSoundProp.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField ("Walk Sound", WalkSoundProp.objectReferenceValue, typeof(AudioClip), true);
					
					EditorGUILayout.Space();
					
					FootStepSecondsWalkProp.floatValue = EditorGUILayout.Slider ("Walk Footstep Seconds", (float)FootStepSecondsWalkProp.floatValue, 0.1f, 1.5f);
					
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The Footstep Seconds controls the seconds in between each time the sound is playing while walking.", MessageType.None, true);
					}
				}
			}

			UseRunSoundProp.boolValue = EditorGUILayout.Toggle ("Use Run Sound?",UseRunSoundProp.boolValue);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The sound the AI uses when it runs.", MessageType.None, true);
			}


			EditorGUILayout.Space();

			if (UseRunSoundProp.boolValue)
			{
				//bool runSound  = !EditorUtility.IsPersistent (self);
				RunSoundProp.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField ("Run Sound", RunSoundProp.objectReferenceValue, typeof(AudioClip), true);

				EditorGUILayout.Space();

				FootStepSecondsProp.floatValue = EditorGUILayout.Slider ("Footstep Seconds", (float)FootStepSecondsProp.floatValue, 0.1f, 2.0f);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Footstep Seconds controls the seconds in between each time the sound is playing.", MessageType.None, true);
				}
			}

			EditorGUILayout.Space();
			
			MinSoundPitchProp.floatValue = EditorGUILayout.Slider ("Min Sound Pitch", (float)MinSoundPitchProp.floatValue, 0.5f, 1.5f);
			MaxSoundPitchProp.floatValue = EditorGUILayout.Slider ("Max Sound Pitch", (float)MaxSoundPitchProp.floatValue, 0.5f, 1.5f);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("This controls the min and max sound pitch for the animal's AudioSource. This affects all sounds adding various pitches to each animal keeping them unique.", MessageType.None, true);
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();
		}


		if (TabNumberProp.intValue == 5 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.LabelField("Pathfinding Options", EditorStyles.boldLabel);

			//Hold
			//selfmaxNumberOfActiveAnimals = EditorGUILayout.IntSlider ("Max Active Aniamls", selfmaxNumberOfActiveAnimals, 1, 100);
			
			DrawWaypointsProp.boolValue = EditorGUILayout.Toggle ("Draw Waypoints?",DrawWaypointsProp.boolValue);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Draw Waypoints determins if the AI will draw its current waypoint/destination. This can make it helpful for development/testing.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			DrawPathsProp.boolValue = EditorGUILayout.Toggle ("Draw Paths?",DrawPathsProp.boolValue);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Draw Paths determins if the AI will draw its current path to its current destination. This can make it helpful for development/testing.", MessageType.None, true);
			}

			if (DrawPathsProp.boolValue)
			{
				EditorGUILayout.Space();
				
				PathWidthProp.floatValue = EditorGUILayout.Slider ("Path Line Width", (float)PathWidthProp.floatValue, 1.0f, 100.0f);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Choose how wide you would like your Path Lines drawn.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				LineYOffSetProp.floatValue = EditorGUILayout.Slider ("Path Line Y Offset", (float)LineYOffSetProp.floatValue, 0.0f, 5.0f);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Path Line Y Offset will offset your path line on the Y axis based on the amount used on the slider above. This is useful if the Path Line is too high or too low.", MessageType.None, true);
				}
			}

			if (DrawPathsProp.boolValue)
			{
				EditorGUILayout.Space();

				PathColorProp.colorValue = EditorGUILayout.ColorField("Path Line Color", PathColorProp.colorValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Path Line Color allows you to customize what color you want the path lines to be.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				//bool pathMaterial  = !EditorUtility.IsPersistent (self);
				PathMaterialProp.objectReferenceValue = (Material)EditorGUILayout.ObjectField ("Path Line Material", PathMaterialProp.objectReferenceValue, typeof(Material), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Choose the material you want to be used for your Path Line. If no material is applied, a default one will be used. Note: The color of the default material is purple and can't be adjusted.", MessageType.None, true);
				}

			}

			EditorGUILayout.Space();

			/*
			selfenableDebugLogs = EditorGUILayout.Toggle ("Enable Debug Logs?",selfenableDebugLogs);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Enable Debug Logs can be useful to help balance your ecosystem. When an initial hunt or flee is triggered, it tells what's happening by the predator or prey's name.", MessageType.None, true);
			}
			*/

			EditorGUILayout.Space();

			AlignAIProp.boolValue = EditorGUILayout.Toggle ("Align AI?",AlignAIProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("If Align AI is enabled, it will automatically, and smoothly, align AI to the slope of the terrain. This allows much more realistic results.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			UpdateSpeedProp.floatValue = EditorGUILayout.Slider ("Update Speed", UpdateSpeedProp.floatValue, 0.01f, 2.0f);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Update Speed determines how often culling is checked. If an animal is culled, it will be disabled to increase performance. If an animal is visible, it will enable all components on that animal. The less often this option is updated, the more it increases performance, but animals may not react the second a player looks at them. So, it's best to balance this option with performance and playing quality.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			DeactivateAISecondsProp.intValue = EditorGUILayout.IntSlider ("Deactivate AI Seconds", DeactivateAISecondsProp.intValue, 10, 60);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Deactivate AI Seconds determines when your AI will be deactivated after being culled for the duration of the Deactivate AI Seconds. The lower the value the higher the performance. AI that are deactivated will be reactivated after they are unculled.", MessageType.None, true);
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}


		if (TabNumberProp.intValue == 6 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.LabelField("Range Options", EditorStyles.boldLabel);

			EditorGUILayout.Space();

			UseVisualRadiusProp.boolValue = EditorGUILayout.Toggle ("Use Visual Radiuses?",UseVisualRadiusProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Using Visual Radiuses will visually render the radiuses in the scene view. This makes it easy to see where your AI's Ranges are.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			StoppingDistanceProp.floatValue = EditorGUILayout.Slider ("Stopping Distance", StoppingDistanceProp.floatValue, 0.1f, 20.0f);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Stopping Distance determins the distance in which your AI will stop for its waypoint/destination/target. If your AI get too close to a target or your AI slides before waypoints, increase its stopping distance.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			StoppingRangeColorProp.colorValue = EditorGUILayout.ColorField("Stopping Range Color", StoppingRangeColorProp.colorValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The color of the Stopping Range Radius.", MessageType.None, true);
			}


			
			EditorGUILayout.Space();

			if (AggressionProp.intValue <= 1)
			{
				FleeRadiusProp.intValue = EditorGUILayout.IntSlider ("Flee Trigger Radius", FleeRadiusProp.intValue, 1, 100);

				EditorGUILayout.Space();

				FleeRadiusColorProp.colorValue = EditorGUILayout.ColorField("Flee Radius Color", FleeRadiusColorProp.colorValue);
			
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Flee Trigger Radius is the radius in which the AI will be triggered to flee.", MessageType.None, true);
				}

				EditorGUILayout.Space();


				editorFleeType = (FleeType)FleeTypeProp.intValue;
				editorFleeType = (FleeType)EditorGUILayout.EnumPopup("Flee Type", editorFleeType);
				FleeTypeProp.intValue = (int)editorFleeType;

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Flee Type determines how your AI will flee from danger.", MessageType.None, true);
				}

				if (FleeTypeProp.intValue == 0)
				{
					EditorGUILayout.HelpBox("When using Distance, your AI will continue to flee until the appropriate distance is met before they stop. There is not cool down for the Distance Flee Type.", MessageType.None, true);
				}

				if (FleeTypeProp.intValue == 1)
				{
					EditorGUILayout.HelpBox("When using Time, your AI will flee for the amount of Flee Seconds set below. Once it is met, your AI will stop fleeing until their Cool Down Seconds have been met.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				if (FleeTypeProp.intValue == 1)
				{
					ExtraFleeSecondsProp.intValue = EditorGUILayout.IntSlider ("Flee Seconds", ExtraFleeSecondsProp.intValue, 1, 120);
				}

				if (FleeTypeProp.intValue == 0)
				{
					MaxFleeDistanceProp.intValue = EditorGUILayout.IntSlider ("Max Flee Distance", MaxFleeDistanceProp.intValue, FleeRadiusProp.intValue+25, 150);
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Flee Seconds controls how many seconds an AI can flee before they are exhausted. Once exhausted, they will switch to cool down mode and will not be able to flee until their Cool Down Seconds below have been reached.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				RangedFleeAmountProp.intValue = EditorGUILayout.IntSlider ("Ranged Flee Distance", RangedFleeAmountProp.intValue, 10, 300);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Ranged Flee Distance controls how much additional distance your AI will flee if attacked from a ranged damage source outside of their flee radius. For example, if your AI is 100 units away, and it's damaged with a gun or bow, it will flee the Ranged Flee Distance amount from where its current position is.", MessageType.None, true);
				}

				if (FleeTypeProp.intValue == 1)
				{
					EditorGUILayout.Space();

					CoolDownSecondsProp.floatValue = EditorGUILayout.Slider ("Cool Down Seconds", CoolDownSecondsProp.floatValue, 0, 25);
					
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Cool Down Seconds controls how many seconds your animal will wait after they have reached their chase seconds. The animal will then return back to its statring position while in this mode.", MessageType.None, true);
					}
				}
			}

			if (AggressionProp.intValue == 3 || AggressionProp.intValue == 4)
			{

				EditorGUILayout.Space();

				if (AITypeProp.intValue == 0)
				{
					HuntRadiusProp.intValue = EditorGUILayout.IntSlider ("Hunt Trigger Radius", HuntRadiusProp.intValue, 1, 200);
				}

				if (AITypeProp.intValue == 1)
				{
					HuntRadiusProp.intValue = EditorGUILayout.IntSlider ("Attack Trigger Radius", HuntRadiusProp.intValue, 1, 200);
				}
				
				EditorGUILayout.Space();

				if (AITypeProp.intValue == 0)
				{
					HuntRadiusColorProp.colorValue = EditorGUILayout.ColorField("Hunt Radius Color", HuntRadiusColorProp.colorValue);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The Hunt Trigger Radius is the radius in which the AI will be triggered to hunt. This process is pause if the animal is within attacking distance.", MessageType.None, true);
					}
				}

				if (AITypeProp.intValue == 1)
				{
					HuntRadiusColorProp.colorValue = EditorGUILayout.ColorField("Attack Radius Color", HuntRadiusColorProp.colorValue);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The Attack Trigger Radius is the radius in which the AI will be triggered to attack. This process is pause if the NPC is within attacking distance.", MessageType.None, true);
					}
				}

				EditorGUILayout.Space();

				ChaseSecondsProp.intValue = EditorGUILayout.IntSlider ("Chase Seconds", (int)ChaseSecondsProp.intValue, 1, 120);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Chase Seconds determines how long an animal can flee before being exhausted and switching to cooldown mode.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				ReturnsToStartProp.boolValue = EditorGUILayout.Toggle ("Returns to Start?",ReturnsToStartProp.boolValue);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Returns to Start controls whether or not your AI will return to its starting position once they have given up on a target.", MessageType.None, true);
				}

				if (ReturnsToStartProp.boolValue)
				{
					EditorGUILayout.Space();

					ReturnBackToStartingPointProtectionProp.boolValue = EditorGUILayout.Toggle ("Return Protection?",ReturnBackToStartingPointProtectionProp.boolValue);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("If an NPC is ReturnProtected, they cannot be injured until they have returned back to their starting position.", MessageType.None, true);
					}
				}

				EditorGUILayout.Space();

				CoolDownSecondsProp.floatValue = EditorGUILayout.Slider ("Cool Down Seconds", CoolDownSecondsProp.floatValue, 0, 25);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Cool Down Seconds controls how many seconds your animal will wait after they have reached their max Hunt Seconds. During the Cool Down phase, the animal will return back to its statring position. The seconds for this are rest if an animal receives damage.", MessageType.None, true);
				}
			}

			EditorGUILayout.Space();

			WanderRangeProp.intValue = EditorGUILayout.IntSlider ("Wander Range", WanderRangeProp.intValue, 1, 500);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Wander Range controls the radius in which the animal will wander. It will not wander out of its Wander Range, unless it's fleeing.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			WanderRangeColorProp.colorValue = EditorGUILayout.ColorField("Wander Range Color", WanderRangeColorProp.colorValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Wander Range is a radius that the AI will wander in from its originally placed spot. It will not wander further than this radius.", MessageType.None, true);
			}

			if (AggressionProp.intValue == 1)
			{
				EditorGUILayout.Space();
				
				FreezeSecondsMinProp.floatValue = EditorGUILayout.Slider ("Min Freeze Seconds", (float)FreezeSecondsMinProp.floatValue, 0.25f, 3.0f);
				FreezeSecondsMaxProp.floatValue = EditorGUILayout.Slider ("Max Freeze Seconds", (float)FreezeSecondsMaxProp.floatValue, 0.5f, 8.0f);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("This controls the min and max seconds it takes for the animals to react to a predator or player triggering the animal to flee.", MessageType.None, true);
				}
			}

			EditorGUILayout.Space();

			if (AITypeProp.intValue == 0)
			{
				GrazeLengthMinProp.intValue = EditorGUILayout.IntSlider ("Graze Length Min", GrazeLengthMinProp.intValue, 1, 100);
				GrazeLengthMaxProp.intValue = EditorGUILayout.IntSlider ("Graze Length Max", GrazeLengthMaxProp.intValue, 1, 100);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Graze Lengths are generated with the min and max values entered above. This also plays a role in how often the waypoints are changed. If the AI is unable to reach its waypoint within its generated graze length time, a new waypoint will be generated.", MessageType.None, true);
				}
			}

			if (AITypeProp.intValue == 1)
			{
				GrazeLengthMinProp.intValue = EditorGUILayout.IntSlider ("Wait Length Min", GrazeLengthMinProp.intValue, 1, 100);
				GrazeLengthMaxProp.intValue = EditorGUILayout.IntSlider ("Wait Length Max", GrazeLengthMaxProp.intValue, 1, 100);
				
				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Wait Lengths are generated with the min and max values entered above. This also plays a role in how often the waypoints are changed. If the AI is unable to reach its waypoint within its generated wait length time, a new waypoint will be generated.", MessageType.None, true);
				}
			}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

		}


		if (TabNumberProp.intValue == 8 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Movement Options", EditorStyles.boldLabel);

			EditorGUILayout.Space();

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Here you can adjust the speeds that your AI will use. This AI system uses a NavMeshAgent, which is applied automatically. These speeds will change the NavMeshAgent's speed when a AI goes into flee mode.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			WalkSpeedProp.floatValue = EditorGUILayout.Slider ("Walk Speed", (float)WalkSpeedProp.floatValue, 0.1f, 10.0f);
			RunSpeedProp.floatValue = EditorGUILayout.Slider ("Run Speed", (float)RunSpeedProp.floatValue, 0.1f, 15.0f);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("These control how fast the AI will walk and run.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			WalkRotateSpeedProp.floatValue = EditorGUILayout.Slider ("Walk Rotate Speed", WalkRotateSpeedProp.floatValue, 0.5f, 10f);
			RunRotateSpeedProp.floatValue = EditorGUILayout.Slider ("Run Rotate Speed", RunRotateSpeedProp.floatValue, 0.5f, 10f);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Rotate Speeds determins how fast your AI will rotate to its waypoint/destination when running or walking.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			BaseOffsetNavProp.floatValue = EditorGUILayout.Slider ("Navmesh Base Offset", BaseOffsetNavProp.floatValue, -2f, 2f);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("The Navmesh Base Offset controls the gap distance between the ground and your AI. If you AI is hovering too far above the ground, setting a negative number will offset it so it'll be touching the ground.", MessageType.None, true);
			}

			//Removed
			/*
			if (AggressionProp.intValue == 1 || AggressionProp.intValue == 0)
			{
				EditorGUILayout.Space();

				MaximumWalkingVelocityProp.intValue = EditorGUILayout.IntSlider ("Maximum Walking Velocity", MaximumWalkingVelocityProp.intValue, 0, 75);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Maximum Walking Velocity controls when your AI will switch to the running animation. This only applies to fleeing AI.", MessageType.None, true);
				}
			}
			*/

				EditorGUILayout.Space();
				EditorGUILayout.Space();

		}

		if (TabNumberProp.intValue == 7 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Animation Options", EditorStyles.boldLabel);

			if (showHelpOptions == true)
			{
					EditorGUILayout.HelpBox("Here you can setup your AI's animations. You simple drag and drop animations you'd like to use below and the system will use them for the selected animations.", MessageType.None, true);
			}

			EditorGUILayout.Space();

			UseAnimationsProp.boolValue = EditorGUILayout.Toggle ("Use Animations?", UseAnimationsProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("If the AI does not use animations, animations can be disabled here. However, they will be disabled automatically if no animation component is found on the current model.", MessageType.None, true);
			}

			if (UseAnimationsProp.boolValue)
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();

				editorAnimationType = (AnimationType)AnimationTypeProp.intValue;
				editorAnimationType = (AnimationType)EditorGUILayout.EnumPopup("Animation Type", editorAnimationType);
				AnimationTypeProp.intValue = (int)editorAnimationType;

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Animation Type allows you to choose whether your AI will use Legacy animations or Mecanim animations.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();

				if (AnimationTypeProp.intValue == 2)
				{
					UseRootMotionProp.boolValue = EditorGUILayout.Toggle ("Use Root Motion?", UseRootMotionProp.boolValue);
					
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Use Root Motion enables or disables Root Motion on the AI's Animator Component.", MessageType.None, true);
					}

					EditorGUILayout.Space();

					UseHitAnimationProp.boolValue = EditorGUILayout.Toggle ("Use Hit Animation?",UseHitAnimationProp.boolValue);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The hit animation that is played when the AI receives damage.", MessageType.None, true);
					}

					if (UseHitAnimationProp.boolValue)
					{
						EditorGUILayout.HelpBox("You have selected to use the hit animation feature. You must apply your hit animation to the Hit State in the Animator. Not doing so will result in an error. You can edit animations by pressing the Edit Animator Controller button below.", MessageType.Warning, true);
					}

					EditorGUILayout.Space();
					EditorGUILayout.Space();
					EditorGUILayout.Space();

					if (AggressionProp.intValue >= 3)
					{
						UseRunAttackAnimationsProp.boolValue = EditorGUILayout.Toggle ("Use Run Attack Animations?",UseRunAttackAnimationsProp.boolValue);

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("If the AI has run attack animation, it can be set here. It will then be played when an AI is attacking a target while running.", MessageType.None, true);
						}

						if (UseRunAttackAnimationsProp.boolValue)
						{
							EditorGUILayout.HelpBox("You have selected to use the run attack animation feature. You must apply your run attack animation to the Run Attack State in the Animator. Not doing so will result in an error. You can edit animations by pressing the Edit Animator Controller button below.", MessageType.Warning, true);
						}

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						editorNumberOfAttackAnimations = (NumberOfAttackAnimations)TotalAttackAnimationsProp.intValue;
						editorNumberOfAttackAnimations = (NumberOfAttackAnimations)EditorGUILayout.EnumPopup("Total Attack Animations", editorNumberOfAttackAnimations);
						TotalAttackAnimationsProp.intValue = (int)editorNumberOfAttackAnimations;

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Total Attack Animations controls how many attack animations your AI will use for attacking. If using more than 1, Emerald will randomly play one of the attack animations. A max of 3 can be used.", MessageType.None, true);
						}

						EditorGUILayout.HelpBox("You must apply each attack animation you are using to each of the Attack States in the Animator. Not doing so will result in an error. You can edit animations by pressing the Edit Animator Controller button below.", MessageType.Warning, true);

						if (TotalAttackAnimationsProp.intValue == 1)
						{
							EditorGUILayout.HelpBox("You are currently using 1 attack animation, make sure you have assigned your attack animation to the Attack 1 State.", MessageType.Warning, true);
						}

						if (TotalAttackAnimationsProp.intValue == 2)
						{
							EditorGUILayout.HelpBox("You are currently using 2 attack animations, make sure you have assigned your attack animations to the Attack 1 and Attack 2 States.", MessageType.Warning, true);
						}

						if (TotalAttackAnimationsProp.intValue == 3)
						{
							EditorGUILayout.HelpBox("You are currently using 3 attack animations, make sure you have assigned your attack animations to the Attack 1, Attack 2, and Attack 3 States.", MessageType.Warning, true);
						}

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					}

					editorNumberOfGrazeAnimations = (NumberOfGrazeAnimations)TotalGrazeAnimationsProp.intValue;
					editorNumberOfGrazeAnimations = (NumberOfGrazeAnimations)EditorGUILayout.EnumPopup("Total Graze Animations", editorNumberOfGrazeAnimations);
					TotalGrazeAnimationsProp.intValue = (int)editorNumberOfGrazeAnimations;

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("The Total Graze Animations controls how many graze animations your AI will use while grazing. If using more than 1, Emerald will randomly play one of the graze animations. A max of 3 can be used.", MessageType.None, true);
					}

					EditorGUILayout.HelpBox("You must apply each graze animation you are using to each of the Graze States in the Animator. Not doing so will result in an error. You can edit animations by pressing the Edit Animator Controller button below.", MessageType.Warning, true);

					if (TotalGrazeAnimationsProp.intValue == 1)
					{
						EditorGUILayout.HelpBox("You are currently using 1 graze animation, make sure you have assigned your graze animation to the Graze 1 State.", MessageType.Warning, true);
					}

					if (TotalGrazeAnimationsProp.intValue == 2)
					{
						EditorGUILayout.HelpBox("You are currently using 2 graze animations, make sure you have assigned your graze animations to the Graze 1 and Graze 2 States.", MessageType.Warning, true);
					}

					if (TotalGrazeAnimationsProp.intValue == 3)
					{
						EditorGUILayout.HelpBox("You are currently using 3 graze animations, make sure you have assigned your graze animations to the Graze 1, Graze 2, and Graze 3 States.", MessageType.Warning, true);
					}

					EditorGUILayout.Space();
					EditorGUILayout.Space();
				}
			}

			//Mecanim
			if (UseAnimationsProp.boolValue && AnimationTypeProp.intValue == 2)
			{
				EditorGUILayout.LabelField("Mecanim Setup", EditorStyles.boldLabel);

				EditorGUILayout.HelpBox("If you have not already done so, you will need to create an Animator Controller using the button below. This will create an Animator Controller, create all necessary parameters and states, and assign it to your AI's Animator automatically. This is required to work with Emerald. Animations and blending can be assigned using the Edit Animator Controller button.", MessageType.None, true);
				EditorGUILayout.HelpBox("Note: Do not move or delete the Emerald Controller Animator object (located under the Animator folder of Emerald). This is used as a reference for creating Animators for Emerald.", MessageType.Warning, true);


				string FilePath = "Assets/Emerald AI/Animator/" + self.gameObject.name + " (Emerald)" + ".controller";

				EditorGUILayout.Space();

				self.AnimatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField ("Animator Controller", self.AnimatorController, typeof(RuntimeAnimatorController), true);

				if (self.AnimatorController == null)
				{
					EditorGUILayout.HelpBox("You will need to create an animator controller using the button below in order for this model to work with Emerald and Mecanim.", MessageType.Info, true);
				}

				EditorGUILayout.Space();

				EditorGUI.BeginDisabledGroup (self.AnimatorController == true);

				if(GUILayout.Button("Create Animator Controller"))
				{
					if (!File.Exists(FilePath))
					{
						AssetDatabase.CopyAsset("Assets/Emerald AI/Animator/Emerald Controller.controller", FilePath);
						AssetDatabase.Refresh();

						RuntimeAnimatorController CreatedController = AssetDatabase.LoadAssetAtPath(FilePath, (typeof(RuntimeAnimatorController))) as RuntimeAnimatorController;

						if (self.gameObject.GetComponent<Animator>() != null)
						{
							Animator animator = self.gameObject.GetComponent<Animator>();
							animator.runtimeAnimatorController = CreatedController as RuntimeAnimatorController;
							self.AnimatorController = CreatedController as RuntimeAnimatorController;
						}

						if (self.gameObject.GetComponent<Animator>() == null)
						{
							self.gameObject.AddComponent<Animator>();
							Animator animator = self.gameObject.GetComponent<Animator>();
							animator.runtimeAnimatorController = CreatedController as RuntimeAnimatorController;
							self.AnimatorController = CreatedController as RuntimeAnimatorController;
						}
					}

					else
					{
						EditorUtility.DisplayDialog("", "An Animator Controller already exists for this model. Please delete it to create another Animator Controller for this model.", "Okay");
					}
				}

				EditorGUI.EndDisabledGroup ();

				if (self.AnimatorController != null)
				{
					EditorGUILayout.HelpBox("In order to create a new animator controller for this model, you will need to remove or delete the one currently in the Animator Controller slot.", MessageType.Info, true);
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				if (self.AnimatorController != null)
				{
					EditorGUILayout.HelpBox("Animations will need to be assigned using the button below. This is done using the Animator Window. Assign the animations to each logical state using the animations from your model's animations (Idle with 'Idle', Walk with 'Walk', etc)", MessageType.None, true);

					if(GUILayout.Button("Edit Animator Controller"))
					{
						EditorApplication.ExecuteMenuItem("Window/Animator");
					}
				}
			}


			if (UseAnimationsProp.boolValue && AnimationTypeProp.intValue == 1)
			{

				EditorGUILayout.Space();

				IdleAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Idle Animation", IdleAnimationProp.objectReferenceValue, typeof(AnimationClip), true);

				if (AggressionProp.intValue >= 3)
				{
					EditorGUILayout.Space();

					//bool idleBattleAnimation  = !EditorUtility.IsPersistent (self);
					IdleBattleAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Idle (Aggressive) Animation", IdleBattleAnimationProp.objectReferenceValue, typeof(AnimationClip), true);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("If an AI doesn't have an aggressive idle animation, you can just use your basic idle animation.", MessageType.None, true);
					}
				}

				EditorGUILayout.Space();

				TotalGrazeAnimationsProp.intValue = EditorGUILayout.IntSlider ("Total Graze Animations", TotalGrazeAnimationsProp.intValue, 1, 3);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Total Graze Animations determins how many graze animations your AI will use when wandering. These animations will be picked at random. These can also be Idle animations, if desired. There is a max of 3.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				if (AITypeProp.intValue == 0)
				{
					Graze1AnimationProp.objectReferenceValue = EditorGUILayout.ObjectField ("Graze 1 Animation", Graze1AnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					//EditorGUILayout.PropertyField(Graze1AnimationProp);

					if (TotalGrazeAnimationsProp.intValue == 2 || TotalGrazeAnimationsProp.intValue == 3)
					{
						//bool graze2Animation  = !EditorUtility.IsPersistent (self);
						Graze2AnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Graze 2 Animation", Graze2AnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					}
					
					if (TotalGrazeAnimationsProp.intValue == 3)
					{
						//bool graze3Animation  = !EditorUtility.IsPersistent (self);
						Graze3AnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Graze 3 Animation", Graze3AnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					}
				}

				if (AITypeProp.intValue == 1)
				{
					//bool graze1Animation  = !EditorUtility.IsPersistent (self);
					Graze1AnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Wait 1 Animation", Graze1AnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					
					if (TotalGrazeAnimationsProp.intValue == 2 || TotalGrazeAnimationsProp.intValue == 3)
					{
						//bool graze2Animation  = !EditorUtility.IsPersistent (self);
						Graze2AnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Wait 2 Animation", Graze2AnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					}
					
					if (TotalGrazeAnimationsProp.intValue == 3)
					{
						//bool graze3Animation  = !EditorUtility.IsPersistent (self);
						Graze3AnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Wait 3 Animation", Graze3AnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					}
				}

				EditorGUILayout.Space();

				//bool walkAnimation  = !EditorUtility.IsPersistent (self);
				WalkAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Walk Animation", WalkAnimationProp.objectReferenceValue, typeof(AnimationClip), true);

				//bool runAnimation  = !EditorUtility.IsPersistent (self);
				RunAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Run Animation", RunAnimationProp.objectReferenceValue, typeof(AnimationClip), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The walk and run animation your AI will use.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				
				UseTurnAnimationProp.boolValue = EditorGUILayout.Toggle ("Use Turn Animation?",UseTurnAnimationProp.boolValue);

				EditorGUILayout.Space();

				if (UseTurnAnimationProp.boolValue)
				{
					//bool turnAnimation  = !EditorUtility.IsPersistent (self);
					TurnAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Turn Animation", TurnAnimationProp.objectReferenceValue, typeof(AnimationClip), true);
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Trun Animation will play a turn animation when your AI is turning more than a percalculated degree. This applies to wandering, fighting, and fleeing. This animation helps with making AI function more realistic. If your AI doesn't have a turn animation, a walk animation can be used in its place. However, this feature is completely optional.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				UseHitAnimationProp.boolValue = EditorGUILayout.Toggle ("Use Hit Animation?",UseHitAnimationProp.boolValue);

				if (UseHitAnimationProp.boolValue)
				{
					HitAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Hit Animation", HitAnimationProp.objectReferenceValue, typeof(AnimationClip), true);
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The hit animation that is played when the AI receives damage.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				if (AggressionProp.intValue >= 3)
				{
					UseRunAttackAnimationsProp.boolValue = EditorGUILayout.Toggle ("Use Run Attack Animations?",UseRunAttackAnimationsProp.boolValue);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("If the AI has run attack animation, it can be set here. It will then be played when an AI is attacking a target while running.", MessageType.None, true);
					}
					
					EditorGUILayout.Space();

					if (UseRunAttackAnimationsProp.boolValue)
					{
						//bool runAttackAnimation  = !EditorUtility.IsPersistent (self);
						RunAttackAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Run Attack Animation", RunAttackAnimationProp.objectReferenceValue, typeof(AnimationClip), true);
					}
				}


				if (AggressionProp.intValue >= 3)
				{
					EditorGUILayout.Space();

					TotalAttackAnimationsProp.intValue = EditorGUILayout.IntSlider ("Total Attack Animations", TotalAttackAnimationsProp.intValue, 1, 6);

					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Here you control how many attack animations your AI uses. There are a max of 6. Each will be used randomly when the AI is attacking. If your AI doesn't have an attack animation, another animation can be used in its place such as idle, walk, etc.", MessageType.None, true);
					}

					EditorGUILayout.Space();

					//bool attackAnimation1  = !EditorUtility.IsPersistent (self);
					AttackAnimation1Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 1", AttackAnimation1Prop.objectReferenceValue, typeof(AnimationClip), true);

					if (TotalAttackAnimationsProp.intValue == 2)
					{
						//bool attackAnimation2  = !EditorUtility.IsPersistent (self);
						AttackAnimation2Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 2", AttackAnimation2Prop.objectReferenceValue, typeof(AnimationClip), true);
					}

					if (TotalAttackAnimationsProp.intValue == 3)
					{
						//bool attackAnimation2  = !EditorUtility.IsPersistent (self);
						AttackAnimation2Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 2", AttackAnimation2Prop.objectReferenceValue, typeof(AnimationClip), true);

						//bool attackAnimation3  = !EditorUtility.IsPersistent (self);
						AttackAnimation3Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 3", AttackAnimation3Prop.objectReferenceValue, typeof(AnimationClip), true);
					}

					if (TotalAttackAnimationsProp.intValue == 4)
					{
						//bool attackAnimation2  = !EditorUtility.IsPersistent (self);
						AttackAnimation2Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 2", AttackAnimation2Prop.objectReferenceValue, typeof(AnimationClip), true);

						//bool attackAnimation3  = !EditorUtility.IsPersistent (self);
						AttackAnimation3Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 3", AttackAnimation3Prop.objectReferenceValue, typeof(AnimationClip), true);

						//bool attackAnimation4  = !EditorUtility.IsPersistent (self);
						AttackAnimation4Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 4", AttackAnimation4Prop.objectReferenceValue, typeof(AnimationClip), true);
					}

					if (TotalAttackAnimationsProp.intValue == 5)
					{
						//bool attackAnimation2  = !EditorUtility.IsPersistent (self);
						AttackAnimation2Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 2", AttackAnimation2Prop.objectReferenceValue, typeof(AnimationClip), true);
						
						//bool attackAnimation3  = !EditorUtility.IsPersistent (self);
						AttackAnimation3Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 3", AttackAnimation3Prop.objectReferenceValue, typeof(AnimationClip), true);

						//bool attackAnimation4  = !EditorUtility.IsPersistent (self);
						AttackAnimation4Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 4", AttackAnimation4Prop.objectReferenceValue, typeof(AnimationClip), true);

						//bool attackAnimation5  = !EditorUtility.IsPersistent (self);
						AttackAnimation5Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 5", AttackAnimation5Prop.objectReferenceValue, typeof(AnimationClip), true);
					}

					if (TotalAttackAnimationsProp.intValue == 6)
					{
						//bool attackAnimation2  = !EditorUtility.IsPersistent (self);
						AttackAnimation2Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 2", AttackAnimation2Prop.objectReferenceValue, typeof(AnimationClip), true);
						
						//bool attackAnimation3  = !EditorUtility.IsPersistent (self);
						AttackAnimation3Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 3", AttackAnimation3Prop.objectReferenceValue, typeof(AnimationClip), true);
						
						//bool attackAnimation4  = !EditorUtility.IsPersistent (self);
						AttackAnimation4Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 4", AttackAnimation4Prop.objectReferenceValue, typeof(AnimationClip), true);
						
						//bool attackAnimation5  = !EditorUtility.IsPersistent (self);
						AttackAnimation5Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 5", AttackAnimation5Prop.objectReferenceValue, typeof(AnimationClip), true);

						//bool attackAnimation6  = !EditorUtility.IsPersistent (self);
						AttackAnimation6Prop.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Attack Animation 6", AttackAnimation6Prop.objectReferenceValue, typeof(AnimationClip), true);
					}

					EditorGUILayout.Space();
				}


				if (UseDeadReplacementProp.boolValue)
				{
					EditorGUILayout.HelpBox("You have Use Dead Replacement enabled so the death animation will not be used. To disable Use Dead Replacement, go to the Health Options.", MessageType.Info, true);
				}


				DeathAnimationProp.objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField ("Death Animation", DeathAnimationProp.objectReferenceValue, typeof(AnimationClip), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Death Animation is the animation that plays when your AI's health reaches 0.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				WalkAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Walk Animation Speed", (float)WalkAnimationSpeedProp.floatValue, 0.1f, 5.0f);
				RunAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Run Animation Speed", (float)RunAnimationSpeedProp.floatValue, 0.1f, 5.0f);
				IdleAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Idle Animation Speed", (float)IdleAnimationSpeedProp.floatValue, 0.1f, 5.0f);
				GrazeAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Graze Animation Speed", (float)GrazeAnimationSpeedProp.floatValue, 0.1f, 5.0f);

				if (!UseDeadReplacementProp.boolValue)
				{
					DieAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Die Animation Speed", (float)DieAnimationSpeedProp.floatValue, 0.1f, 5.0f);
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("These control your AI's animations speed. Use these are useful to help your animations match your movement speed as well as slow down animations that may be playing too fast.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				if (AggressionProp.intValue >= 3)
				{
					IdleCombatAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Idle Combat Animation Speed", (float)IdleCombatAnimationSpeedProp.floatValue, 0.1f, 5.0f);
					AttackAnimationSpeedProp.floatValue = EditorGUILayout.Slider ("Attack Animation Speed", (float)AttackAnimationSpeedProp.floatValue, 0.1f, 5.0f);
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("These control your AI's Idle combat and attack animation speeds. The Attack Animation Speed applies to all attack animations.", MessageType.None, true);
				}

			}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

		}


		if (TabNumberProp.intValue == 9 || TabNumberProp.intValue == 12) 
		{
			if (AggressionProp.intValue == 2)
			{
				EditorGUILayout.LabelField("Tag Options", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox("The Tag Options for passive animals is a little different. This is because they have the ability to use the breeding feature. In order for an AI to breed, they must have a player present, have matching Animal Type Names (located in the Breeding Options), and have been initialized with food based off of their Food Tags.", MessageType.None, true);

				EditorGUILayout.HelpBox("In order for Emerald to work correctly, your AI will need to have a tag other than the standard Untagged.", MessageType.None, true);
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField("Emerald System Tag");
				self.EmeraldObjectsTag = EditorGUILayout.TagField(self.EmeraldObjectsTag);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.HelpBox("The Emerald Tag is the tag that all Emerald AI objects will use. This is to help define what objects are Emerald objects. This can be any tag, but all Emerald AI objects must use the same Unity tag.", MessageType.None, true);


				EditorGUILayout.Space();

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Food Tags are used to help initiate breeding. For example, some animals can be initiated with Corn and Grass, while others can only be initiated with Apples.", MessageType.None, true);
				}

				if(self.EmeraldTags != null)
				{
					if(self.EmeraldTags.Count > 0)
					{
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();

						for (int j = 0; j < self.EmeraldTags.Count; ++j)
						{
							EditorGUILayout.BeginHorizontal ();
							self.EmeraldTags[j] = EditorGUILayout.TextField("Food Tag " + (j + 1), self.EmeraldTags[j]);
							EditorGUILayout.EndHorizontal ();

							GUILayout.BeginHorizontal();
							GUILayout.Space(75);
							if (GUILayout.Button("Remove Tag"))
							{
								self.EmeraldTags.RemoveAt(j);
								--j;
							}
							GUILayout.Space(75);
							GUILayout.EndHorizontal();

							EditorGUILayout.Space ();
							EditorGUILayout.Space ();
						}
					}

					if(self.EmeraldTags.Count == 0)
					{
						EditorGUILayout.HelpBox("There are currently no tags for this AI.", MessageType.Info);
					}

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Add Tag", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
					{
						self.EmeraldTags.Add("Tag");
					}
					GUILayout.EndHorizontal();
				}
			}

			if (AggressionProp.intValue != 2)
			//if (AITypeProp.intValue == 1 && AggressionProp.intValue == 3 || AITypeProp.intValue == 1 && AggressionProp.intValue == 4)
			{
				EditorGUILayout.LabelField("Tag Options", EditorStyles.boldLabel);

				EditorGUILayout.HelpBox("In order for Emerald to work correctly, your AI will need to have a tag other than the standard Untagged. Using the Emerald Tag tab below, choose the tag that all of your Emerald AI objects will be using. This is important for Emerald so that it only detects relevant targets.", MessageType.None, true);
				EditorGUILayout.Space();

				/*
				if (AggressionProp.intValue == 4)
				{
					EditorGUILayout.HelpBox("A Defensive AI will only attack things on sight that have the above Enemy Tag. However, if they are attacked first by an unknown tag or AI, they will defend themselves by attacking and killing the attacker.", MessageType.Info, true);
				}
				*/

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();


				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField("Emerald System Tag");
				//self.AITag = EditorGUILayout.TextField(self.AITag);
				self.EmeraldObjectsTag = EditorGUILayout.TagField(self.EmeraldObjectsTag);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.HelpBox("The Emerald Tag is the tag that all Emerald AI objects will use. This is to help define what objects are Emerald objects. This can be any tag, but all Emerald AI objects must use the same Unity tag.", MessageType.None, true);

				if (AggressionProp.intValue >= 3)
				{
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				PlayerUsesSeparateLayerProp.boolValue = EditorGUILayout.Toggle ("Player uses Separate Layer", PlayerUsesSeparateLayerProp.boolValue);

				EditorGUILayout.HelpBox("Player uses Separate Layer is for character controller systems that have their player on a separate layer. If true, you will be able to set your player's layer.", MessageType.None, true);
				EditorGUILayout.HelpBox("Note: If your player has a layer other than the Emerald Layer, you will need to set 'Player uses Separate Layer' to true in order for Emerald AI objects to see your player.", MessageType.Warning, true);

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				self.EmeraldLayerMask = EditorGUILayout.LayerField("Emerald Layer", self.EmeraldLayerMask);

				if (PlayerUsesSeparateLayerProp.boolValue)
				{
					self.PlayerLayerMask = EditorGUILayout.LayerField("Player Layer", self.PlayerLayerMask);
				}

				EditorGUILayout.HelpBox("The Emerald Layer helps with performance while AI are searching for potenial targets by only looking for objects with the Emerald Layer. All Emerald AI objects should use the same layer.", MessageType.None, true);

				if (!PlayerUsesSeparateLayerProp.boolValue)
				{
					EditorGUILayout.HelpBox("Your player object must also have the Emerald Layer, unless the camera/character system you are using requires a different layer such as Player.", MessageType.Info, true); 
				}

				if (PlayerUsesSeparateLayerProp.boolValue)
				{
					EditorGUILayout.HelpBox("You have enabled Player uses Separate Layer, ensure that your player's layer is set to your player's layer on your player game object.", MessageType.Warning, true); 
				}
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField("AI Tag");
				self.AITag = EditorGUILayout.TextField(self.AITag);
				//self.AITag = EditorGUILayout.TagField(self.AITag);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.HelpBox("The AI Tag is a customized tagged created with Emerald for this specific AI. This tag doesn't use Unity's tag system, but Emerald's. This allows AI not to be dependent on Unity tags giving more freedom and flexiblity for 3rd party systems and custom sciprts.", MessageType.None, true);

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				if (AggressionProp.intValue < 2)
				{
					EditorGUILayout.LabelField("Flee Tags");
					EditorGUILayout.HelpBox("Flee Tags are the tags that will make this AI flee. This tag is based on the 'AI Tag' of an AI.", MessageType.Info);
				}

				if (AggressionProp.intValue >= 3)
				{
					EditorGUILayout.LabelField("Target Tags");
					EditorGUILayout.HelpBox("Target Tags are the tags that will make this attack another AI or player. This tag is based on the 'AI Tag' of an AI.", MessageType.Info);
				}

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Below, you can add multiple tags for this AI to detect. It will look for other AI's 'AI Tag'. Each tag below will be considered a possible target, if within range. The tags below don't use Unity's tag system, but a custom one designed with Emerald. This is to make Emerald more flexible and eliminate having to rely on certain tags.", MessageType.None, true);
				}

				//EditorGUILayout.HelpBox("Note: The Tags below should match the 'AI Tag' of the AI you'd like to be attacked.", MessageType.Info, true);


				if(self.EmeraldTags != null)
				{
					if(self.EmeraldTags.Count > 0)
					{
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();

						for (int j = 0; j < self.EmeraldTags.Count; ++j)
						{
							EditorGUILayout.BeginHorizontal ();
							self.EmeraldTags[j] = EditorGUILayout.TextField("Tag " + (j + 1), self.EmeraldTags[j]);
							EditorGUILayout.EndHorizontal ();

							GUILayout.BeginHorizontal();
							GUILayout.Space(75);
							if (GUILayout.Button("Remove Tag"))
							{
								self.EmeraldTags.RemoveAt(j);
								--j;
							}
							GUILayout.Space(75);
							GUILayout.EndHorizontal();

							EditorGUILayout.Space ();
							EditorGUILayout.Space ();
						}
					}

					if(self.EmeraldTags.Count == 0)
					{
						EditorGUILayout.HelpBox("There are currently no tags for this AI.", MessageType.Info);
					}

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Add Tag", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
					{
						self.EmeraldTags.Add("Tag");
					}
					GUILayout.EndHorizontal();
				}
			}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

		}


		if (TabNumberProp.intValue == 10 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Effect Options", EditorStyles.boldLabel);

			EditorGUILayout.Space();

			UseBloodProp.boolValue = EditorGUILayout.Toggle ("Use Blood Effect?",UseBloodProp.boolValue);
			
			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Use Blood Effect determins if your player uses a blood effect when hit.", MessageType.None, true);
			}
			
			if (UseBloodProp.boolValue)
			{
				EditorGUILayout.Space();
				
				//bool bloodEffect = !EditorUtility.IsPersistent (self);
				BloodEffectProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Blood Effect", BloodEffectProp.objectReferenceValue, typeof(GameObject), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The particle effect that is used to play a blood effect when an AI is hit.", MessageType.None, true);
				}
			}


			EditorGUILayout.Space();

			UseDustEffectProp.boolValue = EditorGUILayout.Toggle ("Use Dust Effect?",UseDustEffectProp.boolValue);

			if (showHelpOptions == true)
			{
				EditorGUILayout.HelpBox("Use Dust Effect determins if your AI uses a dust effect when running.", MessageType.None, true);
			}

			if (UseDustEffectProp.boolValue)
			{
				EditorGUILayout.Space();

				//bool dustEffect = !EditorUtility.IsPersistent (self);
				DustEffectProp.objectReferenceValue = (ParticleSystem)EditorGUILayout.ObjectField ("Dust Effect", DustEffectProp.objectReferenceValue, typeof(ParticleSystem), true);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The particle effect that is used when an AI is running. The particle effect will only play when an AI is using its running animation. Its effects are controlled automatically by Emerald. It is recommended that the prefab's Emission Rate is set 0.", MessageType.None, true);
				}
			}


			EditorGUILayout.Space();
			EditorGUILayout.Space();

		}

		if (TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.LabelField("Editor Options", EditorStyles.boldLabel);
			string showOrHide = "Show";
			if(showHelpOptions)
				showOrHide = "Hide";
			if(GUILayout.Button(showOrHide+ " Help Boxes", GUILayout.Width(thirdOfScreen*2), GUILayout.Height(sizeOfHideButtons)) )
			{
				showHelpOptions = !showHelpOptions;
			}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

		}

		if (TabNumberProp.intValue == 11 || TabNumberProp.intValue == 12) 
		{
			//temp.Update ();
			//tabTemp.Update ();
			EditorGUILayout.LabelField("Breeding Options", EditorStyles.boldLabel);
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (AggressionProp.intValue != 2 || AITypeProp.intValue == 1)
			{
				EditorGUILayout.HelpBox("The Breeding Options are only for the Animal AIType with a Passive Behavior Type. This will be availble to all Animals and NPCs with Emerald 1.4." , MessageType.Info, true);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}
			
			if (AggressionProp.intValue == 2 && AITypeProp.intValue == 0)
			{
				editorUseBreeding = (UseBreeding)UseBreedingProp.intValue;
				editorUseBreeding = (UseBreeding)EditorGUILayout.EnumPopup("Use Breeding?", editorUseBreeding);
				UseBreedingProp.intValue = (int)editorUseBreeding;

				EditorGUILayout.Space();

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("Use Breeding controls whether or not this animal will use the breeding feature. Animals first need to be triggered before they can breed. Note: The demo scene Animal Breeding Demo demonstrates this and includes a demonstration script.", MessageType.None, true);
				}

				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Animal Type Name", EditorStyles.miniLabel);
				AnimalTypeProp.stringValue = EditorGUILayout.TextField(AnimalTypeProp.stringValue);

				if (showHelpOptions == true)
				{
					EditorGUILayout.HelpBox("The Animal Type is used to help with breeding. Only animals with the same Animal Type Name may breed together.", MessageType.None, true);
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();

				if (UseBreedingProp.intValue == 1)
				{
					IsBabyProp.boolValue = EditorGUILayout.Toggle ("Is Baby?", IsBabyProp.boolValue);
					
					if (showHelpOptions == true)
					{
						EditorGUILayout.HelpBox("Is Baby determines whether or not your animal is a baby. If you are using the Breeding System, your prefabed babies must have Is Baby checked.", MessageType.None, true);
					}

					if (!IsBabyProp.boolValue)
					{
						EditorGUILayout.Space();

						BreedSecondsProp.floatValue = EditorGUILayout.Slider("Breed Seconds", (float)BreedSecondsProp.floatValue, 1f, 100f);

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Breed Seconds determines how many seconds must pass before the baby is spawned.", MessageType.None, true);
						}

						EditorGUILayout.Space();

						BreedCoolDownSecondsProp.floatValue = EditorGUILayout.FloatField("Breed Cool Down Seconds", (float)BreedCoolDownSecondsProp.floatValue);

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("Breed Cool Down Seconds determines how many seconds must pass before your animal can breed again.", MessageType.None, true);
						}
					}

					if (IsBabyProp.boolValue)
					{
						EditorGUILayout.Space();
						
						BabySecondsProp.floatValue = EditorGUILayout.FloatField("Baby Seconds", (float)BabySecondsProp.floatValue);
						
						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Baby Seconds determines how many seconds your baby will stay a baby. Once this amount is exceeded, it will turn into a full grown animal. This full grown animal is based off of your full grown prefab.", MessageType.None, true);
						}

						EditorGUILayout.Space();

						FullGrownPrefabProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Full Grown Prefab", FullGrownPrefabProp.objectReferenceValue, typeof(GameObject), true);

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Full Grown Prefab is the prefab object your baby will turn into after it has exceeded its Baby Seconds.", MessageType.None, true);
						}

						if (FullGrownPrefabProp.objectReferenceValue == null)
						{
							EditorGUILayout.HelpBox("Your Animal is marked as a baby, but there is no Full Grown Prefab. Please apply a prefab to the Full Grown Prefab slot.", MessageType.Warning, true);
						}

						EditorGUILayout.Space();
					}

					if (!IsBabyProp.boolValue)
					{
						EditorGUILayout.Space();

						UseBreedEffectProp.boolValue = EditorGUILayout.Toggle ("Use Breed Effect?", UseBreedEffectProp.boolValue);

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("Use Breed Effect determines whether or not your animal will use the Breed Effect option.", MessageType.None, true);
						}

						if (UseBreedEffectProp.boolValue)
						{
							EditorGUILayout.Space();
							
							BreedEffectProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Breed Effect", BreedEffectProp.objectReferenceValue , typeof(GameObject), true);
							
							if (showHelpOptions == true)
							{
								EditorGUILayout.HelpBox("The Breed Effect is the effect that is spawned indicating that the two animals are in Breed Mode.", MessageType.None, true);
							}

							EditorGUILayout.Space();

							BreedEffectOffSetProp.vector3Value = EditorGUILayout.Vector3Field ("Breed Effect Offset", BreedEffectOffSetProp.vector3Value);

							if (showHelpOptions == true)
							{
								EditorGUILayout.HelpBox("The Breed Effect Offset allows you to adjust the spawning position of the Breed Effect to help match your model.", MessageType.None, true);
							}
						}

						EditorGUILayout.Space();
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("In order for a Baby Prefab to work properly, you will need need to have Is Baby set to true on the prefab object. This is located at the top of the Breeding Options. Your baby will then grow into a full grown animal when its Baby Seconds have been exceeded. Emerlad caluclates the sliders so they don't exceed 100%. When you've calculated your odds, ensure the sum of all your odds are equal to 100% for the most accurate results.", MessageType.Info, true);
						}

						EditorGUILayout.Space();

						GUI.backgroundColor = Color.green;
						BabyPrefabCommonProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Common Baby Prefab", BabyPrefabCommonProp.objectReferenceValue, typeof(GameObject), true);

						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Common Baby Prefab is the prefab that will spawn when a Common baby is created between two animals. There is a " + CommonOddsProp.intValue + "% chance of receiving a Common baby." , MessageType.None, true);
						}

						EditorGUILayout.Space();

						CommonOddsProp.intValue = EditorGUILayout.IntSlider("Common Odds", CommonOddsProp.intValue, 0, 100);

						EditorGUILayout.Space();

						GUI.backgroundColor = Color.blue + Color.grey;
						EditorGUILayout.Space();

						BabyPrefabUncommonProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Uncommon Baby Prefab", BabyPrefabUncommonProp.objectReferenceValue, typeof(GameObject), true);
						
						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Uncommon Baby Prefab is the prefab that will spawn when a Uncommon baby is created between two animals. There is a " + UncommonOddsProp.intValue + "% chance of receiving an Uncommon baby." , MessageType.None, true);
						}


						EditorGUILayout.Space();

						UncommonOddsProp.intValue = EditorGUILayout.IntSlider("Uncommon Odds", UncommonOddsProp.intValue, 0, 100 - (CommonOddsProp.intValue));

						EditorGUILayout.Space();

						GUI.backgroundColor = Color.red;
						BabyPrefabRareProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Rare Baby Prefab", BabyPrefabRareProp.objectReferenceValue, typeof(GameObject), true);
						
						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Rare Baby Prefab is the prefab that will spawn when a Rare baby is created between two animals. There is a " + RareOddsProp.intValue + "% chance of receiving a Rare baby." , MessageType.None, true);
						}

						EditorGUILayout.Space();

						RareOddsProp.intValue = EditorGUILayout.IntSlider("Rare Odds", RareOddsProp.intValue, 0, 100 - (CommonOddsProp.intValue + UncommonOddsProp.intValue));

						EditorGUILayout.Space();

						GUI.backgroundColor = Color.yellow;
						BabyPrefabSuperRareProp.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField ("Super Rare Baby Prefab", BabyPrefabSuperRareProp.objectReferenceValue, typeof(GameObject), true);
						
						if (showHelpOptions == true)
						{
							EditorGUILayout.HelpBox("The Super Rare Baby Prefab is the prefab that will spawn when a Super Rare baby is created between two animals. There is a " + SuperRareOddsProp.intValue + "% chance of receiving a Super Rare baby." , MessageType.None, true);
						}

						SuperRareOddsProp.intValue = EditorGUILayout.IntSlider("Super Rare Odds", SuperRareOddsProp.intValue, 0 , 100 - (CommonOddsProp.intValue + UncommonOddsProp.intValue + RareOddsProp.intValue));
					}
				}
			}
		}

		if (GUI.changed) 
		{ 
			EditorUtility.SetDirty(self); 
			tabTemp.ApplyModifiedProperties ();
			temp.ApplyModifiedProperties ();
		}

			
		if (GUI.changed && !EditorApplication.isPlaying) 
		{
			#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			#else
			EditorApplication.MarkSceneDirty();
			#endif

		}
	}

	void OnSceneGUI () 
	{
		Emerald_Animal_AI self = (Emerald_Animal_AI)target;

		//Cowardly
		if (AggressionProp.intValue == 1 && UseVisualRadiusProp.boolValue || AggressionProp.intValue == 0 && UseVisualRadiusProp.boolValue)
		{
			Handles.color = self.fleeRadiusColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.fleeRadius * self.transform.localScale.x);

			Handles.color = self.wanderRangeColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.wanderRange);

			//Stop new
			Handles.color = self.stoppingRangeColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.stoppingDistance);
		}

		//Passive
		if (AggressionProp.intValue == 2 && UseVisualRadiusProp.boolValue)
		{
			Handles.color = self.wanderRangeColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.wanderRange);

			//Stop new
			Handles.color = self.stoppingRangeColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.stoppingDistance);
		}

		//Aggresive
		if (AggressionProp.intValue == 3 && UseVisualRadiusProp.boolValue || AggressionProp.intValue == 4 && UseVisualRadiusProp.boolValue)
		{
			Handles.color = self.huntRadiusColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.huntRadius * self.transform.localScale.x);
			
			Handles.color = self.wanderRangeColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.wanderRange);

			//Stop new
			Handles.color = self.stoppingRangeColor;
			Handles.DrawSolidDisc(self.transform.position, Vector3.up, self.stoppingDistance);
		}

		SceneView.RepaintAll();
	}
	
}


