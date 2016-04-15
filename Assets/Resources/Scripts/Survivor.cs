using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Survivor : Person {

	SpriteRenderer rend;
	GameObject bulletObj;
	public float size = .45f;
	float shotTimer;
	float shotFrequency;
	float shotDuration;

	Tile startTile, endTile;
	[SerializeField]
	int patrolDirection;
	// Use this for initialization
	public override void init(Tile t, GameManager m) {
		base.init(t, m);
		viewLayerMask = (1 << LayerMask.NameToLayer("Guard")) | (1 << LayerMask.NameToLayer("Wall"));
		obstacleLayerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Survivor"));
		viewDistance = 6f;

		gameObject.layer = LayerMask.NameToLayer("Survivor");

		shotTimer = 0; 
		shotFrequency = .25f;
		shotDuration = .05f;

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;

		transform.localScale = new Vector3(size, size, 1);

		bulletObj = new GameObject();
		bulletObj.transform.parent = transform;
		bulletObj.transform.localPosition = Vector3.zero;
		SpriteRenderer bulletRend = bulletObj.AddComponent<SpriteRenderer>();
		bulletRend.sprite = Resources.Load<Sprite>("Sprites/Beam");
		bulletRend.sortingLayerName = "Foreground";
		bulletObj.SetActive(false);

		startTile = t;
		//endTile = m.getTile(4, 6);
		patrolDirection = 0;
		//		targetPositions = gm.getPath(tile, endTile);
		targetPositions = new List<Vector2>();
		//Debug.DrawLine(tile.transform.position + new Vector3(-.5f, .5f, 0), tile.transform.position + new Vector3(.5f, -.5f, 0));
		//Debug.DrawLine(endTile.transform.position + new Vector3(-.5f, .5f, 0), endTile.transform.position + new Vector3(.5f, -.5f, 0));
		speed = 1f;
	}

	// Update is called once per frame
	void Update() {
		if (patrolDirection == 2) {
			patrolDirection = 0;
		}

		if (shotTimer >= shotFrequency) {
			float closestDistance = float.MaxValue;
			Guard closestGuard = null;
			foreach (Guard z in gm.getGuardList()) {
				float dist = Vector2.Distance(z.transform.position, this.transform.position);
				if (dist < viewDistance && dist < closestDistance) {
					if (canSee(z.transform.position)) {
						closestDistance = dist;
						closestGuard = z;
					}
				}
			}
			if (closestGuard == null) {
				bulletObj.SetActive(false);
			}
			else {
				shootAt(closestGuard.transform.position);
			}
		}
		else if (shotTimer >= shotDuration) {
			bulletObj.SetActive(false);
		}

//		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, viewDistance)) {
//			if (c != coll && c.gameObject.name != "Wall") {
//				if (Vector2.Distance(c.transform.position, transform.position) <= viewDistance / 2 || canSee(c.transform.position)) {
//					switch (c.gameObject.name) {
//					case "Zombie":
//						targetPositions = gm.getPath(tile, gm.getClosestTile(c.transform.position), false);
//						if (targetPositions.Count >= 2) {
//							targetPositions.RemoveAt(targetPositions.Count - 1);
//							targetPositions.RemoveAt(0);
//						}
//						targetPositions.Add(c.transform.position);
//						break;
//					case "Survivor":
//						break;
//					}
//				}
//			}
//		}
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
		else if (patrolDirection == 0) {
			wander(true);
		}
		shotTimer += Time.deltaTime;
	}

	void turnToZombie() {
		GameObject zombie = new GameObject();
		zombie.AddComponent<Zombie>();
		zombie.transform.position = transform.position;
		Destroy(gameObject);
	}

	public void shootAt(Vector2 pos) {
		bulletObj.SetActive(true);
		shotTimer = 0f;

		Vector2 toPos = pos - (Vector2)transform.position;
		Vector2 startPoint = (Vector2)transform.position + toPos.normalized * size/4;
		Vector2 finalToPos = MathHelper.rotate(pos - startPoint, Random.Range(-6f, 6f));
		RaycastHit2D hit = Physics2D.Raycast(startPoint, finalToPos, 2000, viewLayerMask);
		Vector2 toHit = hit.point - startPoint;

		bulletObj.transform.position = startPoint;
		bulletObj.transform.localScale = new Vector2(toHit.magnitude / size, 0.1f);
		bulletObj.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(toHit.y, toHit.x));

//			if (hit.collider != null) {
//				LevelObject hitObj = hit.collider.gameObject.GetComponent<LevelObject>();
//				if (hitObj != null) {
//					hitObj.onObjectShot();
//				}
//			}
	}

	void flock() {

	}
}