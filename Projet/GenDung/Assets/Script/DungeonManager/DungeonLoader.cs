﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using UnityEngine.EventSystems;

public class DungeonLoader : MonoBehaviour {

	public string 
	activeScene, //check active scene
	previousScene, //previous scene
	roomType; // just a checker to see what room is the actual room that we are using.

	public GameObject 
	roomPrefab,	//prefab de la room générale
	doorPrefab,	//prefab de la porte
	chestRoomUIPrefab,	//prefab de l'UI des salles chest
	fightRoomUIPrefab,	//prefab de l'UI des salles fight
	bossRoomUIPrefab,	//prefab de l'UI des salles boss
	enemyPrefabUIICON,	//prefab de l'UI pour l'icone d'un enemi
	bossPrefabUIICON,	//prefab de l'UI pour l'icone de boss
	DebugPrefab,		//prefab de l'UI pour le debug ( surtout le changement de salle de donjon sur la map)
	EndDungeonUIPrefab;	//prefab de l'UI pour l'ecran de victoire apres un donjon

	GameObject 
	BG, //background de la salle
	doorinstantiated; //la porte instantiée

	public RoomList[] 
	roomListDungeon; // this are the dungeons, 

	public GameObject[] 
	dungeonOnTheMap;	//list des boutons des donjons sur la carte

	int 
	index; //index pour les salles du donjon
	public int 
	dungeonIndex, //index pour le donjon
	dungeonUnlockedIndex;	//index pour le donjon unlocked

	public bool
	loadOnce3, //lié au godeeperintodungeon vu que c'est un bouton ca a besoin de verifier que ca ne se fait qu'une fois
	loadOnce2,	//lié au loadRoom vu que c'est un bouton ca a besoin de verifier que ca ne se fait qu'une fois
	loadbutton,	//pour ne charger qu'une fois la scene donjon
	loadbutton2,	//pour ne charger qu'une fois la scene map
	loadOnceDoor,	//pour ne charger qu'une fois la porte
	roomIsLocked,	//permet de verouiller une porte
	isUIinstantiated,	//verifier dans le donjon si l'interface a bien été instanciée
	doOnceCoroutine,	//lance la coroutine qu'une fois
	sceneLoaded,	//attendre que la scene est bien chargé
	InstrantiateOnceEndDungeon,	//instancier une fois l'écran de fin de donjon
	EndDungeon;	//verifier si le donjon est fini ou pas

	void Start () {
		//permet de vérifier ce qu'est la scene actuelle et d'attendre qu'elle aie fini de charger
		SceneManager.sceneLoaded += OnSceneLoaded;

		//seulement d'en l'éditor pour charger les modules de debug
		#if UNITY_EDITOR
			Instantiate(DebugPrefab);
			GameObject.Find("IncreaseUnlockIndexButton").GetComponent<Button> ().onClick.AddListener (UnlockNextDungeon);
			GameObject.Find("DecreaseUnlockIndexButton").GetComponent<Button> ().onClick.AddListener (DecreaseUnlockDungeonIndex);
			GameObject.Find("ResetUnlockIndexButton").GetComponent<Button> ().onClick.AddListener (ResetUnlockDungeonIndex);
		#endif
	}

