
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EmeraldAIMenu : MonoBehaviour 
{
	[MenuItem ("Window/Emerald AI/Create AI/Create new AI (on selected object)", false, 1)]
    static void AssignEmeraldAIComponents () 
	{
		List<SkinnedMeshRenderer> ObjRenderer = new List<SkinnedMeshRenderer>();

		foreach (GameObject obj in Selection.gameObjects) 
		{
			obj.AddComponent<Emerald_Animal_AI>();

			Emerald_Animal_AI Emerald = obj.GetComponent<Emerald_Animal_AI>();

			Component[] AllComponents = obj.GetComponents<Component>();

			for (int i = 0; i < AllComponents.Length; i++)
			{
				UnityEditorInternal.ComponentUtility.MoveComponentUp(Emerald);
			}

			foreach (SkinnedMeshRenderer R in obj.GetComponentsInChildren<SkinnedMeshRenderer>())
			{
				if (R != obj.GetComponent<SkinnedMeshRenderer>())
				{
					ObjRenderer.Add(R);
				}
			}

			if (ObjRenderer.Count > 0)
			{
				Bounds RendererBounds = ObjRenderer[0].bounds;
				obj.GetComponent<BoxCollider>().size = new Vector3(RendererBounds.extents.x, RendererBounds.size.y, RendererBounds.extents.z);
				obj.GetComponent<BoxCollider>().center = new Vector3(obj.GetComponent<BoxCollider>().center.x, RendererBounds.extents.y, obj.GetComponent<BoxCollider>().center.z);
			}
	}

		if (Selection.gameObjects.Length == 0)
		{
			Debug.Log("In order for the New AI button to work, you must have an active GameObject selected.");
		}
    }	

	[MenuItem ("Window/Emerald AI/Create Player/3rd Person Combat Demo Player", false, 0)]
	static void SpawnPlayerCombat3rdPerson () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Emerald 3rd Person Combat Player.prefab", typeof(GameObject))) as GameObject;
		GameObject PlayerUI = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Player UI.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
		PlayerObject.gameObject.name = "Emerald 3rd Person Combat Player";
		PlayerUI.gameObject.name = "Player UI";
	}

	[MenuItem ("Window/Emerald AI/Create Player/Combat Demo Player", false, 0)]
	static void SpawnPlayerCombat () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Emerald Combat Demo Player.prefab", typeof(GameObject))) as GameObject;
		GameObject PlayerUI = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Player UI.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
		PlayerObject.gameObject.name = "Emerald Combat Demo Player";
		PlayerUI.gameObject.name = "Player UI";
	}

	[MenuItem ("Window/Emerald AI/Create Player/Breeding Demo Player", false, 0)]
	static void SpawnPlayerBreeding () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Emerald Breeding Demo Player.prefab", typeof(GameObject))) as GameObject;
		GameObject PlayerUI = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Player UI.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
		PlayerObject.gameObject.name = "Emerald Breeding Demo Player";
		PlayerUI.gameObject.name = "Player UI";
	}

	[MenuItem ("Window/Emerald AI/Create Player/Player Inventory", false, 0)]
	static void PlayerInventory () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Emerald Inventory Player.prefab", typeof(GameObject))) as GameObject;
		GameObject PlayerUI = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Player/Player Inventory UI.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
		PlayerObject.gameObject.name = "Player Inventory";
		PlayerUI.gameObject.name = "Player Inventory UI";
	}

	[MenuItem ("Window/Emerald AI/Create Example AI/Breedable AI", false, 0)]
	static void SpawnPassiveBreedingAI () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Animals - NPCs/AI Examples/Breedable Animal Example.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.gameObject.name = "Breeding Animal Example";
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
	}

	[MenuItem ("Window/Emerald AI/Create Example AI/Passive AI", false, 0)]
	static void SpawnPassiveAI () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Animals - NPCs/AI Examples/Passive AI Example.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.gameObject.name = "Passive AI Example";
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
	}

	[MenuItem ("Window/Emerald AI/Create Example AI/Fleeing AI", false, 0)]
	static void SpawnFleeingAI () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Animals - NPCs/AI Examples/Fleeing AI Example.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.gameObject.name = "Fleeing AI Example";
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
	}

	[MenuItem ("Window/Emerald AI/Create Example AI/Defensive AI", false, 0)]
	static void SpawnDefensiveAI () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Animals - NPCs/AI Examples/Defensive AI Example.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.gameObject.name = "Defensive AI Example";
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
	}

	[MenuItem ("Window/Emerald AI/Create Example AI/Hostile AI", false, 0)]
	static void SpawnHostileAI () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Animals - NPCs/AI Examples/Hostile AI Example.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.gameObject.name = "Hostile AI Example";
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
	}

	[MenuItem ("Window/Emerald AI/Create Example AI/Hostile Skeleton AI (Mecanim)", false, 0)]
	static void SpawnHostileMecanimAI () 
	{
		Selection.activeObject = SceneView.currentDrawingSceneView;
		GameObject PlayerObject = GameObject.Instantiate( AssetDatabase.LoadAssetAtPath("Assets/Emerald AI/Prefabs/Animals - NPCs/AI Examples/Skeleton AI Hostile (Mecanim) Example.prefab", typeof(GameObject))) as GameObject;
		PlayerObject.gameObject.name = "Skeleton AI Hostile (Mecanim) Example";
		PlayerObject.transform.position = new Vector3 (0, 0, 0);
	}

	[MenuItem ("Window/Emerald AI/Documentation/Home", false, 1000)]
	static void Home ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Emerald_Animal_AI_Wikia");
	}
	
	[MenuItem ("Window/Emerald AI/Documentation/Documentation", false, 1000)]
	static void Introduction ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Documentation");
	}
	
	[MenuItem ("Window/Emerald AI/Tutorials/Video Tutorials", false, 100)]
	static void VideoTutorials ()
	{
		Application.OpenURL("https://www.youtube.com/playlist?list=PLlyiPBj7FznY7q4bdDQgGYgUByYpeCe07");
	}
	
	[MenuItem ("Window/Emerald AI/Tutorials/Text Tutorials", false, 100)]
	static void Tutorials ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Tutorials");
	}

	[MenuItem ("Window/Emerald AI/Tutorials/RFPS Tutorial", false, 100)]
	static void RFPSTutorial ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Tutorials#Implementing_RFPS_with_Emerald_AI");
	}

	[MenuItem ("Window/Emerald AI/Tutorials/UFPS Tutorial", false, 100)]
	static void UFPSTutorial ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Tutorials#Implement_UFPS_with_Emerald_AI");
	}

	[MenuItem ("Window/Emerald AI/Documentation/Example AI Stats", false, 1000)]
	static void ExampleStats ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Example_AI_Stat_Tables");
	}
	
	[MenuItem ("Window/Emerald AI/Documentation/Code References", false, 1000)]
	static void CodeReferences ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Code_References");
	}
	
	[MenuItem ("Window/Emerald AI/Documentation/Example Scripts", false, 1000)]
	static void ExampleScripts ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Example_Scripts");
	}
	
	[MenuItem ("Window/Emerald AI/Documentation/Solutions to Possible Issues", false, 1000)]
	static void Solutions ()
	{
		Application.OpenURL("http://emerald-animal-ai.wikia.com/wiki/Solutions_to_Possible_Issues");
	}
	
	[MenuItem ("Window/Emerald AI/Documentation/Forum", false, 1000)]
	static void Forums ()
	{
		Application.OpenURL("http://forum.unity3d.com/threads/released-emerald-animal-ai-v1-1-dynamic-wildlife-predators-prey-packs-herds-npcs-more.336521/");
	}
	
	[MenuItem ("Window/Emerald AI/Customer Service", false, 10000000)]
	static void CustomerService ()
	{
		Application.OpenURL("http://www.blackhorizonstudios.com/contact/");
	}
}