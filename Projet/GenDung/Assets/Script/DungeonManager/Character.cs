﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaterData", menuName = "Characters/CharaterData", order = 1)]
public class Character : ScriptableObject {

	public string Name;

	public Sprite 
	TempSprite,
	ICON;

	public GameObject PrefabCharacter; // ce qui contient l animators et les animations et autres, sur lequels il y a les scripts

	public int 
	Health_PV,
	ActionPoints_PA,
	CloseAttaqueValue,
	DistanceAttaqueValue;

	public string
	story = "this is a story of a random player, we still need to implement this yes...";
}
