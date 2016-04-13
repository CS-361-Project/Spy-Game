using UnityEngine;
using System;
using System.Collections;

public class Burner : LevelObject {
	bool state;
	Vector2 position;
	//Vector2 velocity;
	SpriteRenderer rend;
	Tile location;

	// Class to encapsulate all data sent to subscribers when a burner event happens
	public class BurnerEventArgs : EventArgs {
		public Vector2 position { get; set; }
		public bool state { get; set; }
		//public Vector2 velocity { get; set; }
	}


	public void init (Tile loc) {
		this.location = loc;
		state = false;
		position = transform.position;
		coll = gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 1;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.yellow;

		gameObject.layer = LayerMask.NameToLayer("Room Objects");
	}

	void Update () {
		if (state) {
			location.setGas(.1f);
		}
	}

	void OnMouseDown() {
		toggle();
	}

	public event EventHandler<BurnerEventArgs> BurnerToggled;


	public void setState(bool newState) {
		state = newState;
		onBurnerToggled();
	}

	public void toggle() {
		state = !state;
		if (state) {
			rend.color = Color.blue;
		}
		else {
			rend.color = Color.yellow;
		}
		onBurnerToggled();
	}


	public virtual void onBurnerToggled() {
		if (BurnerToggled != null) {
			BurnerEventArgs args = new BurnerEventArgs();
			args.state = this.state;
			args.position = this.position;
			BurnerToggled(this, args);
		}
	}

}
