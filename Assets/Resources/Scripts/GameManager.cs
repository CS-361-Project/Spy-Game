using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Fan> fanList;
	List<Guard> zombieList;
	List<Burner> burnerList;
	List<Chemical> chemicalList;
	List<LaserSensor> sensorList;
	List<Tile> tileList;

	List<Tile> survivorHubs;
	bool quad1, quad2, quad3, quad4;

	List<Survivor> survivorList;

	GameObject wallFolder, tileFolder, doorFolder, guardFolder, burnerFolder, chemicalFolder, fanFolder, sensorFolder;

	Tile[,] board;
	public int width;
	public int height;
	int count;

	int finishX = 0;
	int finishY = 0;

	List<Tile[]> sections;
	ZombieControl zombieCtrl;

	// Use this for initialization
	void Start() {
		width = 50;
		height = 50;

		zombieCtrl = new GameObject().AddComponent<ZombieControl>();
		zombieCtrl.init(this);
		fanList = new List<Fan>();
		zombieList = new List<Guard>();
		burnerList = new List<Burner>();
		chemicalList = new List<Chemical>();
		sensorList = new List<LaserSensor>();
		tileList = new List<Tile>();

		survivorHubs = new List<Tile> ();
		quad1 = false; quad2 = false; quad3 = false; quad4 = false;

		survivorList = new List<Survivor>();

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
//		buildLevel(10, 10);
//		buildTestChamber(10, 10);
//		addGuard(2, 3);
//		addGuard(2, 4);
//		addGuard(4, 2);
//		addGuard(3, 2);
//		addGuard(2, 2);
//		addGuard(1, 2);
//		addFrank (5, 4);
//		addFan(new Vector2(1, 1), "E");
//		addFan(new Vector2(1, 6), "E");
//		addSensor(1, 2, new Vector2(1, 0));
		//addBurner(new Vector2(1, 1));
		//addChemical (new Vector2 (2, 1));
		//buildLevel();
		//buildLevel(22,22);
		generateLevel(width, height);
//		addChemical (new Vector2 (2, 1));
		count = 0;
	}
	
	// Update is called once per frame
	void Update() {
		if (zombieList.Count <= 0) {
			print("The Game has ended");
			//TODO: Make an End Game screen
		}
		count++;
//		if (count == 150) {
//			getTile(6, 6).setFire(1);
//		}
	}

	void generateLevel(int width, int height){
		int survivorCount = 0;
		this.width = width;
		this.height = height;
		board = new Tile[width, height];
		float xSeed1 = Random.Range (-9999f, 9999f);
		float xSeed2 = Random.Range (-9999f, 9999f);
		float ySeed1 = Random.Range (-9999f, 9999f);
		float ySeed2 = Random.Range (-9999f, 9999f);
		branch (1, height / 2, 1, 0, xSeed1, ySeed1);
		branch (width / 2, 1, 0, 1, xSeed2, ySeed2);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (board[x, y] == null) {
					board[x, y] = addWall(x, y);
				}
				else {
//					if (Random.value > 0.6 && zombieList.Count < 200) {
//						addGuard(x, y);
//					}
					if (survivorCount < 3) {
						addSurvivor (x, y, survivorCount);
						survivorCount++;
					}
				}
			}
		}

		int zombiePriority = 0;

		for (int i = 0; i < 250; i++) {
			addGuard(1, height / 2, zombiePriority);
			zombiePriority++;
		}
		while (survivorHubs.Count < 4) {
			Tile hub = getRandomEmptyTile ();
			switch (findQuadrant (hub)) {
			case 0:
				print ("invalid tile");
				break;
			case 1:
				if (!quad1) {
					survivorHubs.Add (hub);
					quad1 = true;
				}
				break;
			case 2:
				if (!quad2) {
					survivorHubs.Add (hub);
					quad2 = true;
				}
				break;
			case 3:
				if (!quad3) {
					survivorHubs.Add (hub);
					quad3 = true;
				}
				break;
			case 4:
				if (!quad4) {
					survivorHubs.Add (hub);
					quad4 = true;
				}
				break;
			}
		}
		print ("done!");
		foreach (Tile hub in survivorHubs) {
			hub.setColor ();
		}
	}


	

	int findQuadrant(Tile hub){
		int x = hub.posX;
		int y = hub.posY;

		if (x < (float)width/2F) {
			if (y < (float)height/2F) {
				return 1;
			} else if (y > (float)height/2F) {
				return 2;
			}
		} else if (x > (float)width/2F) {
			if (y < (float)height/2F) {
				return 4;
			} else if (y > (float)height/2F) {
				return 3;
			}
		}
		return 0;

	}

	void branch(int x, int y, int dx, int dy, float xSeed, float ySeed){
//		print("Branching at " + x + ", " + y);
		if (x>=width-1 || x<1 || y>=height-1 || y<1) {
			return;
		}
		if (board[x, y] != null) {
			return;
		}
		board[x, y] = addTile(x, y, 0, false);
		int r = Random.Range(1, 16);
		bool bit1 = (r & 1) == 1;
		bool bit2 = (r & 2) == 2;
//		float r2 = Random.Range(0f, 1f);
		float r2 = Mathf.PerlinNoise(x * .5f + xSeed, y * .5f + ySeed);
//		print("r2: " + r2);
		if (r2 <= .35f) {
			if (dx == 0) {
				if (bit1) {
					branch(x + 1, y, 1, 0, xSeed, ySeed);
				}
				if (bit2) {
					branch(x - 1, y, -1, 0, xSeed, ySeed);
				}
			}
			else if (dy == 0) {
				if (bit1) {
					branch(x, y + 1, 0, 1, xSeed, ySeed);
				}
				if (bit2) {
					branch(x, y - 1, 0, -1, xSeed, ySeed);
				}
			}
			else {
				print("this shouldn't happen");
				return;
			}
		}
		branch(x + dx, y + dy, dx, dy, xSeed, ySeed);
	}


	public void moveTo(List<Guard> guards, Vector2 point){
		setTargetTile(getClosestTile(point));
		int split = 12;
		foreach (Guard g in guards) {
			//print(findPathToTarget(g.tile).Count);
			List<Vector2> points = pathToPoints(findPathToTarget(g.tile));
			g.targetPositions = pathToPoints(findPathToTarget(g.tile));
			/*for (int p =0;p<points.Count;p+=split){
				List<Vector2> optPoints = points.GetRange(p, Mathf.Min(split, points.Count - p));
				g.targetPositions.AddRange(optimizePath(optPoints));
			}*/
			if (g.targetPositions.Count > 0)
				g.targetPositions.RemoveAt(0);
			/*if (g.targetPositions.Count > 0) {
				g.targetPositions.RemoveAt(0);
			}*/
		}
	}

	#region building levels
	void buildLevel(int width, int height){
		
		this.width = width;
		this.height = height;
		board = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || y == 0 || x == width - 1 || y == height - 1) {
					board[x, y] = addWall(x, y);
				}
				else if (Random.value > 0.8) {
					board[x, y] = addWall(x, y);
				}
				else {
					board[x, y] = addTile(x, y, 0, false);
					if (Random.value > 0.6 && zombieList.Count < 200) {
						addGuard(x, y);
					}

				}
			}
		}

	}


	public int[] planSectionPath(Tile startTile, Tile endTile){
		List<int> integers = new List<int>();
		List<Tile> pathTiles = getTilePath(startTile, endTile, true);
		int currentSection = -1;
		foreach (Tile t in pathTiles) {
			if (t.section != currentSection && t.section > -1) {
				currentSection = t.section;
				integers.Add(currentSection);
			}
		}
		return integers.ToArray();
	}

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

	public Tile[] getSection(int sectionNum){
		//print(sections.Count + ":" + sectionNum);
		return sections[sectionNum];
	}
	#endregion

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
		Tile checkTile = getTile(i, j);
		if (checkTile != null && checkTile.isPassable()) {
			return checkTile;
		}
		else {
			for(int k=1; k<(width>height?width/2:height/2); k++) {
				Tile left = getTile(i - k, j);
				if (left != null && left.isPassable()) {
					return left;
				}
				Tile right = getTile(i + k, j);
				if (right != null && right.isPassable()) {
					return right;
				}
				Tile up = getTile(i, j + k);
				if (up != null && up.isPassable()) {
					return up;
				}
				Tile down = getTile(i, j - k);
				if (down != null && down.isPassable()) {
					return down;
				}
			}
		}
		return checkTile;
	}

	public Tile getFinishTile() {
		return getTile(finishX, finishY);
	}

	public Tile getRandomEmptyTile() {
		return tileList[Random.Range(0, tileList.Count)];
	}

	public List<Guard> getZombieList() {
		return zombieList;
	}

	public List<Survivor> getSurvivorList() {
		return survivorList;
	}

	public void removeZombie(Guard g) {
		zombieCtrl.removeZombie(g);
		zombieList.Remove(g);
	}

	public void removeSurvivor(Survivor s){
		survivorList.Remove(s);
	}


	public void resetPathTiles(){
		foreach (Tile t in board) {
			t.dist = -1;
			t.crowdFactor = 0;
		}
	}

	#region pathfinding
	public List<Vector2> getPath(Tile start, Tile end, bool ignoreDoors) {
		return optimizePath(pathToPoints(getTilePath(start, end, ignoreDoors)));
//		return pathToPoints(getTilePath(start, end, ignoreDoors));
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
		//allPaths[0].RemoveAt(0);
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

	public void setTargetTile(Tile targetTile){
		List<Tile> queue = new List<Tile>();
		resetPathTiles();
		queue.Add(targetTile);
		targetTile.dist = 0;
		while (queue.Count > 0) {
			Tile currTile = queue[0];
			queue.RemoveAt(0);
			foreach (Tile neighbor in currTile.getNeighbors()) {
				if (neighbor.dist < 0 && neighbor.isPassable()) {
					neighbor.dist = currTile.dist + 1;
					queue.Add(neighbor);
				}
			}
		}
	}

	public List<Tile> findPathToTarget(Tile startTile){
		List<Tile> path = new List<Tile>();
		if (startTile.dist < 0)
			return new List<Tile>();
		Tile curr = startTile;
		path.Add(startTile);
		while (curr.dist > 0) {
			Tile currMin = curr;
			foreach (Tile neighbor in curr.getNeighbors()) {
				if (neighbor.dist < curr.dist && neighbor.dist >= 0) {
					if (currMin.dist >= curr.dist)
						currMin = neighbor;
					else if (currMin.crowdFactor + currMin.dist > neighbor.crowdFactor + neighbor.dist) {
						currMin = neighbor;
					}
				}
			}
			curr = currMin;
			currMin.crowdFactor += 0.5f;
			path.Add(curr);
		}
		return path;
	}

	public List<Tile> getTilePath(Tile startTile, Tile endTile, bool ignoreDoors){
		List<Tile> queue = new List<Tile>();
		startTile.dist = 0;
		bool foundPath = false;
		queue.Add(startTile);
		Tile closestToEnd = startTile;
		float closestDist = 0;
		while (queue.Count > 0) {
			Tile currTile = queue[0];
			queue.RemoveAt(0);
			bool end = false;
			foreach (Tile neighbor in currTile.getNeighbors()) {
				if (currTile.dist < closestDist) {
					closestToEnd = currTile;
					closestDist = currTile.dist;
				}
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
//			print("No path from " + startTile.transform.position + " to " + endTile.transform.position);
//			resetPathTiles();
//			return new List<Tile>();
			endTile = closestToEnd;
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
//		print("Found path of length " + path.Count + " from " + path[0].transform.position + " to " + path[path.Count - 1].transform.position);
		path.Reverse();
		resetPathTiles();
		return path;
	}
	#endregion
	#region addObjects

	Tile addTile(int x, int y, float fire, bool flammable){
		GameObject tileObj = new GameObject();
		Tile tile = tileObj.AddComponent<Tile>();
		tile.init(x,y,this, fire, 0, true);
		tile.transform.localPosition = new Vector3(x, y, 0);
		tile.transform.parent = tileFolder.transform;
		tileList.Add(tile);
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
	// NOTE: Can definitely come up with a better way to do this so we don't need seperate for loops for each type of object added

	// register each guard to be notified when new fan is toggled
	void addFan(Vector2 position, string direction) {
		GameObject fanObj = new GameObject();
		fanObj.name = "Fan";
		fanObj.transform.position = position;
		Fan fan = fanObj.AddComponent<Fan>();
		fan.init(position.x, position.y, direction, this);
		foreach (Guard g in zombieList) {
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

	void addSurvivor(int x, int y, int priority){
		GameObject survObj = new GameObject();
		survObj.name = "Survivor";
		Survivor surv = survObj.AddComponent<Survivor>();
		surv.init(getTile(x, y), this, priority);
		survivorList.Add(surv);

	}

	void addSensor(int x, int y, Vector2 direction) {
		GameObject sensorObj = new GameObject();
		sensorObj.name = "Laser Sensor";
		LaserSensor sensor = sensorObj.AddComponent<LaserSensor>();
		foreach (Guard g in zombieList) {
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
		zombieList.Add(guard);
	}


	void addBurner(Vector2 position) {
		GameObject burnerObj = new GameObject();
		burnerObj.name = "Burner";
		burnerObj.transform.position = position;
		Burner burner = burnerObj.AddComponent<Burner>();
		burner.init(getTile((int)position.x, (int)position.y));
		foreach (Guard g in zombieList) {
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
		foreach (Guard g in zombieList) {
			chemical.ChemicalToggled += g.onChemicalToggled;
		}
		chemical.transform.parent = chemicalFolder.transform;
		chemicalList.Add(chemical);
	}
	#endregion
}

