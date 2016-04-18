using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : Person {
	public static int tileViewDistance = 4;
	SpriteRenderer rend;

	float suspicion;
	int health;

	AlertIcon alert;

	Tile startTile, endTile;
	[SerializeField]
	int patrolDirection;

	public bool selected;

	Color baseColor = Color.magenta;
	Color selectionColor = new Color(.4f, .85f, 1f);

	float actionClock;
	bool commandPressed;


	// Use this for initialization
	public override void init(Tile t, GameManager m) {
		base.init(t, m);
		t.addZombie(this);
		viewLayerMask = 1 << 9 | 1 << 10;

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Box");
		rend.color = baseColor;
		rend.sortingOrder = 1;

		gameObject.layer = LayerMask.NameToLayer("Guard");
		//gameObject.transform.localScale = new Vector3(.4f, .4f, 1);

		/*GameObject fovObj = new GameObject();
		fovObj.name = "FOV";
		fovObj.transform.parent = transform;
		fovObj.transform.localScale = new Vector3(1 / .7f, 1 / .7f, 1);
		fovDisplay = fovObj.AddComponent<FOV>();
		fovDisplay.init(viewDistance);*/

		suspicion = 0.0f;
		actionClock = 0.0f;
		commandPressed = false;

		startTile = t;
		//endTile = m.getTile(4, 6);
		patrolDirection = 3;
//		targetPositions = gm.getPath(tile, endTile);
		targetPositions = new List<Vector2>();
		//Debug.DrawLine(tile.transform.position + new Vector3(-.5f, .5f, 0), tile.transform.position + new Vector3(.5f, -.5f, 0));
		//Debug.DrawLine(endTile.transform.position + new Vector3(-.5f, .5f, 0), endTile.transform.position + new Vector3(.5f, -.5f, 0));

		speed = 2f;
		health = 100;
	}
	
	// Update is called once per frame
	void Update() {
		actionClock += Time.deltaTime;


		/*Vector2 lastPos = tile.transform.position;

		foreach (Vector2 position in targetPositions) {
			Debug.DrawLine(lastPos, position);
			lastPos = position;
		}*/

		//fovDisplay.setDirection(direction);
//		if (suspicion >= 1f) {
//			if (alert == null) {
//				GameObject alertObj = new GameObject();
//				alertObj.name = "Alert";
//				alert = alertObj.AddComponent<AlertIcon>();
//				alert.transform.parent = transform;
//				alert.transform.localPosition = Vector3.up;
//			}
//			suspicion -= .01f;
//			if (suspicion < 1f) {
//				Destroy(alert.gameObject);
//			}
//		}
//		if (targetPositions.Count > 0) {
//			print("Count: " + targetPositions.Count);
//		}
		if (patrolDirection == 2) {
			patrolDirection = 0;
		}

		/*foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, viewDistance)) {
			if (c != coll && c.gameObject.name != "Wall") {
				if (Vector2.Distance(c.transform.position, transform.position) <= viewDistance / 2 || canSee(c.transform.position)) {
					switch (c.gameObject.name) {
						case "Frank":
							print("Guard sees Frank");
							suspicion = 2f;
							targetPositions = gm.getPath(tile, gm.getClosestTile(c.transform.position), false);
							if (targetPositions.Count >= 2) {
								targetPositions.RemoveAt(targetPositions.Count - 1);
								targetPositions.RemoveAt(0);
							}
							targetPositions.Add(c.transform.position);
							patrolDirection = 2;
							break;
						case "Chemical":
							if (c.gameObject.GetComponent<Chemical>().spilled) {
								suspicion += .25f;
							}
							break;
					}
				}
			}
		}*/
		if (patrolDirection == 1) {
			if (targetPositions.Count <= 0) {
				targetPositions = gm.getPath(endTile, startTile, false);
				patrolDirection = -1;
			}
		}
		else if (patrolDirection == -1) {
			if (targetPositions.Count <= 0) {
				targetPositions = gm.getPath(startTile, endTile, false);
				patrolDirection = 1;
			}
		}
		else if (patrolDirection == 0 || patrolDirection == 3) {
			wander(true);
		}

		Survivor closestSurvivor = null;
		float minDist = float.MaxValue;

		if (actionClock > 5F || (!commandPressed)) {
			commandPressed = false;
			foreach (Survivor s in gm.getSurvivorList()) {
				float dist = Vector2.Distance (s.transform.position, this.transform.position);
				if (dist < viewDistance && dist < minDist) {
					Vector2 toObject = s.transform.position - transform.position;
					RaycastHit2D hit = Physics2D.Raycast (transform.position, toObject.normalized, dist, 1 << LayerMask.NameToLayer ("Wall"));
					if (hit.collider == null) {
						closestSurvivor = s;
						minDist = dist;
					}
				}
			}
		}
		if (closestSurvivor != null) {
			targetPositions.InsertRange(0, gm.getPath(tile, closestSurvivor.tile, false));
		}

		Tile oldTile = tile;
		bool changedTile = move();
		if (changedTile) {
			oldTile.removeZombie(this);
			tile.addZombie(this);
		}

		Vector3 sumForce = Vector3.zero;
		int neighborCount = 0;
		foreach (Tile t in tile.getNxNArea(3)) {
			if (t != null) {
				foreach (Guard g in t.getZombieList()) {
					if (g != this) {
						float dist = Vector2.Distance(g.transform.position, transform.position);
						if (dist <= 0.45) {
							sumForce += -10f * (g.transform.position - transform.position).normalized *
							radius / Mathf.Pow((Mathf.Max(Mathf.Min(dist, radius), .1f)), 2);
							neighborCount++;
						}
					}
				}
			}
		}
		if (neighborCount > 0) {
			if (sumForce == Vector3.zero) {
				sumForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			}
			sumForce = sumForce.normalized * Mathf.Min(sumForce.magnitude / (float)neighborCount, 37);
			body.AddForce(sumForce);
		}
		//body.velocity = body.velocity.normalized * speed;
	}

	public void startTimer(){
		actionClock = 0f;
		commandPressed = true;
	}

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

	public virtual void onMotionDetected(object source, LaserSensor.LaserEventArgs args) {
		// TODO: System if reached source of motion and haven't seen frank, ignore that sensor for x seconds
		// sort of a way of saying "all clear"
		// actually would be good to send message to all other guards letting them know there's nothing to see there
//		print("Moving to position " + args.position);
		Tile t = gm.getClosestTile(args.position);
		List<Vector2> path = gm.getPath(tile, t, true);
		if (path.Count <= 15) {
			suspicion = 2f;
//			print("Path from " + tile.transform.position + " to " + t.transform.position + " is " + path.Count + " tiles.");
			targetPositions = path;
		}
	}

	public void onObjectShot(int damage) {
		health -= damage;
		if (health <= 0) {
			gm.removeZombie(this);
			Destroy(gameObject);
		}
	}

	public void setSelected(bool selected) {
		if (selected) {
			rend.color = selectionColor;
		}
		else {
			rend.color = baseColor;
		}
	}
}

