using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Frank : MonoBehaviour {
	BoxCollider2D coll;
	GameManager gm;
	Tile currentTile;
	SpriteRenderer rend;
	Rigidbody2D body;

	bool onFire;
	bool isDrunk;
	bool isSmoking;
	bool isTalking;
	bool isPuking;

	public enum behavior {drinking, smoking, talking, puking};

	Queue<int> toDoList;

	Vector2 position;
	Vector2 direction;
	Vector2 lineOfSight;
	float speed;
	float clock;
	FrankIcon icon;

	Tile startTile, endTile;
	int patrolDirection;
	List<Vector2> targetPositions;
	int currPosIndex;

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

		updateToDoList ();

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.magenta;
		rend.sortingOrder = 1;

		onFire = false;
		isDrunk = false;

		coll = gameObject.AddComponent<BoxCollider2D> ();
		body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
//		body.isKinematic = true;
		gameObject.layer = LayerMask.NameToLayer("Frank");

		currentTile = t;
		position = currentTile.transform.position;
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = new Vector3(0.7f, 0.7f, 1);
		direction = new Vector2 (1f, 0f);
		lineOfSight = direction;

		startTile = t;
		endTile = gm.getTile(t.posX + 2, t.posY + 3);
		patrolDirection = 1;
		//		targetPositions = gm.getPath(startTile, endTile);
		targetPositions = new List<Vector2>();
		targetPositions.Add(startTile.transform.position);
		targetPositions.Add(endTile.transform.position);

	}
	// Update is called once per frame
	void Update () {
		clock += Time.deltaTime;
		if (clock >= 1) {
			clock = 0;
			updateToDoList ();
		}

		Wander ();
		lookAround();


	}

	void updateToDoList(){
		if (isSmoking) {
			throwCigarette ();
			isSmoking = false;
		}
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
		if (!isDrunk) {
			speed = 1.0F;
		} else {
			/*float rand = UnityEngine.Random.Range (0, 10);
			if (rand < 2.5F) {
				direction = new Vector2 (1F, 0);
			} else if (rand < 5F) {
				direction = new Vector2 (0, 1F);
			} else if (rand < 7.5F) {
				direction = new Vector2 (0, -1F);
			} else if (rand < 10F) {
				direction = new Vector2 (-1F, 0);
			}*/
			speed = 10F;
		}
		//transform.position = (Vector2)transform.position + (direction * Time.deltaTime * speed);
		if (patrolDirection == 1 && currPosIndex == targetPositions.Count - 1) {
			print("finished path in direction 1");
			//			targetPositions = gm.getPath(endTile, startTile);
			targetPositions = new List<Vector2>();
			targetPositions.Add(endTile.transform.position);
			targetPositions.Add(startTile.transform.position);
			patrolDirection = -1;
			currPosIndex = 0;
		}
		else if (patrolDirection == -1 && currPosIndex == targetPositions.Count - 1) {
			print("finished path in direction -1");
			//			targetPositions = gm.getPath(startTile, endTile);
			targetPositions = new List<Vector2>();
			targetPositions.Add(startTile.transform.position);
			targetPositions.Add(endTile.transform.position);
			patrolDirection = 1;
			currPosIndex = 0;
		}
		Vector2 targetDirection = (targetPositions[currPosIndex + 1] - targetPositions[currPosIndex]).normalized;
		direction = Vector2.Lerp(direction, targetDirection, .05f);

		body.velocity = direction * speed;
		currentTile = gm.getClosestTile(transform.position);
		if (gm.getClosestTile(targetPositions[currPosIndex + 1]) == currentTile) {
			currPosIndex++;
		}
		lineOfSight = direction.normalized;
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
		direction *= -1;
	}

	void smoking(){
		icon.init ((int)behavior.smoking);
		Tile location = gm.getClosestTile (transform.position);
		/*if (location.gas > .07f) {
			location.fire = Mathf.Max(2, location.fire);
		}*/
		isSmoking = true; 
	}

	void throwCigarette(){
		isSmoking = false;
		Vector2 radius = new Vector2 (transform.position.x + UnityEngine.Random.Range (-5F, 5F), transform.position.y + UnityEngine.Random.Range (-2F, 2F));
		Tile cigTile = gm.getClosestTile(radius);
		if (cigTile.isFlammable()){
			cigTile.setFire(1F);
		}
	}

	void drinking(){
		isDrunk = true;
		icon.init ((int)behavior.drinking);
	}

	void talking(){
		icon.init ((int)behavior.talking);
	}

	void puking(){
		isDrunk = false;
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
