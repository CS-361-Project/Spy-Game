using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : MonoBehaviour {
	SpriteRenderer rend;
	Vector2 position;
	Vector2 direction;
	Vector2 lookingAt;
	float speed;
	Tile tile;
	GameManager gm;
	// Use this for initialization
	void init(Tile t, GameManager m) {
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;
		tile = t;
		position = t.transform.position;
		direction = new Vector2(1, 0);
		lookingAt = new Vector2(1, 0);
		speed = 2;
		gm = m;
	}
	
	// Update is called once per frame
	void Update() {
		position += direction * Time.deltaTime * speed;
		Tile nextTile = gm.getClosestTile(position);
		if (nextTile != tile) {
			if (nextTile.isPassable()) {
				tile = nextTile;
			}
			else {
				List<Tile> neighbors
			}
		}
	}

	public virtual void onFanToggled(object source, Fan.FanEventArgs args) {
		if (args.state) {
			rend.color = Color.red;
		}
		else {
			rend.color = Color.blue;
		}
	}

	public virtual void onBurnerToggled(object source, Burner.BurnerEventArgs args) {
		if (args.state) {
			rend.color = Color.yellow;
		}
		else {
			rend.color = Color.blue;
		}
	}

	public virtual void onChemicalToggled(object source, Chemical.ChemicalEventArgs args) {
		if (args.state) {
			rend.color = Color.green;
		}
		else {
			rend.color = Color.blue;
		}
	}
}

