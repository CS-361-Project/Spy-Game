using UnityEngine;
using System;
using System.Collections;

public class Fan : MonoBehaviour {
	bool state;
	Vector2 position;
	Vector2 velocity;
	SpriteRenderer rend;
	float direction;

	float viewportLength, viewportWidth;

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
		gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 1;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.red;

		gameObject.layer = LayerMask.NameToLayer("Room Objects");

		direction = Mathf.PI/2F;
		viewportHeight = 3F;
		viewportWidth = 3F;
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 leftCorner = new Vector2 (transform.position.x - (viewportWidth * Mathf.Cos (direction+(Mathf.PI/2F))), transform.position.y + (viewportWidth * Mathf.Sin (direction+(Mathf.PI/2F))));
		Vector2 rightCorner = new Vector2 (transform.position.x + (viewportWidth * Mathf.Cos (direction+(Mathf.PI/2F))), transform.position.y + (viewportLength * Mathf.Sin (direction)));
		foreach (Collider2D c in Physics2D.OverlapAreaAll(

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
