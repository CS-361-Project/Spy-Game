using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Frank : MonoBehaviour {
	BoxCollider2D coll;
	GameManager gm;
	Tile currentTile;
	SpriteRenderer rend;
	bool onFire;

	public enum behavior {drinking, smoking, talking, puking};

	Queue<int> toDoList;

	Vector2 position;
	Vector2 direction;
	Vector2 lineOfSight;
	float speed;
	float clock;

	FrankIcon icon;

	// Use this for initialization
	public void init (Tile t, GameManager man) {
		
		gm = man;
		speed = 1F;
		clock = 0; 

		toDoList = new Queue<int> ();
		for (int i = 0; i < Enum.GetNames (typeof(behavior)).Length - 1; i++) {
			toDoList.Enqueue ((int)UnityEngine.Random.Range (0, Enum.GetNames (typeof(behavior)).Length));
		}

		GameObject iconObj = new GameObject();
		iconObj.name = "Icon";
		icon = iconObj.AddComponent<FrankIcon>();
		icon.transform.parent = transform;
		icon.transform.localPosition = Vector3.up;

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.magenta;
		rend.sortingOrder = 1;

		onFire = false;

		coll = gameObject.AddComponent<BoxCollider2D> ();
		Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		body.isKinematic = true;
		gameObject.layer = LayerMask.NameToLayer("Frank");

		currentTile = t;
		position = currentTile.transform.position;
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = new Vector3(0.7f, 0.7f, 1);
		direction = new Vector2 (1f, 0f);
		lineOfSight = direction;

	}
	// Update is called once per frame
	void Update () {
		clock += Time.deltaTime;
		Wander ();
		lookAround();
		if (clock >= 1) {
			clock = 0;
			updateToDoList ();

		}

	}

	void updateToDoList(){
		int behaviorCount = Enum.GetNames(typeof(behavior)).Length;
		int rand = (int)UnityEngine.Random.Range (0, behaviorCount);

		int nextTask = toDoList.Dequeue ();

		Destroy(icon.gameObject);

		GameObject iconObj = new GameObject();
		iconObj.name = "Icon";
		icon = iconObj.AddComponent<FrankIcon>();
		icon.transform.parent = transform;
		icon.transform.localPosition = Vector3.up;

		if (nextTask == (int)behavior.drinking) {
			drinking ();
		}
		if (nextTask == (int)behavior.puking) {
			puking ();
		}
		if (nextTask == (int)behavior.smoking) {
			smoking ();
		}
		if (nextTask == (int)behavior.talking) {
			talking ();
		}

		toDoList.Enqueue (rand);
	}

	void Wander(){
		transform.position = (Vector2)transform.position + (direction * Time.deltaTime * speed);
	}

	void lookAround(){
		currentTile = gm.getClosestTile(transform.position);	
		lineOfSight = direction;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 5)) {
			//TODO: Make sure it's not something boring like a wall
			if (c != coll && c.gameObject.name != "Wall") {
				Vector2 toObject = (c.transform.position - transform.position).normalized;
				float angle = Vector2.Dot(lineOfSight, toObject);
				if (angle <= 1 && angle >= 0.866025404) { // 0 to 60
					if (Physics2D.Raycast(transform.position, toObject, 10, 1 << 8).collider == c) {
						print("Frank sees an object: " + c.gameObject.name);
						Debug.DrawLine(c.transform.position, transform.position, new Color(angle / 2.0f + .5f, angle / 2f + .5f, angle / 2f + .5f));
						// found object code
					}
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		//		transform.position = (Vector2)transform.position - (direction * Time.deltaTime * speed); // get unstuck
		//		float sin = Mathf.Sin(Mathf.PI / 2);
		//		float cos = Mathf.Cos(Mathf.PI / 2);
		direction *= -1;
	}

	void smoking(){
		icon.init ((int)behavior.smoking);
	}
	void drinking(){
		icon.init ((int)behavior.drinking);
	}
	void talking(){
		icon.init ((int)behavior.talking);
	}
	void puking(){
		icon.init ((int)behavior.puking);
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
