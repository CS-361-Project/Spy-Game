using UnityEngine;
using System.Collections;

public class Wall : Tile {

	public override bool isPassable() {
		return false;
	}
	public void init(int x, int y, GameManager game) {
		base.init(x, y, game, 0, 0, false);
		rend.color = Color.black;
		gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
		gameObject.name = "Wall";
		flammable = false;
	}
}

