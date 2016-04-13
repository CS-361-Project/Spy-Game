using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Frank : Person {
	SpriteRenderer rend;

	public enum behavior {
drinking,
		smoking,
		talking,
		puking}

	;

	Queue<int> toDoList;
	float clock;

	FrankIcon icon;

	// Use this for initialization
	public override void init(Tile t, GameManager man) {
		base.init(t, man);
		viewLayerMask = 1 << 8 | 1 << 10;

		speed = 1F;
		clock = 0; 

		toDoList = new Queue<int>();
		for (int i = 0; i < Enum.GetNames(typeof(behavior)).Length - 1; i++) {
			toDoList.Enqueue((int)UnityEngine.Random.Range(0, Enum.GetNames(typeof(behavior)).Length));
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

		gameObject.layer = LayerMask.NameToLayer("Frank");
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = new Vector3(size, size, 1);
		direction = new Vector2(1f, 0f);

		//targetPositions = gm.getPath(t, gm.getTile(7, 1));
		print(gm.getTile(3, 1).isPassable());
		targetPositions = new List<Vector2>();
		print(targetPositions.Count);
		//speed = 2f;

	}
	// Update is called once per frame
	void Update() {
		wander(false);
		move();
		clock += Time.deltaTime;
		lookAround();
		if (clock >= 1) {
			clock = 0;
			updateToDoList();

		}
	}

	void updateToDoList() {
		int behaviorCount = Enum.GetNames(typeof(behavior)).Length;
		int rand = (int)UnityEngine.Random.Range(0, behaviorCount);

		int nextTask = toDoList.Dequeue();

		Destroy(icon.gameObject);

		GameObject iconObj = new GameObject();
		iconObj.name = "Icon";
		icon = iconObj.AddComponent<FrankIcon>();
		icon.transform.parent = transform;
		icon.transform.localPosition = Vector3.up;

		if (nextTask == (int)behavior.drinking) {
			drinking();
		}
		if (nextTask == (int)behavior.puking) {
			puking();
		}
		if (nextTask == (int)behavior.smoking) {
			smoking();
		}
		if (nextTask == (int)behavior.talking) {
			talking();
		}

		toDoList.Enqueue(rand);
	}

	void lookAround() {
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 5)) {
			if (c != coll && c.gameObject.name != "Wall") {
				if (canSee(c.transform.position)) {
					print("Frank sees " + c.gameObject.name);
					shootAt(c.transform.position);
				}
			}
		}
	}

	void smoking() {
		icon.init((int)behavior.smoking);
	}

	void drinking() {
		icon.init((int)behavior.drinking);
	}

	void talking() {
		icon.init((int)behavior.talking);
	}

	void puking() {
		icon.init((int)behavior.puking);
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
		if (args.spilled) {
			rend.color = Color.green;
		}
		else {
			rend.color = Color.blue;
		}
	}
}
