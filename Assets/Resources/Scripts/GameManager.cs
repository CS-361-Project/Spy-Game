using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Fan> fanList;
	List<Guard> guardList;
	List<Burner> burnerList;
	List<Chemical> chemicalList;
	// Use this for initialization
	void Start() {
		fanList = new List<Fan>();
		guardList = new List<Guard>();
		burnerList = new List<Burner>();
		chemicalList = new List<Chemical> ();
//		for (int i = 0; i < 30; i++) {
//			addGuard();
//		}
		addFan(new Vector2(3, 0), new Vector2(-1, 0));
		addBurner(new Vector2(0, 0));
		addChemical (new Vector2 (1, 0));

		buildBoard(6, 6);

	}
	
	// Update is called once per frame
	void Update() {
	
	}

	void buildBoard(int width, int height){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				addTile(x, y);
			}
		}

	}

	void addTile(int x, int y){
		GameObject tileObj = new GameObject();
		Tile tile = tileObj.AddComponent<Tile>();
		tile.init();
		tile.transform.localPosition = new Vector3(x, y, 0);

	}

	// NOTE: Can definitely come up with a better way to do this so we don't need seperate for loops for each type of object added

	// register each guard to be notified when new fan is toggled
	void addFan(Vector2 position, Vector2 direction) {
		GameObject fanObj = new GameObject();
		fanObj.transform.position = position;
		Fan fan = fanObj.AddComponent<Fan>();
		foreach (Guard g in guardList) {
			fan.FanToggled += g.onFanToggled;
		}
		fanList.Add(fan);
	}


	// register each guard to be notified when a fan is toggled
	void addGuard() {
		GameObject guardObj = new GameObject();
		guardObj.transform.position = new Vector2(Random.value * 10 - 5, Random.value * 10 - 5);
		Guard guard = guardObj.AddComponent<Guard>();
		foreach (Fan fan in fanList) {
			fan.FanToggled += guard.onFanToggled;
		}
		foreach (Burner bb in burnerList) {
			bb.BurnerToggled += guard.onBurnerToggled;
		}
		foreach (Chemical chem in chemicalList) {
			chem.ChemicalToggled += guard.onChemicalToggled;
		}
		guardList.Add(guard);
	}


	void addBurner(Vector2 position) {
		GameObject burnerObj = new GameObject();
		burnerObj.transform.position = position;
		Burner burner = burnerObj.AddComponent<Burner>();
		foreach (Guard g in guardList) {
			burner.BurnerToggled += g.onBurnerToggled;
		}
		burnerList.Add(burner);
	}

	void addChemical(Vector2 position) {
		GameObject chemObj = new GameObject();
		chemObj.transform.position = position;
		Chemical chemical = chemObj.AddComponent<Chemical>();
		foreach (Guard g in guardList) {
			chemical.ChemicalToggled += g.onChemicalToggled;
		}
		chemicalList.Add(chemical);
	}
}

