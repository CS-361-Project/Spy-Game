using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	List<Guard> zombieList;
	List<Tile> emptyTileList;
	List<Survivor> enemyList, survivorList;
	Tile[] zombieSpawn, survivorSpawn;

	float zombieSpawnProgress, survivorSpawnProgress;
	public float zombieSpawnInterval, survivorSpawnInterval;
	float baseZombieSpawnInterval, baseSurvivorSpawnInterval;

	ControlPoint[] controlPointList;
	bool quad1, quad2, quad3, quad4;

	GameObject wallFolder, tileFolder, doorFolder, zombieFolder, survivorFolder;
	GameObject winScreen, loseScreen, newGameMenu;

	Tile[,] board;
	public int width;
	public int height;
	public float gameSpeed;

	int finishX = 0;
	int finishY = 0;

	int startZombies, startSurvivors;

	ZombieControl zombieCtrl;
	SurvivorControl survivorCtrl;
	public AudioControl audioCtrl;

	int maxZombiePriority;
	int maxSurvivorPriority;

	public AudioSource audioSource;
	public AudioClip defeatScreen;
	int numControlPoints = 6;

	int difficulty = 2;

//	GameObject cursor;
//	SpriteRenderer cursorRend;

	public enum GameState {
		Playing,
		Paused,
		Victory,
		Defeat}

	;

	GameState state;

	// Use this for initialization
	void Start() {
		width = 60;
		height = 60;

		GameObject.Find("Minimap Camera").GetComponent<Minimap>().init(this);

		gameSpeed = 1f;

		//Set up Cursor
//		Cursor.visible = false;
//		cursor = new GameObject();
//		cursorRend = cursor.AddComponent<SpriteRenderer>();
//		cursorRend.sprite = Resources.Load<Sprite>("Sprites/Mouse");
//		cursorRend.sortingLayerName = "UI";
//		cursorRend.sortingOrder = 4;
//		cursor.layer = LayerMask.NameToLayer("Mouse");
//		cursorRend.color = Color.red;
//
//		cursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);


		maxZombiePriority = 0;
		maxSurvivorPriority = 0;

		winScreen = GameObject.Find("WinPanel");
		loseScreen = GameObject.Find("LosePanel");
		newGameMenu = GameObject.Find("NewGamePanel");

		winScreen.SetActive(false);
		loseScreen.SetActive(false);
		newGameMenu.SetActive(false);

		zombieCtrl = new GameObject().AddComponent<ZombieControl>();

		survivorCtrl = new GameObject().AddComponent<SurvivorControl>();

		audioCtrl = new GameObject().AddComponent<AudioControl>();

		zombieList = new List<Guard>();
		emptyTileList = new List<Tile>();

		survivorSpawnProgress = 0;
		zombieSpawnProgress = 0;

		startZombies = 100;
		startSurvivors = 25;

		baseSurvivorSpawnInterval = 12;
		baseZombieSpawnInterval = 3;
		survivorSpawnInterval = baseSurvivorSpawnInterval;
		zombieSpawnInterval = baseZombieSpawnInterval;

		controlPointList = new ControlPoint[numControlPoints];
		quad1 = false;
		quad2 = false;
		quad3 = false;
		quad4 = false;

		enemyList = new List<Survivor>();
		survivorList = new List<Survivor>();

		wallFolder = new GameObject();
		wallFolder.name = "Walls";
		tileFolder = new GameObject();
		tileFolder.name = "Tiles";
		zombieFolder = new GameObject();
		zombieFolder.name = "Zombies";
		survivorFolder = new GameObject();
		survivorFolder.name = "Survivors";

		setDifficulty(2);
		generateLevel(width, height);
		survivorCtrl.init(this, numControlPoints);
		zombieCtrl.init(this);
		audioCtrl.init(this);

		audioSource = gameObject.AddComponent<AudioSource>();
		defeatScreen = Resources.Load("Audio/Music/160418 Zombie_Paradise", typeof(AudioClip)) as AudioClip;

		state = GameState.Playing;
	}

	public void restart() {
		winScreen.SetActive(false);
		loseScreen.SetActive(false);
		newGameMenu.SetActive(false);
		survivorList.Clear();
		emptyTileList.Clear();
		survivorSpawnProgress = 0;
		zombieSpawnProgress = 0;
		gameSpeed = 1;


		for (int i = enemyList.Count - 1; i >= 0; i--) {
			Survivor s = enemyList[i];
			removeSurvivor(enemyList[i]);
			Destroy(s.gameObject);
		}
		for (int i = zombieList.Count - 1; i >= 0; i--) {
			Guard z = zombieList[i];
			removeZombie(zombieList[i]);
			Destroy(z.gameObject);
		}
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				Destroy(board[i, j].gameObject);
				board[i, j] = null;
			}
		}