	//permet de vérifier ce qu'est la scene actuelle et d'attendre qu'elle aie fini de charger
	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		sceneLoaded = true;
	}


	void FixedUpdate () {
		//permet de savoir quel nom de scene
		activeScene = SceneManager.GetActiveScene ().name;

		//attendre que la scene soit chargée
		if (!sceneLoaded) {
			
			//-----------Dungeon gestion scene-------------//
			if (activeScene == "Dungeon") {

				//initialise la référence au background de la salle
				BG = GameObject.FindGameObjectWithTag ("backgroundOfRoom");

				//permet de charger la salle room si on vient de changer de scene
				if (previousScene != activeScene) {
					LoadRoom ();
				}
			}		

			//-----------Map gestion scene-------------//
			if (activeScene == "Map") { 

				//----------ecran de fin de donjon-----------//
				if(EndDungeon && !InstrantiateOnceEndDungeon){

					//instantie l'écran de fin
					InstrantiateOnceEndDungeon = true;
					Instantiate(EndDungeonUIPrefab);

					//verifie dans toutes les salles
					for (int i = 0; i < roomListDungeon[dungeonIndex].RoomOfTheDungeon.Count(); i++) {

						//si la salle est de type fight
						if (roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].roomType.ToString() == "fight") {
							
							//prendre ses enfants et instantier une icone pour chaque + definir leur parent dans l'écran de fin
							for (int l = 0; l < roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].enemies; l++) {

								GameObject enemyUI;
								enemyUI = Instantiate (enemyPrefabUIICON);
								enemyUI.transform.SetParent (GameObject.FindGameObjectWithTag ("EnemyPanel").transform, false);
								enemyUI.transform.GetChild(0).GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].enemiesList [l].enemyIcon;
							}
						}

						//si la salle est de type boss
						if (roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].roomType.ToString() == "boss") {
							
							//prendre ses enfants et instantier une icone pour chaque + definir leur parent dans l'écran de fin
							for (int l = 0; l < roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].enemies; l++) {

								GameObject enemyUI;
								enemyUI = Instantiate (enemyPrefabUIICON);
								enemyUI.transform.SetParent (GameObject.FindGameObjectWithTag ("EnemyPanel").transform, false);
								enemyUI.transform.GetChild(0).GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].enemiesList [l].enemyIcon;
							}

							//vu que c'est un type boss il y a aussi le boss a instancier
							GameObject bossUI;
							bossUI = Instantiate (bossPrefabUIICON);
							bossUI.transform.SetParent (GameObject.FindGameObjectWithTag ("EnemyPanel").transform, false);
							bossUI.transform.GetChild(0).GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [i].bossList [0].bossIcon;
						}
					}
				}

				//va rechercher dans la liste de donjon dans le prefab de carte l'index qui permet de savoir en passant la souris dans quel donjon on va entrer
				dungeonIndex = GameObject.FindGameObjectWithTag ("DungeonButtonMap").GetComponent<DungeonListOnMap> ().indexLocal;

				//ajoute a tout les boutons sur la carte le fait de charger la salle donjon
				dungeonOnTheMap [dungeonIndex].transform.Find("DungeonButton").GetComponent<Button> ().onClick.AddListener (LoadSceneDungeon);

				//assure que les salles sont bien unlock
				roomIsLocked = false;
				//reinitialise le systeme de check de salle
				previousScene = null;


				//----------Dungeon Unlocking Feature ------------//
				if (dungeonUnlockedIndex <= dungeonOnTheMap.Length) {
					
					for (int i = 0; i < dungeonOnTheMap.Length; i++) {
						//Met faux tous les donjons non débloqué
						dungeonOnTheMap [i].SetActive (false);
					}
					for (int i = 0; i < dungeonUnlockedIndex; i++) {
						//Met vrai tous les donjons débloqué
						dungeonOnTheMap [i].SetActive (true);
						dungeonOnTheMap [i].transform.Find ("DungeonButton").GetComponent<Button> ().interactable = true;
						dungeonOnTheMap [i].transform.Find ("DungeonButton").GetComponent<Button> ().image.color = Color.white;

						//---- Grisé le donjon suivant------//
						if(i +1 < dungeonOnTheMap.Length){
							dungeonOnTheMap [i+1].SetActive (true);
							dungeonOnTheMap [i+1].transform.Find ("DungeonButton").GetComponent<Button> ().interactable = false;
							dungeonOnTheMap [i+1].transform.Find ("DungeonButton").GetComponent<Button> ().image.color = Color.grey;
						}
					}
				}
			}
		} else {
			//systeme de vérification pour voir si la scene a bien charger
			//au sinon lance une coroutine qui attend peu et reinitialise les données
			if (!doOnceCoroutine) {
				doOnceCoroutine = true;
				StartCoroutine ("WaitLoading");
			}
		}
	}

	//load the dungeon scene
	void LoadSceneDungeon () {
		if (!loadbutton) {
			loadbutton = true;
			InstrantiateOnceEndDungeon = false;
			SceneManager.LoadScene ("Dungeon");
		}
	}

	//charge la scene map
	void LoadSceneMap () {
		if (!loadbutton2) {
			loadbutton2 = true;
			SceneManager.LoadScene ("Map");
			EndDungeon = true;
		}
	}

	//load first time room function
	void LoadRoom () {
		if (!loadOnce2) {
			loadOnce2 = true;
			Instantiate (roomPrefab);

			//reset l'index du donjon
			index = 0;

			//attribue le background de la salle
			BG = GameObject.FindGameObjectWithTag ("backgroundOfRoom");
			BG.transform.GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].back;

			//instantiate the door
			loadDoor();

			//permet de vérifier le type de salle
			GetRoomType ();
		}
	}


		
	void GoDeeperInTheDungeon () {
		if (!loadOnce3) {
			//si la salle n'est pas vérouillée
			if (!roomIsLocked) {
				loadOnce3 = true;

				//reset for ui
				isUIinstantiated = false;

				//look throught all the stats and asign them to object in the scene depending on the tags
				//Change le background en fonction de la salle
				if (roomListDungeon [dungeonIndex].RoomOfTheDungeon[index].roomID <= roomListDungeon [dungeonIndex].RoomOfTheDungeon.Count)
					BG.transform.GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].back;

				//charge la porte
				loadDoor ();

				//permet de vérifier le type de salle
				GetRoomType ();

				//change l'index pour naviger dans le donjon
				index = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].doorList [0].connectingTo - 1;

				//attend pour ne pas spammer le bouton de porte
				StartCoroutine ("waitLagForClicking");
				}
			}
		}

	//permet de savoir le type de la room
	void GetRoomType()
	{
		//cherche pour la salle précise et store son room type
		if (roomListDungeon [dungeonIndex].RoomOfTheDungeon[index].roomID <= roomListDungeon [dungeonIndex].RoomOfTheDungeon.Count)
			roomType = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].roomType.ToString(); 

		//--------CHEST---------//
		if (roomType == "chest") {
			roomIsLocked = true;

			if (!isUIinstantiated) {
				isUIinstantiated = true;
				Instantiate (chestRoomUIPrefab);
			}
			GameObject.FindGameObjectWithTag ("unlockRoomButton").GetComponent<Button> ().onClick.AddListener (UnlockRoom);
		}

		//--------FIGHT---------//
		if (roomType == "fight") {
			roomIsLocked = true;

			if (!isUIinstantiated) {
				isUIinstantiated = true;
				Instantiate (fightRoomUIPrefab);
			}
			GameObject.FindGameObjectWithTag ("unlockRoomButton").GetComponent<Button> ().onClick.AddListener (UnlockRoom);

			//instantie pour chaque enemi dans la liste une icone
			for (int i = 0; i < roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].enemies; i++) {
				GameObject enemyUI;
				enemyUI = Instantiate (enemyPrefabUIICON);
				enemyUI.transform.SetParent (GameObject.FindGameObjectWithTag ("EnemyPanel").transform, false);
				enemyUI.transform.GetChild(0).GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].enemiesList [i].enemyIcon;
			}

		}

		//--------BOSS---------//
		if (roomType == "boss") {
			roomIsLocked = true;

			if (!isUIinstantiated) {
				isUIinstantiated = true;
				Instantiate (bossRoomUIPrefab);
			}
			GameObject.FindGameObjectWithTag ("unlockRoomButton").GetComponent<Button> ().onClick.AddListener (UnlockRoom);

			//instantie l'icone de boss
			GameObject bossUI;
			bossUI = Instantiate (bossPrefabUIICON);
			bossUI.transform.SetParent (GameObject.FindGameObjectWithTag ("EnemyPanel").transform, false);
			bossUI.transform.GetChild(0).GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].bossList [0].bossIcon;

			//instantie pour chaque enemi dans la liste une icone
			for (int i = 0; i < roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].enemies; i++) {

				GameObject enemyBossUI;
				enemyBossUI = Instantiate (enemyPrefabUIICON);
				enemyBossUI.transform.SetParent (GameObject.FindGameObjectWithTag ("EnemyPanel").transform, false);
				enemyBossUI.transform.GetChild(0).GetComponent<Image> ().sprite = roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].enemiesList [i].enemyIcon;
			}
		}
	}

	//charge la porte suivante et ses données
	void loadDoor () {
		if (!loadOnceDoor) {
			loadOnceDoor = true;

			//initialise the door at each time we call it
			GameObject[] tempDoor;
			tempDoor = GameObject.FindGameObjectsWithTag ("door");

			//desactive les portes suivantes
			if (tempDoor != null) {
				for (int i = 0; i < tempDoor.Length; i++) {
					tempDoor [i].SetActive (false);
				}
			}

			//assigne la porte a ses coordonnées
			doorinstantiated = Instantiate (doorPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
			doorinstantiated.transform.SetParent (GameObject.FindGameObjectWithTag ("Canvas").transform, false);
			doorinstantiated.transform.localPosition = new Vector3 (roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].doorList [0].coordinate.x, roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].doorList [0].coordinate.y,0);

			//assigne les scripts que la porte a lorsqu'on clique dessus en fonction de son emplacement dans le donjon
			if (roomListDungeon [dungeonIndex].RoomOfTheDungeon [index].doorList [0].doorType.ToString () == "LastDoor") {
				Debug.Log ("hey im last door");
				doorinstantiated.GetComponent<Button> ().onClick.AddListener (LoadSceneMap);
				doorinstantiated.GetComponent<Button> ().onClick.AddListener (UnlockNextDungeon);
			} else {
				doorinstantiated.GetComponent<Button> ().onClick.AddListener (GoDeeperInTheDungeon);
			}

			StartCoroutine ("loadWaitRoom");
		}
	}

	public void UnlockRoom () {
		roomIsLocked = false;
		GameObject.FindGameObjectWithTag ("canvasInDungeon").SetActive (false);
	}

	public void UnlockNextDungeon(){
		if (dungeonUnlockedIndex < dungeonOnTheMap.Length) {
			dungeonUnlockedIndex++;
		}
	}

	public void DecreaseUnlockDungeonIndex(){
		if (dungeonUnlockedIndex > 1) {
			dungeonUnlockedIndex--;
		}
	}
		
	public void ResetUnlockDungeonIndex(){
		dungeonUnlockedIndex = 1;
	}

	//----------------------------IENUMERATOR---------------------------------//

	//attend que la salle soit bien chargée
	IEnumerator loadWaitRoom(){
		yield return new WaitForSeconds (0.1f);
		loadOnceDoor = false;
	}

	//attend que la scene soit bien chargée
	IEnumerator WaitLoading(){
		yield return new WaitForSeconds (0.05f);

		//reinitialise la scene pour charger a nouveau lors du prochain donjon le LOADROOM
		if (activeScene == "Dungeon") {
			previousScene = "";
		}

		if (activeScene == "Map") {
			dungeonOnTheMap = GameObject.FindGameObjectWithTag ("DungeonButtonMap").GetComponent<DungeonListOnMap> ().dungeonOnTheMapList;

			//reinitialise la scene pour charger a nouveau lors du prochain donjon le LOADROOM
			previousScene = "";

			//réinitialise les données
			loadOnce3 = false;
			loadOnce2 = false;
			loadbutton = false;
			isUIinstantiated = false;
			loadbutton2 = false;
		}

		//relance la recherche des données dans le fixedUpdate
		sceneLoaded = false;
		doOnceCoroutine = false;
	}

	//coroutine qui attend pour ne pas spammer le bouton de porte
	IEnumerator waitLagForClicking () {
		yield return new WaitForSeconds (0.1f);
		loadOnce3 = false;
	}
}
