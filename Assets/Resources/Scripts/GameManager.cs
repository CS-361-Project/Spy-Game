using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Fan> fanList;
	List<Guard> guardList;
	List<Burner> burnerList;
	List<Chemical> chemicalList;
	List<LaserSensor> sensorList;

	GameObject wallFolder, tileFolder, doorFolder, guardFolder, burnerFolder, chemicalFolder, fanFolder, sensorFolder;

	Tile[,] board;
	public int width;
	public int height;
	int count;

	List<Tile[]> sections;

	// Use this for initialization
	void Start() {
		fanList = new List<Fan>();
		guardList = new List<Guard>();
		burnerList = new List<Burner>();
		chemicalList = new List<Chemical>();
		sensorList = new List<LaserSensor>();
		wallFolder = new GameObject();
		wallFolder.name = "Walls";
		tileFolder = new GameObject();
		tileFolder.name = "Tiles";
		doorFolder = new GameObject();
		doorFolder.name = "Doors";
		guardFolder = new GameObject();
		guardFolder.name = "Guards";
		burnerFolder = new GameObject();
		burnerFolder.name = "Burners";
		chemicalFolder = new GameObject();
		chemicalFolder.name = "Chemicals";
		fanFolder = new GameObject();
		fanFolder.name = "Fans";
		sensorFolder = new GameObject();
		sensorFolder.name = "Sensors";
		//buildBoard(10, 10);
		buildLevel(10, 10);
//		addGuard(2, 3);
//		addGuard(2, 4);
		addGuard(4, 2);
		addGuard(3, 2);
		addGuard(2, 2);
		addGuard(1, 2);
		addFrank (5, 4);
		addFan(new Vector2(2, 1), "E");
		addFan(new Vector2(1, 6), "E");
		addSensor(1, 2, new Vector2(1, 0));
		//addBurner(new Vector2(1, 1));
		//addChemical (new Vector2 (2, 1));
		constructSections();
//		addChemical (new Vector2 (2, 1));
		count = 0;
	}
	
	// Update is called once per frame
	void Update() {
		count++;
//		if (count == 150) {
//			getTile(1, 1).setFire(1);
//		}
	}

	void buildLevel(int width, int height){
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
<<<<<<< HEAD
					addFan(new Vector2 (x, y));
=======
					//addBurner(new Vector2 (x, y));
<<<<<<< HEAD
>>>>>>> origin/sam-dev
				} else if((x==4 && y==3) || (x==3 && y==7) || (x==5 && y==7)){
=======
				} else if((x==4 && y==3) || (x==3 && y==7) || (x==6 && y==1)){
>>>>>>> master
					board[x, y] = addDoor(x, y);
				} else if ((x==1 && y==3) || (x==2 && y==3) || (x==3 && y==3) || (x==5 && y==3) || (x==6 && y==3) || (x==6 && y==2) || (x==6 && y==1) ){
					board[x, y] = addWall(x, y);
				} else if ((x==3 && y==3) || (x==5 && y==3) || (x==3 && y==4) || (x==5 && y==4) || (x==3 && y==5) || (x==5 && y==5) || (x==3 && y==6) || (x==5 && y==6) || (x==3 && y==8) || (x==5 && y==8) || (x==4 && y==8)){
					board[x, y] = addWall(x, y);
				}
				else {

						board[x, y] = addTile(x, y, 0);
				}
			}
		}
	}

	/*void buildBoard(int width, int height){
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
				} else if(x==4 && y==5){
					board[x, y] = addDoor(x, y);
				}
				else {
					if (UnityEngine.Random.value > 1f)
						board[x, y] = addWall(x, y);
					else
						board[x, y] = addTile(x, y, 0);
				}
			}
		}
<<<<<<< HEAD
	}*/
=======
	}
