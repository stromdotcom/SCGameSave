using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
 
 [XmlRoot("SaveGameCollection")]
 public class GameSaveContainer {
	
	// VALUES
    [XmlArray("GameSaveList"),XmlArrayItem("GameSave")]
    public GameSave[] GameSaves;
    
	public int ActiveGameSave = -1;
	// END VALUES
	
	private static string path = Path.Combine(Application.persistentDataPath, "GameSaves.xml");
	
	#region Load and Save
	
	/// <summary>
	/// Save this TMGGameSaveContainer to disk
	/// </summary>
    public void Save() {
		Debug.Log("Saving game");
		
		//string path = Path.Combine(Application.persistentDataPath, "GameSaves.xml");
		
#if UNITY_IPHONE || UNITY_STANDALONE_OSX
		string save = SaveToText();
		PlayerPrefs.SetString("GameSave", save);
		PlayerPrefs.Save();

#elif !UNITY_WEBPLAYER
        var serializer = new XmlSerializer(typeof(GameSaveContainer));
        using (var stream = new FileStream(path, FileMode.Create)) {
            serializer.Serialize(stream, this);
        }
#else
		string save = SaveToText();
		PlayerPrefs.SetString("GameSave", save);
		PlayerPrefs.Save();
#endif
    }
    
	/// <summary>
	/// Load the TMGGameSaveContainer from disk
	/// </summary>
	public static GameSaveContainer Load() {
		//string path = Path.Combine(Application.persistentDataPath, "GameSaves.xml");
		
#if UNITY_IPHONE || UNITY_STANDALONE_OSX
		string gsc_text = PlayerPrefs.GetString("GameSave");
		GameSaveContainer container;
		
		if (gsc_text == null || gsc_text == "") {
			gsc_text = PlayerPrefs.GetString("GameSave");	
		}
		
		if (gsc_text == null || gsc_text == "") {
			container = new GameSaveContainer();
			container.AddNewGameSave("default", true);
		} else {
			container = GameSaveContainer.LoadFromText(gsc_text);	
			if (container.ActiveSave() == null) container.AddNewGameSave("default", true);
		}	
		
		return container;
#elif !UNITY_WEBPLAYER
		if (!File.Exists(path)) {
			GameSaveContainer container = new GameSaveContainer();
			container.AddNewGameSave("default", 0, true);
			container.Save();
			
			return container;
		}
		
        var serializer = new XmlSerializer(typeof(GameSaveContainer));
        using(var stream = new FileStream(path, FileMode.Open)) {
            return serializer.Deserialize(stream) as GameSaveContainer;
        }

#else
		string gsc_text = PlayerPrefs.GetString("GameSave");
		GameSaveContainer container;
		if (gsc_text == null || gsc_text == "") {
			container = new GameSaveContainer();
			container.AddNewGameSave("default", 0, true);
		} else {
			container = GameSaveContainer.LoadFromText(gsc_text);	
			if (container.ActiveSave() == null) container.AddNewGameSave("default", 0, true);
		}	
		
		return container;
#endif
    }
		
	/// <summary>
	/// Saves the TMGGameSaveContainer to a string.  Use to post the serialized object to a web server.
	/// </summary>
	/// <returns>
	/// THe serialized TMGGameSaveContainer as a string of text.
	/// </returns>
	public string SaveToText() {
		var serializer = new XmlSerializer(typeof(GameSaveContainer));
        
		StringWriter stringStream = new StringWriter();
		
        serializer.Serialize(stringStream, this);
        	
		return stringStream.ToString();
	}
	
    /// <summary>
    /// Loads a TMGGameSaveContainer from text.  Use after loading the XML from a web server.
    /// </summary>
    /// <returns>
    /// The deserialized TMGGameSaveContainer
    /// </returns>
    /// <param name='text'>
    /// The XML text describing the TMGContainer
    /// </param>
    public static GameSaveContainer LoadFromText(string text) {
        var serializer = new XmlSerializer(typeof(GameSaveContainer));
        return serializer.Deserialize(new StringReader(text)) as GameSaveContainer;
    }
	
	#endregion
	
	#region Save Management
	public bool DeleteSaves() {
#if UNITY_IPHONE || UNITY_STANDALONE_OSX
		PlayerPrefs.SetString("GameSave","");
		PlayerPrefs.Save();
		return true;	
#elif !UNITY_WEBPLAYER		
		if (File.Exists(path)) {
			File.Delete(path);
			return true;
		} else return false;
#else
		PlayerPrefs.SetString("GameSave","");
		return true;
#endif
	}
	
	public bool SetActiveGameSave(int index) {
		if (GameSaves == null) {
			ActiveGameSave = -1;
			return false;
		}
		
		if (index >= 0 && index < GameSaves.Length) {
			ActiveGameSave = index;	
			return true;
		} 
		
		ActiveGameSave = -1;
		
		return false;
	}
	
	public int AddNewGameSave(string name, bool makeActive) {
		if (GameSaves == null) {
			GameSaves = new GameSave[1];
		} else {
			GameSave[] newGameSaveArray = new GameSave[GameSaves.Length + 1];
			
			for (int i = 0; i < GameSaves.Length; i++) newGameSaveArray[i] = GameSaves[i];
			
			GameSaves = newGameSaveArray;			
		}				
		
		GameSave newGameSave = new GameSave();
		newGameSave.Name = name;

		GameSaves[GameSaves.Length - 1] = newGameSave;
		
		if (makeActive) ActiveGameSave = GameSaves.Length - 1;
		
		return GameSaves.Length - 1;
	}
	
	public GameSave ActiveSave() {
		if (GameSaves == null || ActiveGameSave < 0 || ActiveGameSave >= GameSaves.Length) return null;
		
		return GameSaves[ActiveGameSave];
	}
	#endregion
	
	
	
	#region GUID
	public static string GetGUID() {
		if (PlayerPrefs.GetString("UserID") != "") return PlayerPrefs.GetString("UserID");
		
		System.Guid guid = System.Guid.NewGuid();
		
		PlayerPrefs.SetString("UserID", "" + guid);
		
		return "" + guid;
	}
	#endregion
 }