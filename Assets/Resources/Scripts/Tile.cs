using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	float fireTimer;
	protected SpriteRenderer rend;
	protected SpriteRenderer gasRend;

	GameManager game;
	protected bool flammable;
	public float fire;
	public float gas;

	bool fanEffect;
	string fanDirec;

	public int posX;
	public int posY;
	public Color col;

	float TimeBeforeSpread = 1.5f;

	//Used for path finding
	public int dist = -1;

	// Use this for initialization
	public void init (int x, int y, GameManager game, float fire, float gas, bool flammable) {
		fireTimer = 0;
		fanEffect = false;
		this.flammable = flammable;
		this.gas = gas;
		this.fire = fire;
		this.game = game;
		this.posX = x;
		this.posY = y;
		gameObject.name = "Tile";
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 0;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.grey;

		GameObject obj = new GameObject();
		obj.transform.parent = transform;
		obj.transform.localPosition = Vector3.zero;
		gasRend = obj.AddComponent<SpriteRenderer>();
		gasRend.sortingOrder = 1;
		gasRend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		col = Color.green;
		col.a = 0f;
		gasRend.color = col;

	}
	
	// Update is called once per frame
	public void Update () {
		if (flammable) {
			checkForFire();
		}
		if (isPassable()) {
			checkForGas();
			col = Color.green;
			//TODO we are capping ALPHA VALUE not GAS PER TILE come back to this later and think more
			col.a = Mathf.Min(gas, 0.25f);
			gasRend.color = col;
			//TODO if a tile was previously inflammable and now has gas on it. That tile should become flammable. 
		}
		if (fire >= 1) {
			fireTimer += Time.deltaTime;
			rend.color = Color.red;
			if (fireTimer > TimeBeforeSpread) {
				fire = Mathf.Max(2,fire);

			}
		}
	}

	public virtual bool isPassable() {
		return true;
	}

	public void applyFanForce(string direc){
		fanEffect = true;
		fanDirec = direc;
		flammable = false;
	}

	public void unApplyFanForce(){
		fanEffect = false;
		flammable = true;
	}

	public Tile[] getNeighbors() {
		List<Tile> neighbors = new List<Tile>();
		for (int i = posX - 1; i <= posX + 1; i++) {
			for (int j = posY - 1; j <= posY + 1; j++) {
				if (i == posX ^ j == posY) {
					Tile t = game.getTile(i, j);
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

	void checkForFire(){
		//if one of the neighbors is burning then start burning
		Tile [] Neighbors =getNeighbors();
		foreach (Tile neighbor in Neighbors) {
			
			if (neighbor.fire >= 2){
				if (gas >= 0) {
					fire = Mathf.Max(fire, 1);
					fire = Mathf.Min(Mathf.Max(fire, (gas*10) * fire), 3);
					gas = 0;
				}
				else {
					fire = Mathf.Max(fire, 1);
				}
			}
		}
	}

	void checkForGas(){
		//steal gas
		float numToDonate = 0;
		foreach (Tile neighbor in getNeighbors()) {
			if (neighbor.gas < gas && neighbor.isPassable()) {
				float amt = (gas - neighbor.gas);
				neighbor.gas += amt * Time.deltaTime;
				gas -= amt * Time.deltaTime;
			}
		}
	}

	public void setGas(float gas){
		this.gas += gas;

	}

	public void setFire(float fire){
		this.fire += fire;

	}

}