>>>>>>> origin/sam-dev

	void constructSections(){
		int numSections = 0;
		sections = new List<Tile[]>();
		foreach (Tile tile in board) {
			if (tile.section == -1 && tile.isPassable()) {
				numSections++;
				sections.Add(fillSection(tile, numSections - 1));
			}
		}
	}

	Tile[] fillSection(Tile section,int sectionNum){
		List<Tile> sectionQueue = new List<Tile>();
		List<Tile> sectionMembers = new List<Tile>();
		sectionQueue.Add(section);
		while (sectionQueue.Count > 0) {
			Tile tile = sectionQueue[0];
			sectionQueue.RemoveAt(0);
			tile.section = sectionNum;
			sectionMembers.Add(tile);
			foreach (Tile neighbor in tile.getNeighbors()) {
				if (neighbor.section == -1 && neighbor.isPassable())
					sectionQueue.Add(neighbor);
			}
		}
		return sectionMembers.ToArray();
	}

	Tile[] getSection(int sectionNum){
		return sections[sectionNum];
	}

	Tile addTile(int x, int y, float fire){
		GameObject tileObj = new GameObject();
		Tile tile = tileObj.AddComponent<Tile>();
		tile.init(x,y,this, fire, 0, true);
		tile.transform.localPosition = new Vector3(x, y, 0);
		tile.transform.parent = tileFolder.transform;
		return tile;
	}

	Wall addWall(int x, int y) {
		GameObject wallObj = new GameObject();
		Wall wall = wallObj.AddComponent<Wall>();
		wall.init(x, y, this);
		wall.transform.localPosition = new Vector3(x, y, 0);
		wall.transform.parent = wallFolder.transform;
		return wall;
	}

	Door addDoor(int x, int y) {
		GameObject doorObj = new GameObject();
		Door door = doorObj.AddComponent<Door>();
		door.init(x, y, this);
		door.transform.localPosition = new Vector3(x, y, 0);
		door.transform.parent = doorFolder.transform;
		return door;
	}

	public Tile getTile(int x,int y){
		if (onBoard(x, y)) {
			return board[x, y];
		}
		else {
			return null;
		}
	}

	public bool onBoard(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	public Tile getClosestTile(Vector2 check){
		int i = (int)Mathf.RoundToInt(check.x);
		int j = (int)Mathf.RoundToInt(check.y);
		return getTile(i, j);
	}

	public Tile getFinishTile() {
		return getTile(width - 2, 1);
	}

	public void resetPathTiles(){
		foreach (Tile t in board) {
			t.dist = -1;
		}
	}
	
	public List<Vector2> getPath(Tile start, Tile end, bool ignoreDoors) {
//		return optimizePath(pathToPoints(getTilePath(start, end, ignoreDoors)));
		return pathToPoints(getTilePath(start, end, ignoreDoors));
	}

	public List<Vector2> optimizePath(List<Vector2> path) {
		if (path.Count  < 3) {
			return path;
		}
		float[] S = new float[path.Count];
		List<Vector2>[] allPaths = new List<Vector2>[path.Count];

		allPaths[path.Count - 1] = new List<Vector2>();
		allPaths[path.Count - 1].Add(path[path.Count - 1]);
		S[path.Count - 1] = 0;
		for (int i = path.Count - 2; i >= 0; i--) {
			float minDist = float.MaxValue;
			int minDistIndex = i + 1;
			for (int j = i + 1; j < path.Count; j++) {
				if (!pathObstructed(path[i], path[j])) {
					float dist = Vector2.Distance(path[i], path[j]) + S[j];
					if (dist < minDist) {
						minDist = dist;
						minDistIndex = j;
					}
				}
			}
			S[i] = minDist;
			allPaths[i] = new List<Vector2>();
			allPaths[i].Add(path[i]);
			foreach (Vector2 v in allPaths[minDistIndex]) {
				allPaths[i].Add(v);
			}
		}
		allPaths[0].RemoveAt(0);
		return allPaths[0];
	}

	bool pathObstructed(Vector2 pos1, Vector2 pos2) {
		Vector2 v = pos2 - pos1;
		float playerRad = .35f;
		Vector2 perpVec = new Vector2(v.normalized.y, -v.normalized.x);
		RaycastHit2D rayHit = Physics2D.Raycast(pos1 + perpVec * playerRad, v.normalized, v.magnitude, 1 << 10);
		RaycastHit2D rayHit2 = Physics2D.Raycast(pos1 - perpVec * playerRad, v.normalized, v.magnitude, 1 << 10);
//		Debug.DrawRay(pos1 + perpVec * playerRad, v);
//		Debug.DrawRay(pos1 - perpVec * playerRad, v);
		return (rayHit.collider != null || rayHit2.collider != null);

	}

	public List<Vector2> pathToPoints(List<Tile> path){
		List<Vector2> points = new List<Vector2>();
		foreach (Tile tile in path) {
			points.Add(tile.transform.position);
		}
		return points;
	}

	public List<Tile> getTilePath(Tile startTile,Tile endTile, bool ignoreDoors){
		List<Tile> queue = new List<Tile>();
		startTile.dist = 0;
		bool foundPath = false;
		queue.Add(startTile);
		while (queue.Count > 0) {
			Tile currTile = queue[0];
			queue.RemoveAt(0);
			bool end = false;
			foreach (Tile neighbor in currTile.getNeighbors()) {
				if (neighbor.dist < 0 && (neighbor.isPassable() || (neighbor is Door && ignoreDoors))) {
					neighbor.dist = currTile.dist + 1;
					if (neighbor == endTile) {
						end = true;
						break;
					}
					queue.Add(neighbor);
				}
			}
			if (end) {
				foundPath = true;
				break;
			}
		}
		if (!foundPath) {
			print("No path from " + startTile.transform.position + " to " + endTile.transform.position);
			resetPathTiles();
			return new List<Tile>();
		}
		List<Tile> path = new List<Tile>();
		path.Add(endTile);
		Tile curr = endTile;
		while (curr.dist > 0) {
			foreach (Tile neighbor in curr.getNeighbors()) {
				if (neighbor.dist == curr.dist - 1) {
					curr = neighbor;
					path.Add(curr);
					break;
				}
			}
		}
		print("Found path of length " + path.Count + " from " + path[0].transform.position + " to " + path[path.Count - 1].transform.position);
		path.Reverse();
		resetPathTiles();
		return path;
	}

	// NOTE: Can definitely come up with a better way to do this so we don't need seperate for loops for each type of object added

	// register each guard to be notified when new fan is toggled
<<<<<<< HEAD
	void addFan(Vector2 position) {
=======
	void addFan(Vector2 position, string direction) {
>>>>>>> origin/sam-dev
		GameObject fanObj = new GameObject();
		fanObj.name = "Fan";
		fanObj.transform.position = position;
		Fan fan = fanObj.AddComponent<Fan>();
<<<<<<< HEAD
		
=======
		fan.init(direction);
<<<<<<< HEAD
>>>>>>> origin/sam-dev
=======
		fan.transform.parent = fanFolder.transform;
>>>>>>> master
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

	void addSensor(int x, int y, Vector2 direction) {
		GameObject sensorObj = new GameObject();
		sensorObj.name = "Laser Sensor";
		LaserSensor sensor = sensorObj.AddComponent<LaserSensor>();
		foreach (Guard g in guardList) {
			sensor.MotionDetected += g.onMotionDetected;
		}
		sensor.init(this, getTile(x, y).transform.position, direction);
		sensor.transform.parent = sensorFolder.transform;
		sensorList.Add(sensor);
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
		foreach (LaserSensor sensor in sensorList) {
			sensor.MotionDetected += guard.onMotionDetected;
		}
		guard.init(getTile(x, y), this);
		guard.transform.parent = guardFolder.transform;
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
		burner.transform.parent = burnerFolder.transform;
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
		chemical.transform.parent = chemicalFolder.transform;
		chemicalList.Add(chemical);
	}
}

