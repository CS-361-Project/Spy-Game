using UnityEngine;
using System;
using System.Collections;

public class Fan : MonoBehaviour {
	bool state;
	Vector2 position;
	Vector2 velocity;
	SpriteRenderer rend;
	string direction;

	float viewportHeight, viewportWidth;
	Vector2 leftCorner, rightCorner;

	BoxCollider2D coll;

	// Class to encapsulate all data sent to subscribers when a fan event happens
	public class FanEventArgs : EventArgs {
		public Vector2 position { get; set; }
		public bool state { get; set; }
		public Vector2 velocity { get; set; }
	}

	// Steps for creating an event
	// 1. Define a delegate (can use an EventHandler<> or EventHandler which creates the delegate for you)
	// 2. Define an event based with the type being the delegate
	// 3. Raise the event

	// To subscribe to an event
	// Simply add the handler function to the publisher event
	// e.g. FanEnabled += SomeClass.OnFanEnabled;
	// SomeClass.OnFanEnabled must take same params as FanEnabled
	// SomeClass.OnFanEnabled(object source, EventArgs e)

	// Use this for initialization
	void Start () {
		state = false;
		position = transform.position;
		velocity = new Vector2(1, 0);
		coll = gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 1;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.red;

		gameObject.layer = LayerMask.NameToLayer("Room Objects");

		direction = "N";
		viewportHeight = 3F;
		viewportWidth = 3F;
		leftCorner = new Vector2(0,0);
		rightCorner = new Vector2(0,0);

		switch (direction) {
		case "E":
			leftCorner = new Vector2 (transform.position.x, transform.position.y + viewportWidth);
			rightCorner = new Vector2 (transform.position.x + viewportHeight, transform.position.y - viewportWidth);
			break;
		case "N":
			leftCorner = new Vector2 (transform.position.x - viewportWidth, transform.position.y);
			rightCorner = new Vector2 (transform.position.x + viewportWidth, transform.position.y + viewportHeight);
			break;
		case "W":
			leftCorner = new Vector2 (transform.position.x, transform.position.y - viewportWidth);
			rightCorner = new Vector2 (transform.position.x - viewportHeight, transform.position.y + viewportWidth);
			break;
		case "S":
			leftCorner = new Vector2 (transform.position.x + viewportWidth, transform.position.y);
			rightCorner = new Vector2 (transform.position.x - viewportWidth, transform.position.y - viewportHeight);
			break;
		default:
			print ("Not a valid direction. Must be N, E, S, or W.");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (state) {
			foreach (Collider2D c in Physics2D.OverlapAreaAll(leftCorner, rightCorner)) {
				if (c != coll && c.gameObject.name != "Wall") {
					float distance = Vector2.Distance ((Vector2)coll.transform.position, (Vector2)c.transform.position);
					float angle = Vector2.Angle ((Vector2)coll.transform.position, (Vector2)c.transform.position);
					c.attachedRigidbody.AddForce (new Vector2 ((1 / distance) * (Mathf.Cos (angle)), (1 / distance) * (Mathf.Cos (angle))));
				}
			}
		}
	}

	void OnMouseDown() {
		toggle();
	}

	public event EventHandler<FanEventArgs> FanToggled;

	public void setState(bool newState) {
		state = newState;
		onFanToggled();
	}

	public void toggle() {
		state = !state;
		if (state) {
			rend.color = Color.green;
		}
		else {
			rend.color = Color.red;
		}
		onFanToggled();
	}

	public virtual void onFanToggled() {
		if (FanToggled != null) {
			FanEventArgs args = new FanEventArgs();
			args.state = this.state;
			args.position = this.position;
			args.velocity = this.velocity;
			FanToggled(this, args);
		}
	}
}
