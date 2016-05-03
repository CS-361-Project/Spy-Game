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

	List<Zombie> zombies;
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

		zombies = new List<Zombie>();
		survivors = new List<Survivor>();

		containsLaser = false;
		visible = false;
		needToCheckVisibility = true;
	}
	
	// Update is called once per frame
	public virtual void Update() {
		if (needToCheckVisibility) {
			bool foundZombie = false;
			foreach (Tile t in getNxNArea(Zombie.tileViewDistance * 2)) {
				ControlPoint cp = t as ControlPoint;
				if (t.getZombieList().Count > 0 || (cp != null && cp.currentOwner == ControlPoint.Owner.Zombie)) {
					foundZombie = true;
					break;
				}
			}
			setVisibility(foundZombie);
			needToCheckVisibility = false;
		}
	}

	public virtual bool isPassable() {
		return passable;
	}

	public void setPassable(bool value){
		passable = value;
	}

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

	public void addZombie(Zombie z) {
		if (zombies.Count == 0) {
			foreach (Tile t in getNxNArea(Zombie.tileViewDistance * 2)) {
				t.checkVisibility();
			}
		}
		zombies.Add(z);
	}

	public void removeZombie(Zombie z) {
		zombies.Remove(z);
		if (zombies.Count == 0) {
			foreach (Tile t in getNxNArea(Zombie.tileViewDistance * 2)) {
				t.checkVisibility();
			}
		}
	}

	public List<Zombie> getZombieList() {
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
