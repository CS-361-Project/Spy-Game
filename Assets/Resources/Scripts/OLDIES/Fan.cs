﻿using UnityEngine;
using System;
using System.Collections;

public class Fan : MonoBehaviour {
	
	Vector2 position;
	Vector2 velocity;
	SpriteRenderer rend;
	public string direction;
	public bool state;
	GameManager game;
	int posX;
	int posY;

	public float viewportHeight, viewportWidth;
	Vector2 leftCorner, rightCorner;

	BoxCollider2D coll;

	public event EventHandler<FanEventArgs> FanToggled;

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
	public void init(float posX, float posY, string dir, GameManager game) {
		this.posX = (int) posX;
		this.posY = (int) posY;
		this.game = game;
		state = false;
		position = transform.position;
		velocity = new Vector2(1, 0);
		coll = gameObject.AddComponent<BoxCollider2D>();
		coll.isTrigger = true;
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 1;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.red;

		gameObject.layer = LayerMask.NameToLayer("Room Objects");

		direction = dir;
		viewportHeight = 1F;
		viewportWidth = 5F;
		leftCorner = new Vector2(0,0);
		rightCorner = new Vector2(0,0);


		//TODO: correct all box drawings
		switch (direction) {
			case "E":
				leftCorner = new Vector2(transform.position.x, transform.position.y - viewportHeight / 2);
				rightCorner = new Vector2(transform.position.x + viewportWidth, transform.position.y + viewportHeight / 2);
				break;
			case "N":
				leftCorner = new Vector2(transform.position.x - viewportWidth, transform.position.y);
				rightCorner = new Vector2(transform.position.x + viewportWidth, transform.position.y + viewportHeight);
				break;
			case "W":
				leftCorner = new Vector2(transform.position.x, transform.position.y - viewportWidth);
				rightCorner = new Vector2(transform.position.x - viewportHeight, transform.position.y + viewportWidth);
				break;
			case "S":
				leftCorner = new Vector2(transform.position.x + viewportWidth, transform.position.y);
				rightCorner = new Vector2(transform.position.x - viewportWidth, transform.position.y - viewportHeight);
				break;
			default:
				print("Not a valid direction. Must be N, E, S, or W.");
				break;
		}
	}
	
	// Update is called once per frame
//	void Update () {
//		if (state) {
//			foreach (Collider2D c in Physics2D.OverlapAreaAll(leftCorner, rightCorner)) {
//				// change viewportwidth part of this for when not east
//				RaycastHit2D ray = Physics2D.Raycast(transform.position, velocity.normalized, viewportWidth, LayerMask.NameToLayer("Wall"));
//				if (ray.collider == null || ray.collider == c) {
//					Person p = c.gameObject.GetComponent<Person>();
//					if (p != null && c != coll) {
//						float distance = Vector2.Distance((Vector2)coll.transform.position, (Vector2)c.transform.position);
////					float angle = Vector2.Angle ((Vector2)coll.transform.position, (Vector2)c.transform.position);
//						Debug.DrawLine(transform.position, c.transform.position);
//						p.applyFanForce(10 / distance * velocity);
////					c.attachedRigidbody.AddForce (new Vector2 ((1 / distance) * (Mathf.Cos (angle)), (1 / distance) * (Mathf.Cos (angle))));
//					}
//				}
//			}
			//TODO: Tell all Tiles in box to 
			//tile.applyFanForce(direction);
//			switch (direction) {
//			case "E":
//				for (int i = posX + 1; i < posX + 1 + viewportWidth; i++) {
//					game.getTile(i, posY).applyFanForce("E", posX,posY);
//				}
//				break;
//			case "N":
//				for (int i = posX; i < viewportWidth; i++) {
//					game.getTile(i, posY).applyFanForce("N",posX,posY);
//				}
//				break;
//			case "W":
//				leftCorner = new Vector2(transform.position.x, transform.position.y - viewportWidth);
//				rightCorner = new Vector2(transform.position.x - viewportHeight, transform.position.y + viewportWidth);
//				break;
//			case "S":
//				leftCorner = new Vector2(transform.position.x + viewportWidth, transform.position.y);
//				rightCorner = new Vector2(transform.position.x - viewportWidth, transform.position.y - viewportHeight);
//				break;
//			default:
//				print("Not a valid direction. Must be N, E, S, or W.");
//				break;
//			}


//		}
//		else
//		if (!state) {
//			switch (direction) {
//				case "E":
//					for (int i = posX + 1; i < posX + 1 + viewportWidth; i++) {
//						game.getTile(i, posY).removeFanForce();
//					}
//					break;
//				case "N":
//					for (int i = posX; i < viewportWidth; i++) {
//						game.getTile(i, posY).removeFanForce();
//					}
//					break;
//				case "W":
//					leftCorner = new Vector2(transform.position.x, transform.position.y - viewportWidth);
//					rightCorner = new Vector2(transform.position.x - viewportHeight, transform.position.y + viewportWidth);
//					break;
//				case "S":
//					leftCorner = new Vector2(transform.position.x + viewportWidth, transform.position.y);
//					rightCorner = new Vector2(transform.position.x - viewportWidth, transform.position.y - viewportHeight);
//					break;
//				default:
//					print("Not a valid direction. Must be N, E, S, or W.");
//					break;
//			}
//		}
//	}

	void OnMouseDown() {
		toggle();
	}

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
