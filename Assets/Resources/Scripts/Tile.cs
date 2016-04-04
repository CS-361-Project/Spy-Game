using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	float fireTimer;
	SpriteRenderer rend;
	GameManager game;
	bool flammable;
	float fire;
	float gas;
	int posX;
	int posY;

	float TimeBeforeSpread = 1;

	// Use this for initialization
	public void init (int x, int y, GameManager game, float fire, float gas, bool flammable) {
		fireTimer = 0;
		this.flammable = flammable;
		this.gas = gas;
		this.fire = fire;
		this.game = game;
		this.posX = x;
		this.posY = y;
		gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 0;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.grey;

	}
	
	// Update is called once per frame
	void Update () {
		if (flammable) {
			checkForFire();
		}
		if (fire == 1) {
			fireTimer += Time.deltaTime;
			rend.color = Color.red;
			if (fireTimer > TimeBeforeSpread) {
				fire = 2;

			}
		}
	}

	public bool isPassable() {
		return true;
	}

	public List<Tile> getNeighbors() {
		List<Tile> neighbors = new List<Tile>();
		for (int i = posX - 1; i <= posX + 1; i++) {
			for (int j = posY - 1; j <= posY + 1; j++) {
				neighbors.Add(game.getTile(i, j));
			}
		}
		return neighbors();
	}

	void checkForFire(){
		//if one of the neighbors is burning then start burning
		for (int i = posX - 1; i <= posX + 1; i++) {
			for (int j = posY - 1; j <= posY + 1; j++) {
				if ((i != posX || j != posY) && (i == posX || j == posY) && !(i==0&&j==0) && (i > -1 && i < game.width) && (j > -1 && j < game.height)) {
					if (game.getTile(i, j).fire>=2) {
						this.fire = 1;
					}
				}
			}
		}
	}
}
