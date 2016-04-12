using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : Person {
	SpriteRenderer rend;

	float suspicion;

	AlertIcon alert;
	FOV fovDisplay;

	Tile startTile, endTile;
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
		patrolDirection = 1;
		targetPositions = gm.getPath(startTile, endTile);
		speed = 2f;
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

		move();
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

