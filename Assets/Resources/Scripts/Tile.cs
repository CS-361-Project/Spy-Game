using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	int fireTimer;
	SpriteRenderer rend;
	GameManager game;
	bool flammable;
	bool fire;
	bool gas;
	int posX;
	int posY;

	// Use this for initialization
	public void init (int x, int y, GameManager game) {
		fireTimer = 0;
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
		if (fire) {
			fireTimer += 1;
		}
	}

	void checkForFire(){
		//if one of the neighbors is burning then start burning
		for (int i = posX - 1; i < posX + 1; i++) {
			for (int j = posY - 1; j < posY + 1; j++) {
				if (game.getTile(i, j).fire) {
					this.fire = true;
				}
			}
		}
	}
}
