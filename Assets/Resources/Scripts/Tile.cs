using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	float fireTimer;
	protected SpriteRenderer rend;
	//	protected SpriteRenderer gasRend;
//	protected SpriteRenderer fogRend;

	protected GameManager gm;
	protected bool flammable;
//	public bool visited;
	public bool visible;
	bool needToCheckVisibility;
	public float fire;
	public float gas;
	public bool passable;


	bool fanEffect;
	string fanDirec;
	int fanPosX;
	int fanPosY;

	public int posX;
	public int posY;
	public Color col;

	public bool containsLaser;

	float TimeBeforeSpread = 3f;

	public int section = -1;

	//Used for path finding
	public float dist = -1;
	public float crowdFactor = 0;

	List<Guard> zombies;
	List<Survivor> survivors;

	//ACCCK
	public List<Vector2> pathToTarget = new List<Vector2>();

	// Use this for initialization
	public void init(int x, int y, GameManager game, float fire, float gas, bool flammable) {
		fireTimer = 0;
		fanEffect = false;
		this.flammable = flammable;
		this.gas = gas;
		this.fire = fire;
		this.gm = game;
		this.posX = x;
		this.posY = y;
		passable = true;
		gameObject.name = "Tile";
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 0;
		rend.sprite = Resources.Load<Sprite>("Sprites/Box");

		if (flammable) {
			rend.color = Color.grey;
		}
		else {
			rend.color = Color.Lerp(Color.grey, Color.black, .5F);
		}

//		GameObject obj = new GameObject();
//		obj.transform.parent = transform;
//		obj.transform.localPosition = Vector3.zero;
//		gasRend = obj.AddComponent<SpriteRenderer>();
//		gasRend.sortingOrder = 1;
//		gasRend.sprite = Resources.Load<Sprite>("Sprites/Box");
//		col = Color.green;
//		col.a = 0f;
//		gasRend.color = col;
//
//		GameObject fogObj = new GameObject();
//		fogObj.transform.parent = transform;
//		fogObj.transform.localPosition = Vector3.zero;
//		fogObj.name = "Fog";
//		fogRend = fogObj.AddComponent<SpriteRenderer>();
//		fogRend.sprite = Resources.Load<Sprite>("Sprites/Box");
//		fogRend.color = Color.white;
//		fogRend.sortingLayerName = "Foreground";
//		fogRend.sortingOrder = 3;

		zombies = new List<Guard>();
		survivors = new List<Survivor>();

//		visited = false;
		containsLaser = false;
		visible = false;
		needToCheckVisibility = true;
	}

//	public void setColor() {
//		rend.color = Color.green;
//	}

	class IntDirection {
		public int x, y;
		public IntDirection(int x, int y) {
			this.x = x;
			this.y = y;
		}
		public void turnLeft() {
			int tmp = x;
			x = -y;
			y = tmp;
		}
		public IntDirection copy() {
			return new IntDirection(x, y);
		}
	}

	public float closestZombieDistance(int maxDistance) {
		IntDirection dir = new IntDirection(0, 1);
		float zombieDistance = maxDistance;
		bool foundZombie = false;
		Tile currTile = this;
		HashSet<Tile> visitedTiles = new HashSet<Tile>();
		while (!foundZombie && Mathf.Abs(currTile.posX - posX) <= maxDistance && Mathf.Abs(currTile.posY - posY) <= maxDistance) {
			ControlPoint cp = currTile as ControlPoint;
			if (currTile.getZombieList().Count > 0 || (cp != null && cp.currentOwner == ControlPoint.Owner.Zombie)) {
				foundZombie = true;
				zombieDistance = Vector2.Distance(new Vector2(posX, posY), new Vector2(currTile.posX, currTile.posY));
			}
			else {
				visitedTiles.Add(currTile);
				IntDirection left = dir.copy().turnLeft();
				Tile leftTile = gm.getTile(currTile.posX + left.x, currTile.posY + left.y);
				if (!visitedTiles.Contains(leftTile)) {
					currTile = leftTile;
					dir = left;
				}
				else {
					currTile = (gm.getTile(currTile.posX + dir.x, currTile.posY + dir.y));
				}
			}
		}
		return zombieDistance;
	}
	
	// Update is called once per frame
	public virtual void Update() {
		if (needToCheckVisibility) {
			int minCheck = 0;
			int maxCheck = Guard.tileViewDistance;
			bool foundZombie = false;
			foreach (Tile t in getNxNArea(Guard.tileViewDistance * 2)) {
				ControlPoint cp = t as ControlPoint;
				if (t.getZombieList().Count > 0 || (cp != null && cp.currentOwner == ControlPoint.Owner.Zombie)) {
					foundZombie = true;
					break;
				}
			}
			setVisibility(foundZombie);
//			setVisibility(true);
//			if (foundZombie && !visited) {
//				Destroy(fogRend.gameObject);
//				visited = true;
//			}
			needToCheckVisibility = false;
		}


//		if (flammable) {
//			checkForFire();
//		}
//		if (isPassable()) {
////			checkForGas();
////			col = Color.green;
////			//TODO we are capping ALPHA VALUE not GAS PER TILE come back to this later and think more
////			col.a = Mathf.Min(gas, 0.25f);
////			gasRend.color = col;
//			//TODO if a tile was previously inflammable and now has gas on it. That tile should become flammable. 
//		}
//		if (fire >= 1) {
//			fireTimer += Time.deltaTime;
//			rend.color = Color.red;
//			if (fireTimer > TimeBeforeSpread) {
//				fire = Mathf.Max(2, fire);
//
//			}
//		}
	}

	public virtual bool isPassable() {
		return passable;
	}

	public void setPassable(bool value){
		passable = value;
	}

