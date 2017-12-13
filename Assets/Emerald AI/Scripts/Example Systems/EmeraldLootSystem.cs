//Black Horizon Studios
//Loot Example

using UnityEngine;
using System.Collections;

public class EmeraldLootSystem : MonoBehaviour {

	public bool hasGuaranteedLoot = true;
	public string GuaranteedLoot = "Pelt";
	public string[] MinorLoot = {"10 Gold", "Gem", "Rusty Sword"};
	public string[] RareLoot = {"50 Gold", "Magic Helmet", "Magic Sword"};

	public int MinorLootPercentage = 15;
	public int RareLootPercentage = 5;

	private int MinorLootRoll;
	private int RareLootRoll;
	private GameObject playerInventoryGO;
	private EmeraldInventorySystem playerInventory;
	private bool systemDisabled = false;

	public void GenerateLoot () 
	{
		if (!systemDisabled)
		{
			//Find our inventory
			playerInventoryGO = GameObject.Find("Player Inventory");

			if (playerInventoryGO == null)
			{
				systemDisabled = true;
			}

			if (playerInventoryGO != null)
			{
				playerInventory = playerInventoryGO.GetComponent<EmeraldInventorySystem>();
			}
		}

		if (playerInventory == null)
		{
			Debug.Log("In order for the loot system to work correctly, you will need to have a Player Inventory system in your scene. Go to Windows>Emerald AI>Create Player>Player Inventory System");
		}

		//If our AI has Guaranteed Loot, they will always drop this item on death
		//This can be used for things like materials.
		if (hasGuaranteedLoot && playerInventory != null)
		{
			//Send our generated loot to the player's inventory list
			playerInventory.playerInventory.Add(GuaranteedLoot);
		}

		//Roll for our Minor Loot
		MinorLootRoll = Random.Range(1, 101);

		//If our roll is less than or equal to our MinorLoot percentage, roll for loot within the loot array
		if (MinorLootRoll <= MinorLootPercentage && playerInventory != null)
		{
			MinorLootRoll = Random.Range(0, MinorLoot.Length);

			//Send our generated loot to the player's inventory list
			playerInventory.playerInventory.Add(MinorLoot[MinorLootRoll]);
		}

		//Roll for our Rare Loot
		RareLootRoll = Random.Range(1, 101);
		
		//If our roll is less than or equal to our RareLoot percentage, roll for loot within the loot array 
		if (RareLootRoll <= RareLootPercentage && playerInventory != null)
		{
			RareLootRoll = Random.Range(0, RareLoot.Length);

			//Send our generated loot to the player's inventory list
			playerInventory.playerInventory.Add(RareLoot[RareLootRoll]);
		}

		//If the Player's Inventory is found, and they have the necessary UI, add these items to the player's UI Inventory
		if (playerInventory != null)
		{
			playerInventory.RefreshInventory();
			playerInventory.UpdateInventory();
		}
	}
}
