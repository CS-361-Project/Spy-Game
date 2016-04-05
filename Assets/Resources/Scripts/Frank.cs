using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Frank : MonoBehaviour {
	SpriteRenderer rend;
	bool onFire;
	enum behavior {smoking, drinking, talking, puking, shooting, running};
	Queue toDoList;
	Behavior[] behaviorList;

	Vector2 position;
	Vector2 direction;
	Vector2 lineOfSight;
	float speed;

	BoxCollider2D coll;
	GameManager gm;
	Tile currentTile;

	float clock;


	// Use this for initialization
	public void init (Tile t, GameManager man) {
		
		gm = man;
		speed = 1F;
		clock = 0; 

		toDoList = new Queue ();
		behaviorList[Enum.GetNames(typeof(behavior)).Length];

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.magenta;
		rend.sortingOrder = 1;

		onFire = false;

		coll = gameObject.AddComponent<BoxCollider2D> ();
		Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		body.isKinematic = true;

		currentTile = t;
		position = currentTile.transform.position;
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		direction = new Vector2 (1f, 0f);
		lineOfSight = direction;

	}
	// Update is called once per frame
	void Update () {
		clock += Time.deltaTime;
		Wander ();
		lookAround();
		if (clock >= 10) {
			clock = 0;
			print ("sam");
			//updateToDoList ();

		}

	}

	void updateToDoList(){
		int behaviorCount = Enum.GetNames(typeof(behavior)).Length;
		toDoList.Dequeue ();
		int rand = (int)UnityEngine.Random.Range (0, behaviorCount - 1);
		toDoList.Enqueue (behaviorList [rand]);

	}

	void Wander(){
		transform.position = (Vector2)transform.position + (direction * Time.deltaTime * speed);
	}

	void lookAround(){
		currentTile = gm.getClosestTile(transform.position);	
		lineOfSight = direction;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 5)) {
			//TODO: Make sure it's not something boring like a wall
			if (c != coll) {
				Vector2 toObject = (c.transform.position - transform.position).normalized;
				float angle = Vector2.Dot(lineOfSight, toObject);
				if (angle <= 0.86602540378 || angle >= -0.86602540378) { // -30 to 30 degrees
					if (Physics2D.Raycast(transform.position, c.transform.position - transform.position).collider == c) {
						//  direction = toObject;
						print("I see an object: " + c.gameObject.name);
						// found object code
					}
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		print("Collided with: " + c.gameObject.name);
		direction *= -1;
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
