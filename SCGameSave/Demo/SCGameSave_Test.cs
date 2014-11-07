using UnityEngine;
using System.Collections;

public class SCGameSave_Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Load the game save container (or create it if we don't have one)
		GameSaveContainer gsc = GameSaveContainer.Load();

		// We automatically get an active save.  Add a medal for level 0
		//gsc.ActiveSave().AddMedal(0,0);

		// Save the container
		gsc.Save();

		Debug.Log ("We " + (gsc.ActiveSave().HasMedal(0,0)?"do":"don't") + " have a level 0 medal");
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
