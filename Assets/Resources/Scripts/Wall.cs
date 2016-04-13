using UnityEngine;
using System.Collections;

public class Wall : Tile {

	public override bool isPassable() {
		return false;
	}
	public void init(int x, int y, GameManager game) {
		base.init(x, y, game, 0, 0, false);
		rend.color = Color.black;
		coll = gameObject.AddComponent<BoxCollider2D>();
		coll.sharedMaterial = Resources.Load<PhysicsMaterial2D>("WallMaterial");
		gameObject.name = "Wall";
		flammable = false;
		gameObject.layer = LayerMask.NameToLayer("Wall");
	}

	public override void applyFanForce(string direc, int fanPosX, int fanPosY) {
	}

	public override void removeFanForce() {
	}
}

