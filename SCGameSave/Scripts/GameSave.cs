 using System.Xml;
 using System.Xml.Serialization;
using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using System;

 public class GameSave { 
	// Name of the game save object
    [XmlAttribute("name")]
    public string Name;

	#region Custom Data
	// ----------------------------------------------------
	// CUSTOM DATA Section
	// Add your own custom data that you want to store here
	// ----------------------------------------------------

	// Static variables - not serialized
	public static int LevelsPerCampaign = 28;
    
	// Non-static variables - serialized
	public int Coins = 0;

	// Serialized array of strings
	[XmlArray("UpgradesList"),XmlArrayItem("Upgrade")]
	public string[] Upgrades;

	// ----------------------------------------------------
	// END CUSTOM DATA SECTION
	// ----------------------------------------------------
	#endregion

	// ----------------------------------------------------
	// VALUES dictionary
	// A generic dictionary which you can use to store any miscelaneous values
	// ----------------------------------------------------
	[XmlIgnoreAttribute]
	public Dictionary<string, string> Data = new Dictionary<string, string>();
	
	public string Values {
		get {
			string ret = "";
			int count = 0;
			foreach(string key in Data.Keys) {
				if(count != 0) ret += "&";
				ret += WWW.EscapeURL(key) + "=" + WWW.EscapeURL(Data[key]);
				count++;
			}
			return ret;
		}
		set {
			Data.Clear();
			string [] parts = value.Split('&');
			for(int i = 0; i < parts.Length; i++) {
				string [] split = parts[i].Split('=');
				if(split.Length == 2) {
					string key = WWW.UnEscapeURL(split[0]);
					string val = WWW.UnEscapeURL(split[1]);
					Data[key] = val;
				}
			}
		}
	}
	// ----------------------------------------------------
	// END VALUES
	// ----------------------------------------------------

	#region Utility Methods

	// ----------------------------------------------------
	// UTILITY METHODS
	// You can add helper methods to this class to store, retrieve and examine data
	// ----------------------------------------------------

	#region Upgrade Management

	// As an example, we include a couple methods to look at our custom Upgrades array

	public bool AddUpgrade(string upgrade) {
		// Add an item to the Upgrades array to store this upgrade in
		if (Upgrades == null) Upgrades = new string[1];
		else {
			// Make sure its not a duplicate
			//for (int i = 0; i < Upgrades.Length; i++) 
			//	if (Upgrades[i] == upgrade) return false;
			if (HasUpgrade(upgrade)) return false;
						
			string[] newUpgradesArray = new string[Upgrades.Length + 1];
			
			for (int i = 0; i < Upgrades.Length; i++) newUpgradesArray[i] = Upgrades[i];
			
			Upgrades = newUpgradesArray;
		}
		
		Upgrades[Upgrades.Length - 1] = upgrade;
		
		Array.Sort(Upgrades);
		
		return true;
	}
	
	public bool HasUpgrade(string upgrade) {
		if (Upgrades == null) return false;
		
		//bool hasUpgrade = false;
				
		int index = Array.BinarySearch(Upgrades, upgrade);
		return index >= 0;
				
		//for (int i = 0; i < Upgrades.Length; i++) {
		//	if (Upgrades[i] == upgrade) return true; //hasUpgrade = true;
		//}
		
		//return hasUpgrade;
	}
	

	#endregion
	
	#region Medal Management

	// As an example, we include a couple methods to work with our values dictionary

	public bool AddMedal(int level, int medal) {
				
		Data["Medal" + level + ":" + medal] = "1";
				
		return true;
	}
	
	public bool HasMedal(int level, int medal) {
		if (!Data.ContainsKey("Medal" + level + ":" + medal)) return false;
						
		return Data["Medal" + level + ":" + medal] == "1";		
	}

	#endregion
	
	#endregion
 }