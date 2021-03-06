﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {

	public GameData gameData;

	public void ContinueGameLoadMap () {
		SceneManager.LoadScene ("Map");
		GameObject.Find ("DontDestroyOnLoad").GetComponent<DungeonLoader> ().dungeonUnlockedIndex = gameData.DungeonIndexData;
	}

	public void NewGameLoadMap () {
		SceneManager.LoadScene ("CharacterCreation");
		GameObject.Find ("DontDestroyOnLoad").GetComponent<DungeonLoader> ().dungeonUnlockedIndex = 1;
	}

	public void LoadDungeon () {
		SceneManager.LoadScene ("Dungeon");
	}
	public void LoadMarket () {
		SceneManager.LoadScene ("Market");
	}
	public void LoadMainMenu () {
		SceneManager.LoadScene ("MainMenu");
	}
}