//		for (int i = 0; i < controlPointList.Length; i++) {
//			Destroy(controlPointList[i].gameObject);
//		}

		generateLevel(width, height);

		survivorCtrl.init(this, numControlPoints);
		zombieCtrl.init(this);
		audioCtrl.init(this);
		state = GameState.Playing;
	}
	
	// Update is called once per frame
	void Update() {
//		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//		cursor.transform.position = mousePos;
//		cursorRend.transform.localScale = new Vector3(1,1,1) *  Camera.main.orthographicSize * .02f;

		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		else if (Input.GetKeyDown(KeyCode.Escape)) {
			newGameMenu.SetActive(!newGameMenu.activeInHierarchy);
		}
		if (zombieSpawnProgress >= 1) {
			zombieSpawnProgress -= 1;
			spawnZombies(1);
		}
		if (survivorSpawnProgress >= 1) {
			survivorSpawnProgress -= 1;
			spawnSurvivors(1);
		}
		if (state == GameState.Playing) {
			if (zombieList.Count == 0) {
				loseScreen.SetActive(true);
				state = GameState.Defeat;
				audioSource.PlayOneShot(defeatScreen);
			}
			else {
				ControlPoint.Owner owner = controlPointList[0].currentOwner;
				if (owner != ControlPoint.Owner.Unclaimed) {
					bool allPointsCaptured = true;
					for (int i = 1; i < numControlPoints; i++) {
						if (controlPointList[i].currentOwner != owner) {
							allPointsCaptured = false;
							break;
						}

					}
					if (allPointsCaptured) {
						if (owner == ControlPoint.Owner.Survivor) {
							loseScreen.SetActive(true);
							state = GameState.Defeat;
						}
						else if (owner == ControlPoint.Owner.Zombie && enemyList.Count == 0) {
							winScreen.SetActive(true);
							state = GameState.Victory;
						}
					}
				}
			}
		}
		zombieSpawnProgress += Time.deltaTime / zombieSpawnInterval;
		survivorSpawnProgress += Time.deltaTime / survivorSpawnInterval;
	}

	public void generateLevel(int w, int h) {
		int survivorCount = 0;
		survivorSpawn = new Tile[10];
		zombieSpawn = new Tile[10];

		this.width = width + 2;
		this.height = height + 2;
		board = new Tile[width, height];
		float xSeed1 = Random.Range(-9999f, 9999f);
		float xSeed2 = Random.Range(-9999f, 9999f);
		float ySeed1 = Random.Range(-9999f, 9999f);
		float ySeed2 = Random.Range(-9999f, 9999f);
		branch(1, height / 2, 1, 0, xSeed1, ySeed1);
		branch(width - 2, height / 2, -1, 0, xSeed2, ySeed2);
//		branch(width / 2, 1, 0, 1, xSeed2, ySeed2);

		int spawnTiles = 0;
		foreach (int x in Enumerable.Range(1, 2)) {
			foreach (int y in Enumerable.Range(height/2 - 2, 5)) {
				if (board[x, y] == null) {
					board[x, y] = addTile(x, y, 0, false);
				}
				if (board[width - x - 1, y] == null) {
					board[width - x - 1, y] = addTile(width - x - 1, y, 0, false);
				}

				zombieSpawn[spawnTiles] = board[x, y];
				survivorSpawn[spawnTiles] = board[width - x - 1, y];
				spawnTiles++;
			}
		}
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (board[x, y] == null) {
					board[x, y] = addWall(x, y);
				}
//				else {
//					if (Random.value > 0.6 && zombieList.Count < 400) {
//						addGuard(x, y);
//					}
//					if (survivorCount < 15) {
//						addSurvivor (x, y);
//						survivorCount++;
//					}
//				}
			}
		}

		int numCtrlPointsFound = 0;
		Tile[] hubs = new Tile[numControlPoints];
		controlPointList = new ControlPoint[numControlPoints];
		for (int i = 0; i < numControlPoints; i++) {
			switch (i) {
				case 0:
					hubs[0] = getClosestEmptyTile(new Vector2(Random.Range(0, width / 2), Random.Range(height / 2, height)));
					break;
				case 1:
					hubs[1] = getClosestEmptyTile(new Vector2(Random.Range(width / 2, width), Random.Range(height / 2, height)));
					break;
				case 2:
					hubs[2] = getClosestEmptyTile(new Vector2(Random.Range(0, width / 2), Random.Range(0, height / 2)));
					break;
				case 3:
					hubs[3] = getClosestEmptyTile(new Vector2(Random.Range(width / 2, width), Random.Range(0, height / 2)));
					break;
				default:
					hubs[i] = getRandomEmptyTile();
					break;
			}
		}
		int k = 0;
		foreach (Tile hub in hubs) {
			GameObject pointObj = new GameObject();
			ControlPoint point = pointObj.AddComponent<ControlPoint>();
			point.init(hub.posX, hub.posY, this, survivorCtrl);
			point.transform.position = new Vector3(hub.posX, hub.posY, 0);
			board[hub.posX, hub.posY] = point;
			controlPointList[k++] = point;
			emptyTileList.Remove(hub);
			emptyTileList.Add(point);
			Destroy(hub.gameObject);
		}

		spawnZombies(startZombies);
		spawnSurvivors(startSurvivors);
	}

	public void setDifficulty(int level) {
		switch (level) {
			case 0:
				numControlPoints = 4;
				baseSurvivorSpawnInterval = 17;
				baseZombieSpawnInterval = 2;
				Survivor.setDamageRange(30, 45);
				Survivor.setShotInterval(.65f);
				Turret.setTurretDamageRange(45, 65);
				Turret.setTurretShotInterval(.1f);
				Turret.setShootingTime(5f);
				startZombies = 120;
				startSurvivors = 20;
				break;
			case 1:
				numControlPoints = 6;
				baseSurvivorSpawnInterval = 15;
				baseZombieSpawnInterval = 3;
				Survivor.setDamageRange(35, 55);
				Survivor.setShotInterval(.65f);
				Turret.setTurretDamageRange(66, 81);
				Turret.setTurretShotInterval(.05f);
				Turret.setShootingTime(3f);
				startZombies = 120;
				startSurvivors = 20;
				break;
			case 2:
				numControlPoints = 6;
				baseSurvivorSpawnInterval = 10;
				baseZombieSpawnInterval = 3;
				Survivor.setDamageRange(40, 65);
				Survivor.setShotInterval(.55f);
				Turret.setTurretDamageRange(66, 81);
				Turret.setTurretShotInterval(.05f);
				Turret.setShootingTime(3f);
				startZombies = 100;
				startSurvivors = 25;
				break;
			default:
				numControlPoints = 6;
				baseSurvivorSpawnInterval = 12;
				baseZombieSpawnInterval = 3;
				Survivor.setDamageRange(30, 55);
				Survivor.setShotInterval(.55f);
				Turret.setTurretDamageRange(66, 81);
				Turret.setTurretShotInterval(.05f);
				Turret.setShootingTime(3f);
				startZombies = 100;
				startSurvivors = 25;
				break;
		}
		survivorSpawnInterval = baseSurvivorSpawnInterval;
		zombieSpawnInterval = baseZombieSpawnInterval;
	}

	public int findQuadrant(Tile hub) {
		int x = hub.posX;
		int y = hub.posY;

		if (x < (float)width / 2F) {
			if (y < (float)height / 2F) {
				return 1;
			}
//			else if (y > (float)height / 2F) {
			else {
				return 2;
			}
		}
		else if (x > (float)width / 2F) {
			if (y < (float)height / 2F) {
				return 4;
			}
//			else if (y > (float)height / 2F) {
			else {
				return 3;
			}
		}
		return 0;

	}

	void branch(int x, int y, int dx, int dy, float xSeed, float ySeed) {
//		print("Branching at " + x + ", " + y);
		if (x >= width - 1 || x < 1 || y >= height - 1 || y < 1) {
			return;
		}
		if (board[x, y] != null) {
			return;
		}
		board[x, y] = addTile(x, y, 0, false);
		int r = Random.Range(1, 16);
		bool bit1 = (r & 1) == 1;
		bool bit2 = (r & 2) == 2;
		float r2 = Mathf.PerlinNoise(x * .5f + xSeed, y * .5f + ySeed);
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


	public List<Vector2> splitOptPath(List<Vector2> points, int split) {
		List<Vector2> results = new List<Vector2>();
		for (int p = 0; p < points.Count; p += split) {
			List<Vector2> optPoints = points.GetRange(p, Mathf.Min(split, points.Count - p));
//			results.Add(optPoints[0]);
			results.AddRange(optimizePath(optPoints));
		}
		if (results.Count > 0) {
			/*print("Removing first element from path of length " + results.Count);
			foreach (Vector2 v in results) {
				print(v);
			}*/
			results.RemoveAt(0);
		}
		return results;
	}

	public void moveTo(List<Guard> guards, Vector2 point, bool shouldOverwrite, bool attackSurvivors) {
		setTargetTile(getClosestEmptyTile(point));
		Tile mouseTile = getClosestTile(point);
		int split = 12;

		foreach (Guard g in guards) {
			//print(findPathToTarget(g.tile).Count);
			Vector2 startPoint = g.transform.position;
			if (!shouldOverwrite && g.isPathing()) {
				startPoint = g.targetPositions.Last();
			}
			Tile start = getClosestEmptyTile(startPoint);
			bool foundCut = false;
			foreach (Tile t in start.getNxNArea(3)) {
				if (t != null) {
					if (t.pathToTarget.Count > 0) {
						if (!pathObstructed(startPoint, t.transform.position)) {
							if (shouldOverwrite)
								g.targetPositions = new List<Vector2>();
							//g.targetPositions.Add(t.transform.position);
							g.targetPositions.AddRange(t.pathToTarget);
							foundCut = true;
							break;
						}
					}
				}
			}
			if (!foundCut) {
				//print("Doing this");
				//List<Vector2> points = pathToPoints(findPathToTarget(g.tile));
				if (shouldOverwrite) {
					g.targetPositions = splitOptPath(pathToPoints(findPathToTarget(getClosestTile(startPoint))), 10);
				}
				else {
					g.targetPositions.AddRange(splitOptPath(pathToPoints(findPathToTarget(getClosestTile(startPoint))), 10));
				}
				g.tile.pathToTarget = g.targetPositions;
			}
			if (g.targetPositions.Count > 0) {
				g.direction = g.targetPositions[0] - (Vector2)g.transform.position;
				g.targetPositions.RemoveAt(g.targetPositions.Count - 1);
			}
			g.targetPositions.Add(point);
			//misc guard stuff
			if (shouldOverwrite || (g.chasingSurvivor && g.targetPositions.Count < 2)) {
//				g.startTimer();
				g.chasingSurvivor = false;
			}
			g.setIgnoreSurvivors (!attackSurvivors);
		}
	}

	public Tile getTile(int x, int y) {
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

	public Tile getClosestTile(Vector2 pos) {
		return getTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
	}

	public Tile getClosestEmptyTile(Vector2 check) {
		int i = (int)Mathf.RoundToInt(check.x);
		int j = (int)Mathf.RoundToInt(check.y);
		Tile checkTile = getTile(i, j);
		if (checkTile != null && checkTile.isPassable()) {
			return checkTile;
		}
		else {
			for (int k = 1; k < (width > height ? width / 2 : height / 2); k++) {
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
		return emptyTileList[Random.Range(0, emptyTileList.Count)];
	}

	public Tile getRandomControlPoint() {
		return controlPointList[Random.Range(0, controlPointList.Count())];
	}

	public List<Guard> getZombieList() {
		return zombieList;
	}

	public List<Survivor> getEnemyList() {
		return enemyList;
	}

	public List<Survivor> getSurvivorList() {
		return survivorList;
	}

	public ControlPoint[] getControlPoints() {
		return controlPointList;
	}

	public void modifyZombieSpawnRate(float percent) {
		zombieSpawnInterval = (zombieSpawnInterval * baseZombieSpawnInterval) / (baseZombieSpawnInterval + percent * zombieSpawnInterval);
	}

	public void modifySurvivorSpawnRate(float percent) {
		survivorSpawnInterval = (survivorSpawnInterval * baseSurvivorSpawnInterval) / (baseSurvivorSpawnInterval + percent * survivorSpawnInterval);
	}

	public void removeZombie(Guard g) {
		zombieCtrl.removeZombie(g);
		zombieList.Remove(g);
		g.tile.removeZombie(g);
	}

	public void removeSurvivor(Survivor s) {
		enemyList.Remove(s);
		survivorList.Remove(s);
		if (s.getDestination() != null) {
			s.getDestination().removeIncomingSurvivor(s);
		}
		s.tile.removeSurvivor(s);
	}

	// return number of zombies in nxn area centered on (x, y)
	public int countZombiesInArea(int x, int y, int size) {
		Tile center = getTile(x, y);
		int count = 0;
		if (center != null) {
			foreach (Tile t in center.getNxNArea(size)) {
				if (t != null) {
					count += t.getZombieList().Count;
				}
			}
		}
		return count;
	}

	public int countSurvivorsInArea(int x, int y, int size) {
		Tile center = getTile(x, y);
		int count = 0;
		if (center != null) {
			foreach (Tile t in center.getNxNArea(size)) {
				if (t != null) {
					count += t.getSurvivorList().Count;
				}
			}
		}
		return count;
	}


	public void resetPathTiles() {
		foreach (Tile t in board) {
			t.dist = -1;
			t.crowdFactor = 0;
		}
	}

	#region pathfinding

	public List<Vector2> getPath(Tile start, Tile end, bool ignoreDoors) {
		List<Vector2> result = optimizePath(pathToPoints(getTilePath(start, end, ignoreDoors)));
		if (result.Count > 0) {
			result.RemoveAt(0);
		}
		return result;
//		return pathToPoints(getTilePath(start, end, ignoreDoors));
	}

	public List<Vector2> optimizePath(List<Vector2> path) {
		if (path.Count < 3) {
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
//		allPaths[0].RemoveAt(0);
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

	public List<Vector2> pathToPoints(List<Tile> path) {
		List<Vector2> points = new List<Vector2>();
		foreach (Tile tile in path) {
			points.Add(tile.transform.position);
		}
		return points;
	}

	public void setTargetTile(Tile targetTile) {
		List<Tile> queue = new List<Tile>();
		resetPathTiles();
		queue.Add(targetTile);
		targetTile.dist = 0;
		while (queue.Count > 0) {
			Tile currTile = queue[0];
			//don't question it...
			currTile.pathToTarget = new List<Vector2>();
			queue.RemoveAt(0);
			foreach (Tile neighbor in currTile.getNeighbors()) {
				if (neighbor.dist < 0 && neighbor.isPassable()) {
					neighbor.dist = currTile.dist + 1;
					queue.Add(neighbor);
				}
			}
		}
	}

	public List<Tile> findPathToTarget(Tile startTile) {
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

	public List<Tile> getTilePath(Tile startTile, Tile endTile, bool ignoreDoors) {
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

	public void spawnSurvivors(int n) {
		int numTiles = survivorSpawn.Count();
		int numSurvivorPoints = 0;
		ControlPoint[] survivorControlPoints = new ControlPoint[numControlPoints];
		bool ctrlPointOwned = false;
		foreach (ControlPoint cp in controlPointList) {
			if (cp.currentOwner == ControlPoint.Owner.Survivor) {
				survivorControlPoints[numSurvivorPoints] = cp;
				numSurvivorPoints++;
				ctrlPointOwned = true;
			}
		}

		for (int i = 0; i < n; i++) {
			Tile spawnTile;
			if (ctrlPointOwned && Random.Range(0f, 1f) >= .5f) {
				spawnTile = survivorControlPoints[Random.Range(0, numSurvivorPoints)];
			}
			else {
				spawnTile = survivorSpawn[Random.Range(0, numTiles)];
			}
			addSurvivor(spawnTile.posX, spawnTile.posY);
		}
	}


	public void spawnZombies(int n) {
		int numTiles = zombieSpawn.Count();
		for (int i = 0; i < n; i++) {
			Tile spawnTile = zombieSpawn[Random.Range(0, numTiles)];
			addGuard(spawnTile.posX, spawnTile.posY);
		}
	}

	Tile addTile(int x, int y, float fire, bool flammable) {
		GameObject tileObj = new GameObject();
		Tile tile = tileObj.AddComponent<Tile>();
		tile.init(x, y, this, fire, 0, true);
		tile.transform.localPosition = new Vector3(x, y, 0);
		tile.transform.parent = tileFolder.transform;
		emptyTileList.Add(tile);
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

	public void addSurvivor(int x, int y) {
		GameObject survObj = new GameObject();
		survObj.name = "Survivor";
		Survivor surv = survObj.AddComponent<Survivor>();
		surv.init(getTile(x, y), this, maxSurvivorPriority++);
		if (controlPointList[0] != null) {
			surv.setDestination(controlPointList[findQuadrant(getTile(x, y)) - 1]);
		}
		enemyList.Add(surv);
		survivorList.Add(surv);
		surv.transform.parent = survivorFolder.transform;
	}

	public void addTurret(int x, int y) {
		GameObject turrObj = new GameObject();
		turrObj.name = "Turret";
		Turret turr = turrObj.AddComponent<Turret>();
		turr.init(getTile(x, y), this, int.MaxValue);
		enemyList.Add(turr);
		turr.transform.parent = survivorFolder.transform;
	}
		
	// register each guard to be notified when a fan is toggled
	public void addGuard(int x, int y) {
		GameObject guardObj = new GameObject();
		guardObj.name = "Guard";
		Guard guard = guardObj.AddComponent<Guard>();
		guard.init(getTile(x, y), this, maxZombiePriority++);
		guard.transform.parent = zombieFolder.transform;
		zombieList.Add(guard);
	}

	#endregion
}