//	public virtual void applyFanForce(string direc, int fanPosX, int fanPosY) {
//		fanEffect = true;
//		fanDirec = direc;
//		flammable = false;
//		this.fanPosX = fanPosX;
//		this.fanPosY = fanPosY;
//	}

//	public virtual void removeFanForce() {
//		fanEffect = false;
//		flammable = true;
//	}

	public Tile[] getNeighbors() {
		List<Tile> neighbors = new List<Tile>();
		for (int i = posX - 1; i <= posX + 1; i++) {
			for (int j = posY - 1; j <= posY + 1; j++) {
				if (i == posX ^ j == posY) {
					Tile t = gm.getTile(i, j);
					if (t != null) {
						neighbors.Add(t);
					}
				}
			}
		}
		//Remove all neighbors effected by fan
//		foreach(Tile neighbor in neighbors){
//			if (fanEffect) {
//				switch (fanDirec) {
//					case "E":
//						if (neighbor.posX < posX) {
//							neighbors.Remove(neighbor);
//						}
//						break;
//					case "S":
//						if (neighbor.posY > posY) {
//							neighbors.Remove(neighbor);
//						}
//						break;
//					case "W":
//						if (neighbor.posX > posX) {
//							neighbors.Remove(neighbor);
//						}
//						break;
//					case "N":
//						if (neighbor.posY < posY) {
//							neighbors.Remove(neighbor);
//						}
//						break;
//				}
//			}
//		}


		return neighbors.ToArray();
	}

	public Tile[] getNxNArea(int n) {
		List<Tile> result = new List<Tile>();;
		int i = 0;
		for (int x = posX - n/2; x <= posX + n/2; x++) {
			for (int y = posY - n/2; y <= posY + n/2; y++) {
				Tile t = gm.getTile(x, y);
				if (t != null) {
					result.Add(t);
				}
			}
		}
		return result.ToArray();
	}

	public Tile[] getNxNEmptyTiles(int n, bool includeThisTile) {
		List<Tile> result = new List<Tile>();;
		int i = 0;
		for (int x = posX - n/2; x <= posX + n/2; x++) {
			for (int y = posY - n/2; y <= posY + n/2; y++) {
				Tile t = gm.getTile(x, y);
				if (t != null && t.isPassable() && (!includeThisTile || t != this)) {
					result.Add(t);
				}
			}
		}
		return result.ToArray();
	}

//	void checkForFire() {
//		//if one of the neighbors is burning then start burning
//		Tile[] Neighbors = getNeighbors();
//		foreach (Tile neighbor in Neighbors) {
//			
//			if (neighbor.fire >= 2) {
//				if (gas >= 0) {
//					fire = Mathf.Max(fire, 1);
//					fire = Mathf.Min(Mathf.Max(fire, (gas * 10) * fire), 3);
//					gas = 0;
//				}
//				else {
//					fire = Mathf.Max(fire, 1);
//				}
//			}
//		}
//	}

	public void addZombie(Guard z) {
		if (zombies.Count == 0) {
			foreach (Tile t in getNxNArea(Guard.tileViewDistance * 2)) {
				t.checkVisibility();
			}
		}
		zombies.Add(z);
	}

	public void removeZombie(Guard z) {
		zombies.Remove(z);
		if (zombies.Count == 0) {
			foreach (Tile t in getNxNArea(Guard.tileViewDistance * 2)) {
				t.checkVisibility();
			}
		}
	}

	public List<Guard> getZombieList() {
		return zombies;
	}

	public void addSurvivor(Survivor s) {
		survivors.Add(s);
	}

	public void removeSurvivor(Survivor s) {
		survivors.Remove(s);
	}

	public List<Survivor> getSurvivorList() {
		return survivors;
	}

	public void checkVisibility() {
		needToCheckVisibility = true;
	}

	public virtual void setVisibility(bool visible) {
		if (!visible) {
			rend.sortingLayerName = "Foreground";
			rend.sortingOrder = 3;
			rend.color = Color.gray;
		}
		else {
			rend.sortingLayerName = "Default";
			rend.sortingOrder = 0;
			rend.color = new Color(.75f, .75f, .75f);
		}
	}
}
