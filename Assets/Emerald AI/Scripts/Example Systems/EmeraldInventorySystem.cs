//Black Horizon Studios
//Inventory Example

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EmeraldInventorySystem : MonoBehaviour 
{
	public List<string> playerInventory = new List<string>();
	public Text inventoryListText;

	void Start ()
	{
		inventoryListText = GameObject.Find("Inventory Text").GetComponent<Text>();
	}

	public void RefreshInventory ()
	{
		//Only refresh our inventory list if an inventoryListText is present
		if (inventoryListText != null)
		{
			inventoryListText.text = "";
		}
	}

	public void UpdateInventory ()
	{
		//Only add our loot to the inventory if an inventoryListText is present
		if (inventoryListText != null)
		{
			foreach (string s in playerInventory)
			{
				inventoryListText.text += "-" + s + "\n";
			}
		}
	}
}
