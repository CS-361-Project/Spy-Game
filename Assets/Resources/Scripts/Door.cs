using UnityEngine;
using System.Collections;

public class Door : Tile {

	bool state;

	// Use this for initialization
	public void init(int x, int y, GameManager game) {
		base.init(x, y, game, 0, 0, false);
		rend.color = Color.black;
		BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
		coll.sharedMaterial = Resources.Load<PhysicsMaterial2D>("WallMaterial");
		gameObject.name = "Door";
		flammable = false;
		gameObject.layer = LayerMask.NameToLayer("Room Objects");
		//True is door is closed, False is door is open
		state = true;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update();
	}

	void OnMouseDown() {
		toggle();
	}

	public void toggle() {
		state = !state;
		if (state) {
			rend.color = Color.black;
			//kill fire and gas
			fire = 0;
			gas = 0;
			col.a = 0;
			gasRend.color = col;
			flammable = false;

		}
		else {
			rend.color = Color.grey;
			flammable = true;
		}
	}


	public override bool isPassable() {
		return !state;
	}
}
