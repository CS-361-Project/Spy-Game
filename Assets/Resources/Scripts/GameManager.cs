using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Fan> fanList;
	List<Guard> guardList;
	List<Burner> burnerList;
	List<Chemical> chemicalList;

	Tile[,] board;
	public int width;
	public int height;
	int count;
	// Use this for initialization
	void Start() {
		fanList = new List<Fan>();
		guardList = new List<Guard>();
		burnerList = new List<Burner>();
		chemicalList = new List<Chemical> ();
		buildBoard(8, 8);
		addGuard(2, 3);
		addFrank (5, 4);
		//addFan(new Vector2(4, 1), new Vector2(-1, 0));
		//addBurner(new Vector2(1, 1));
		//addChemical (new Vector2 (2, 1));

//		addChemical (new Vector2 (2, 1));
		count = 0;
	}
	
	// Update is called once per frame
	void Update() {
		count++;
		if (count == 150) {
			getTile(1, 1).setFire(1);
		}
	}

	void buildBoard(int width, int height){
		this.width = width;
		this.height = height;
		board = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
					board[x, y] = addWall(x, y);
				}
				else if (x == 1 && y == 1) {
					board[x, y] = addTile(x, y, 0);
					addBurner(new Vector2 (x, y));
				}
				else {
					if (UnityEngine.Random.value > 1.1)
						board[x, y] = addWall(x, y);
					else
						board[x, y] = addTile(x, y, 0);
				}
			}
		}



	}

	Tile addTile(int x, int y, float fire){
		GameObject tileObj = new GameObject();
		Tile tile = tileObj.AddComponent<Tile>();
		tile.init(x,y,this, fire, 0, true);
		tile.transform.localPosition = new Vector3(x, y, 0);
		return tile;
	}

	Wall addWall(int x, int y) {
		GameObject wallObj = new GameObject();
		Wall wall = wallObj.AddComponent<Wall>();
		wall.init(x, y, this);
		wall.transform.localPosition = new Vector3(x, y, 0);
		return wall;
	}

	public Tile getTile(int x,int y){
		return board[x, y];
	}

	public Tile getClosestTile(Vector2 check){
		int i = (int)Mathf.Floor(check.x);
		int j = (int)Mathf.Floor(check.y);
		return getTile(i, j);
	}

	// NOTE: Can definitely come up with a better way to do this so we don't need seperate for loops for each type of object added

	// register each guard to be notified when new fan is toggled
	void addFan(Vector2 position, Vector2 direction) {
		GameObject fanObj = new GameObject();
		fanObj.name = "Fan";
		fanObj.transform.position = position;
		Fan fan = fanObj.AddComponent<Fan>();
		foreach (Guard g in guardList) {
			fan.FanToggled += g.onFanToggled;
		}
		fanList.Add(fan);
	}

	void addFrank(int x, int y) {
		GameObject frankObj = new GameObject();
		frankObj.name = "Frank";
		Frank frank = frankObj.AddComponent<Frank>();
		foreach (Fan fan in fanList) {
			fan.FanToggled += frank.onFanToggled;
		}
		foreach (Burner bb in burnerList) {
			bb.BurnerToggled += frank.onBurnerToggled;
		}
		foreach (Chemical chem in chemicalList) {
			chem.ChemicalToggled += frank.onChemicalToggled;
		}
		frank.init(getTile(x, y), this);
	}


	// register each guard to be notified when a fan is toggled
	void addGuard(int x, int y) {
		GameObject guardObj = new GameObject();
		guardObj.name = "Guard";
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
		guard.init(getTile(x, y), this);
		guardList.Add(guard);
	}


	void addBurner(Vector2 position) {
		GameObject burnerObj = new GameObject();
		burnerObj.name = "Burner";
		burnerObj.transform.position = position;
		Burner burner = burnerObj.AddComponent<Burner>();
		burner.init(getTile((int)position.x, (int)position.y));
		foreach (Guard g in guardList) {
			burner.BurnerToggled += g.onBurnerToggled;
		}
		burnerList.Add(burner);
	}

	void addChemical(Vector2 position) {
		GameObject chemObj = new GameObject();
		chemObj.name = "Chemical";
		chemObj.transform.position = position;
		Chemical chemical = chemObj.AddComponent<Chemical>();
		foreach (Guard g in guardList) {
			chemical.ChemicalToggled += g.onChemicalToggled;
		}
		chemicalList.Add(chemical);
	}
}

