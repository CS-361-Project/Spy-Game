using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : Person {
	SpriteRenderer rend;

	float suspicion;

	AlertIcon alert;
	FOV fovDisplay;

	Tile startTile, endTile;
	[SerializeField]
	int patrolDirection;


	// Use this for initialization
	public override void init(Tile t, GameManager m) {
		base.init(t, m);
		viewLayerMask = 1 << 9 | 1 << 10;

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;
		gameObject.layer = LayerMask.NameToLayer("Guard");

		GameObject fovObj = new GameObject();
		fovObj.name = "FOV";
		fovObj.transform.parent = transform;
		fovDisplay = fovObj.AddComponent<FOV>();
		fovDisplay.init(viewDistance);

		suspicion = 0.0f;

		startTile = t;
		endTile = m.getTile(4, 6);
		patrolDirection = 0;
//		targetPositions = gm.getPath(tile, endTile);
		targetPositions = new List<Vector2>();
		Debug.DrawLine(tile.transform.position + new Vector3(-.5f, .5f, 0), tile.transform.position + new Vector3(.5f, -.5f, 0));
		Debug.DrawLine(endTile.transform.position + new Vector3(-.5f, .5f, 0), endTile.transform.position + new Vector3(.5f, -.5f, 0));
		speed = 1f;
	}
	
	// Update is called once per frame
	void Update() {
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
//		if (targetPositions.Count > 0) {
//			print("Count: " + targetPositions.Count);
//		}
		if (patrolDirection == 2) {
			patrolDirection = 0;
		}
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, viewDistance)) {
			if (c != coll && c.gameObject.name != "Wall") {
				if (Vector2.Distance(c.transform.position, transform.position) <= viewDistance / 2 || canSee(c.transform.position)) {
					switch (c.gameObject.name) {
						case "Frank":
							print("Guard sees Frank");
							suspicion = 2f;
							targetPositions = gm.getPath(tile, gm.getClosestTile(c.transform.position));
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
		}
		if (patrolDirection == 1) {
			if (targetPositions.Count <= 0) {
				targetPositions = gm.getPath(endTile, startTile);
				patrolDirection = -1;
			}
		}
		else if (patrolDirection == -1) {
			if (targetPositions.Count <= 0) {
				targetPositions = gm.getPath(startTile, endTile);
				patrolDirection = 1;
			}
		}
		else if (patrolDirection == 0) {
			wander();
		}
		move();
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
}

