using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : MonoBehaviour {
	SpriteRenderer rend;
	Vector2 position;
	Vector2 direction;
	Vector2 lookingAt;

	Tile tile;
	GameManager gm;
	CircleCollider2D coll;
	Rigidbody2D body;

	float speed;
	float suspicion;

	AlertIcon alert;
	FOV fovDisplay;

	const float viewDistance = 2.5f;
	Tile startTile, endTile;
	int patrolDirection;
	List<Vector2> targetPositions;
	int currPosIndex;

	// Use this for initialization
	public void init(Tile t, GameManager m) {
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;
		tile = t;
		position = t.transform.position;

		coll = gameObject.AddComponent<CircleCollider2D>();
		body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
//		body.isKinematic = true;
		body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		body.constraints = RigidbodyConstraints2D.FreezeRotation;
		gameObject.layer = LayerMask.NameToLayer("Guard");
		coll.gameObject.layer = LayerMask.NameToLayer("Guard");
		Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
		//print(gameObject.layer);
		//gameObject.layer = 8;
		GameObject fovObj = new GameObject();
		fovObj.name = "FOV";
		fovObj.transform.parent = transform;
		fovDisplay = fovObj.AddComponent<FOV>();
		fovDisplay.init(viewDistance);

		tile = t;
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = new Vector3(0.7f, 0.7f, 1);
		direction = new Vector2(0, 1);
		lookingAt = new Vector2(0, 1);
		gm = m;

		suspicion = 0.0f;
		speed = 2f;
		startTile = t;
		endTile = m.getTile(4, 6);
		patrolDirection = 1;
		targetPositions = gm.getPath(startTile, endTile);
		foreach (Vector2 v in targetPositions) {
			print("(" + v.x + ", " + v.y + ")");
		}
//		targetPositions = new List<Vector2>();
//		targetPositions.Add(startTile.transform.position);
//		targetPositions.Add(endTile.transform.position);
	}
	
	// Update is called once per frame
	void Update() {
		//Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
		fovDisplay.setDirection(direction);
		if (suspicion >= 1f) {
			if (alert == null) {
				GameObject alertObj = new GameObject();
				alertObj.name = "Alert";
				alert = alertObj.AddComponent<AlertIcon>();
				alert.transform.parent = transform;
				alert.transform.localPosition = Vector3.up;
			}
			suspicion -= .01f;
			if (suspicion < 1f) {
				Destroy(alert.gameObject);
			}
		}
		if (patrolDirection == 1 && currPosIndex == targetPositions.Count - 1) {
			print("finished path in direction 1");
			targetPositions = gm.getPath(endTile, startTile);
			patrolDirection = -1;
			currPosIndex = 0;
		}
		else if (patrolDirection == -1 && currPosIndex == targetPositions.Count - 1) {
			print("finished path in direction -1");
			targetPositions = gm.getPath(startTile, endTile);
			patrolDirection = 1;
			currPosIndex = 0;
		}
//		Vector2 targetDirection = (targetPositions[currPosIndex + 1] - targetPositions[currPosIndex]).normalized;
//		direction = Vector2.Lerp(direction, targetDirection, .05f);
		direction = (targetPositions[currPosIndex + 1] - (Vector2)transform.position).normalized;
		Debug.DrawLine(transform.position, targetPositions[currPosIndex + 1]);

		Vector2 currDir = body.velocity.normalized;
		direction = Vector2.Lerp(currDir, direction, 0.22f);

		/*body.AddForce(direction * (0.5f * (1.0f-Vector2.Dot(direction,currDir)) + 0.5f));
		if (body.velocity.magnitude > 1f) {
			body.velocity = body.velocity.normalized * 1f;
		}*/

		body.velocity = direction * speed;
		tile = gm.getClosestTile(transform.position);
		if (Vector2.Distance((Vector2)transform.position, targetPositions[currPosIndex + 1]) <= .1) {
			print("Reached point " + currPosIndex + ", moving to point " + (currPosIndex + 1));
			currPosIndex++;
		}
		lookingAt = body.velocity.normalized;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, viewDistance)) {
			//TODO: Make sure it's not something boring like a wall
			if (c != coll && c.gameObject.name != "Wall") {
				if (canSee(c.transform.position)) {
					switch (c.gameObject.name) {
						case "Frank":
							suspicion = 2f;
							break;
						case "Chemical":
							if (c.gameObject.GetComponent<Chemical>().state) {
								suspicion += .25f;
							}
							break;
					}
				}
			}
		}
	}

	bool canSee(Vector3 pos) {
		Vector2 toObject = (pos - transform.position).normalized;
		float angle = Vector2.Dot(lookingAt, toObject);
		if (angle <= 1 && angle >= 0.866025404) { // cos 0 to cos 60
			RaycastHit2D rayHit = Physics2D.Raycast(transform.position, toObject, viewDistance, 1 << 9 | 1 << 10);
			if (rayHit.collider != null && rayHit.collider.transform.position == pos) {
				return true;
			}
		}
		return false;
	}

//	void OnTriggerEnter2D(Collider2D c) {
////		transform.position = (Vector2)transform.position - (direction * Time.deltaTime * speed); // get unstuck
//		direction *= -1;
//	}

	public virtual void onFanToggled(object source, Fan.FanEventArgs args) {
		if (canSee(args.position)) {
			suspicion += .5f;
		}
	}

	public virtual void onBurnerToggled(object source, Burner.BurnerEventArgs args) {
		if (args.state) {
			if (canSee(args.position)) {
				suspicion += .5f;
				// also probably want to go turn that off
			}
		}
	}

	public virtual void onChemicalToggled(object source, Chemical.ChemicalEventArgs args) {
		if (canSee(args.position)) {
			suspicion += .5f;
			print("that's supsicious");
		}
	}
}

