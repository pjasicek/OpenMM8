using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(PlayerWeapon))] 
public class PlayerWeaponEditor : Editor 
{
	public override void OnInspectorGUI () 
	{
		PlayerWeapon self = (PlayerWeapon)target;

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Timing Options", EditorStyles.boldLabel);

		self.attackDelay = EditorGUILayout.Slider ("Attack Delay", self.attackDelay, 0, 5);
		
		EditorGUILayout.HelpBox("The Attack Delay controls the delay for a registered hit. If set to 0.5, there will be a half a second delay before the hit registers. This is used to help animations match the hit registration.", MessageType.None, true);
		
		EditorGUILayout.Space();

		self.attackTime = EditorGUILayout.Slider ("Attack Time", self.attackTime, 0, 5);
		
		EditorGUILayout.HelpBox("The Attack Time controls how often your player can attack. For example, if set to 1, your player can only trigger a hit once per 1 second. This is used to help animations match the hit registration.", MessageType.None, true);
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();




		EditorGUILayout.LabelField("Damage Options", EditorStyles.boldLabel);

		self.attackDistance = EditorGUILayout.Slider ("Attack Distance", self.attackDistance, 0, 10);

		EditorGUILayout.HelpBox("The Attack Distance controls how far away your player can attack from.", MessageType.None, true);

		EditorGUILayout.Space();

		//self.MinDamage = EditorGUILayout.IntSlider ("Min Damage", self.MinDamage, 1, 25);
		//self.MaxDamage = EditorGUILayout.IntSlider ("Max Damage", self.MaxDamage, 1, 25);
		self.MinDamage = EditorGUILayout.IntField("Min Damage", self.MinDamage);
		self.MaxDamage = EditorGUILayout.IntField("Max Damage", self.MaxDamage);

		EditorGUILayout.HelpBox("The Min and Max damage your player can do. A value will randonly be generated between these two values. They can be equal if you want consistent damage.", MessageType.None, true);

		EditorGUILayout.Space();





		EditorGUILayout.LabelField("Sound Options", EditorStyles.boldLabel);

		EditorGUILayout.Space();

		self.useImpactSounds = EditorGUILayout.Toggle ("Use Impact Sounds", self.useImpactSounds);

		EditorGUILayout.HelpBox("Use Impact Sounds enable or disable Impact Sounds.", MessageType.None, true);

		EditorGUILayout.Space();

		if (self.useImpactSounds)
		{
			self.impactSoundSize = EditorGUILayout.IntSlider("Impact Sound Size", self.impactSoundSize, 1, 20);

			EditorGUILayout.HelpBox("Impact Sounds allow you to set an array of sounds that will play dynamically for each hit on an AI. This will pick from a selection of up to 20 sounds. You can choose to enable or disable sounds using the Use Impact Sounds check box.", MessageType.None, true);

			EditorGUILayout.Space();
			
			if(self.impactSoundSize > self.foldOutList.Count)              
			{
				var temp = (self.impactSoundSize - self.foldOutList.Count);
				for(int j = 0; j < temp ; j++)
					self.foldOutList.Add(true);                      
			}
			
			if(self.impactSoundSize > self.impactSounds.Count)                 
			{
				var temp1 = self.impactSoundSize - self.impactSounds.Count;
				for(int j = 0; j < temp1 ; j++)
				{
					self.impactSounds.Add(new AudioClip() );    
				}
			}
			
			if(self.impactSounds.Count > self.impactSoundSize)
			{
				self.impactSounds.RemoveRange( (self.impactSoundSize), self.impactSounds.Count - (self.impactSoundSize));       
				self.foldOutList.RemoveRange( (self.impactSoundSize), self.foldOutList.Count-(self.impactSoundSize));
			}
			
			for(int i = 0; i < self.impactSounds.Count; i++)
			{                   
				int tempCount = i + 1;
				self.impactSounds[i] = (AudioClip)EditorGUILayout.ObjectField("Impact Sound " + tempCount + ":" , self.impactSounds[i], typeof(AudioClip), true );
				GUILayout.Space(10);
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		self.useImpactOtherSounds = EditorGUILayout.Toggle ("Use Impact Other Sounds", self.useImpactOtherSounds);

		EditorGUILayout.HelpBox("Use Impact Other Sounds enable or disable Impact Other Sounds.", MessageType.None, true);

		EditorGUILayout.Space();

		if (self.useImpactOtherSounds)
		{
			self.impactOtherSoundSize = EditorGUILayout.IntSlider("Impact Other Sound Size", self.impactOtherSoundSize, 1, 20);

			EditorGUILayout.HelpBox("Impact Other Sounds allow you to set an array of sounds that will play dynamically for each hit on a non-AI object. This will pick from a selection of up to 20 sounds. You can choose to enable or disable sounds using the Use Impact Other Sounds check box.", MessageType.None, true);
			
			EditorGUILayout.Space();
			
			if(self.impactOtherSoundSize > self.foldOutListOther.Count)              
			{
				var temp2 = (self.impactOtherSoundSize - self.foldOutListOther.Count);
				for(int j = 0; j < temp2 ; j++)
					self.foldOutListOther.Add(true);                      
			}
			
			if(self.impactOtherSoundSize > self.impactOtherSounds.Count)                 
			{
				var temp3 = self.impactOtherSoundSize - self.impactOtherSounds.Count;
				for(int j = 0; j < temp3 ; j++)
				{
					self.impactOtherSounds.Add(new AudioClip() );    
				}
			}
			
			if(self.impactOtherSounds.Count > self.impactOtherSoundSize)
			{
				self.impactOtherSounds.RemoveRange( (self.impactOtherSoundSize), self.impactOtherSounds.Count - (self.impactOtherSoundSize));       
				self.foldOutListOther.RemoveRange( (self.impactOtherSoundSize), self.foldOutListOther.Count-(self.impactOtherSoundSize));
			}
			
			for(int ii = 0; ii < self.impactOtherSounds.Count; ii++)
			{                   
				int tempCount1 = ii + 1;
				self.impactOtherSounds[ii] = (AudioClip)EditorGUILayout.ObjectField("Impact Other Sound " + tempCount1 + ":" , self.impactOtherSounds[ii], typeof(AudioClip), true );
				GUILayout.Space(10);
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		self.useAttackSound = EditorGUILayout.Toggle ("Use Attack Sound", self.useAttackSound);

		EditorGUILayout.HelpBox("Use Attack Sounds enable or disable Attack Sounds.", MessageType.None, true);

		EditorGUILayout.Space();

		if (self.useAttackSound)
		{
			self.attackSoundSize = EditorGUILayout.IntSlider("Attack Sound Size", self.attackSoundSize, 1, 20);
			
			EditorGUILayout.HelpBox("Attack Sounds allow you to set an array of sounds that will play dynamically for each swing. This will pick from a selection of up to 20 sounds. You can choose to enable or disable sounds using the Use Impact Other Sounds check box.", MessageType.None, true);
			
			EditorGUILayout.Space();
			
			if(self.attackSoundSize > self.foldOutListAttack.Count)              
			{
				var temp = (self.attackSoundSize - self.foldOutListAttack.Count);
				for(int j = 0; j < temp ; j++)
					self.foldOutListAttack.Add(true);                      
			}
			
			if(self.attackSoundSize > self.attackSounds.Count)                 
			{
				var temp1 = self.attackSoundSize - self.attackSounds.Count;
				for(int j = 0; j < temp1 ; j++)
				{
					self.attackSounds.Add(new AudioClip() );    
				}
			}
			
			if(self.attackSounds.Count > self.attackSoundSize)
			{
				self.attackSounds.RemoveRange( (self.attackSoundSize), self.attackSounds.Count - (self.attackSoundSize));       
				self.foldOutListAttack.RemoveRange( (self.attackSoundSize), self.foldOutListAttack.Count-(self.attackSoundSize));
			}
			
			for(int i = 0; i < self.attackSounds.Count; i++)
			{                   
				int tempCount = i + 1;
				self.attackSounds[i] = (AudioClip)EditorGUILayout.ObjectField("Attack Sound " + tempCount + ":" , self.attackSounds[i], typeof(AudioClip), true );
				GUILayout.Space(10);
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Effect Options", EditorStyles.boldLabel);

		self.useBloodEffect = EditorGUILayout.Toggle ("Use Blood Effect?", self.useBloodEffect);

		EditorGUILayout.HelpBox("Use Blood Effect controls whether or not there will be blood hit effects.", MessageType.None, true);

		EditorGUILayout.Space();

		if (self.useBloodEffect)
		{
			self.bloodEffect = (GameObject)EditorGUILayout.ObjectField ("Blood Effect", self.bloodEffect, typeof(GameObject), false);

			EditorGUILayout.HelpBox("The Blood Effect used when hitting an AI. The blood is spawn where the hit position is.", MessageType.None, true);
		}

		EditorGUILayout.Space();
		
		self.useHitEffect = EditorGUILayout.Toggle ("Use Hit Effect?", self.useHitEffect);

		EditorGUILayout.HelpBox("Use Hit Effect controls whether or not there will be hit effects when hitting other objects.", MessageType.None, true);
		
		EditorGUILayout.Space();
		
		if (self.useHitEffect)
		{
			self.hitOtherEffect = (GameObject)EditorGUILayout.ObjectField ("Hit Other Effect", self.hitOtherEffect, typeof(GameObject), false);

			EditorGUILayout.HelpBox("The Hit Other Effect used when hitting something other than an AI. Examples can be the terrain and other game objects.", MessageType.None, true);
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();





		EditorGUILayout.LabelField("Tag Options", EditorStyles.boldLabel);
		
		if(self.HitTags != null)
		{
			if(self.HitTags.Count > 0)
			{
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				for (int j = 0; j < self.HitTags.Count; ++j)
				{
					EditorGUILayout.BeginHorizontal ();
					self.HitTags[j] = EditorGUILayout.TextField("Tag " + (j + 1), self.HitTags[j]);
					EditorGUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal();
					GUILayout.Space(75);
					if (GUILayout.Button("Remove Tag"))
					{
						self.HitTags.RemoveAt(j);
						--j;
					}
					GUILayout.Space(75);
					GUILayout.EndHorizontal();

					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
				}
			}

			if(self.HitTags.Count == 0)
			{
				EditorGUILayout.HelpBox("There are currently no tags for your player to hit.", MessageType.Info);
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Tag", GUILayout.MinWidth(90), GUILayout.MaxWidth(90)))
			{
				self.HitTags.Add("Tag");
			}
			GUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}
}
