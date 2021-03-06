﻿using UnityEngine;
using System.Collections;

public class Door : Tile {

	bool closed;
	BoxCollider2D coll;

	// Use this for initialization
	public void init(int x, int y, GameManager game) {
		base.init(x, y, game, 0, 0, false);
		rend.color = Color.black;
		coll = gameObject.AddComponent<BoxCollider2D>();
		coll.sharedMaterial = Resources.Load<PhysicsMaterial2D>("WallMaterial");
		gameObject.name = "Door";
		flammable = false;
		gameObject.layer = LayerMask.NameToLayer("Wall");
		//True is door is closed, False is door is open
		closed = false;
		toggle();
	}

	void OnMouseDown() {
		toggle();
	}

	public void toggle() {
		closed = !closed;
		if (closed) {
			rend.color = Color.cyan;
			//kill fire and gas
			fire = 0;
			gas = 0;
			col.a = 0;
//			gasRend.color = col;
			flammable = false;
			gameObject.layer = LayerMask.NameToLayer("Wall");
		}
		else {
			rend.color = new Color(0f, 0, 0f, 0.1f);
			flammable = true;
			gameObject.layer = LayerMask.NameToLayer("Default");
		}
		coll.isTrigger = !closed;
	}


	public override bool isPassable() {
		return !closed;
	}
}
