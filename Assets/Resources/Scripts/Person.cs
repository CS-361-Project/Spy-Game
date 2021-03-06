﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {
	public Tile tile;
	protected GameManager gm;
	protected Collider2D coll;
	protected Rigidbody2D body;

	public Vector2 direction;
	protected Vector2 intDirection;
	public List<Vector2> targetPositions;

	protected float speed;
	protected float viewDistance = 2.5f;

	public float radius = 0.5f;
	protected float rotationDegPerSecond = 2200;

	protected int viewLayerMask = 1 << 10;
	protected int obstacleLayerMask = (1 << LayerMask.NameToLayer("Wall"));

	protected bool beingPushed = false;
	protected bool onFire = false;
	protected float timeOnFire = 0.0f;

	// Use this for initialization
	public virtual void init(Tile t, GameManager m) {
		tile = t;
		gm = m;

		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = new Vector3(0.45f, 0.45f, 1);


		coll = gameObject.AddComponent<CircleCollider2D>();
		((CircleCollider2D)coll).radius = 0.45f;

		coll.sharedMaterial = Resources.Load<PhysicsMaterial2D>("WallMaterial");
		body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		body.constraints = RigidbodyConstraints2D.FreezeRotation;

		speed = 1f;

		direction = new Vector2(1, 0);
		intDirection = new Vector2(1, 0);
		targetPositions = new List<Vector2>();
		//targetPositions = gm.getPath(tile, gm.getFinishTile());
	}

	public Vector2 getPosition() {
		return transform.position;
	}

	Vector2 nextPosition() {
		if (targetPositions.Count == 0) {
			return transform.position;
		}
		else {
			return targetPositions[0];
		}
	}

	public virtual void wander(bool avoidLasers) {
		if (targetPositions.Count == 0) {
			body.velocity = Vector2.zero;
		}
		/*if (targetPositions.Count == 0) {
			intDirection.Normalize();
			Vector2 dir = intDirection;

			Tile nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			if (!nextTile.isPassable() || (avoidLasers && nextTile.containsLaser)) {
				dir = MathHelper.rotate90(intDirection);
				nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			}
			if (!nextTile.isPassable() || (avoidLasers && nextTile.containsLaser)) {
				dir = -MathHelper.rotate90(intDirection);
				nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			}
			if (!nextTile.isPassable() || (avoidLasers && nextTile.containsLaser)) {
				dir = -intDirection;
				nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			}
			intDirection = dir.normalized;
			targetPositions.Add(nextTile.transform.position);
		}*/
//		else {
//			print("Not wandering..." + targetPositions.Count + " better things to do.");
//		}
	}

	public Vector2 nextPoint(){
		if (targetPositions.Count > 0) {
			return targetPositions[0];
		}
		else {
			return transform.position;
		}
	}

	public Vector2 lastPoint() {
		if (targetPositions.Count > 0) {
			return targetPositions[targetPositions.Count - 1];
		}
		else {
			return transform.position;
		}
	}
	
	// called once per frame
	public bool move() {
		bool switchedTile = false;
//		if (onFire) {
//			timeOnFire += Time.deltaTime;
//			if (timeOnFire >= 5) {
//
//			}
//		}
		if (!beingPushed && targetPositions.Count >= 1) {
//			Vector2 toObject = targetPositions[0] - (Vector2)transform.position;
//			RaycastHit2D hit;
//			if (!gm.getClosestTile(targetPositions[0]).isPassable()) {
//				if (targetPositions.Count > 1) {
//					targetPositions.RemoveAt(0);
//				}
//				else {
//					targetPositions[0] = (Vector2)transform.position + 2 * toObject;
//				}
//			}
//			if ((hit = Physics2D.Raycast(transform.position, toObject.normalized, toObject.magnitude, 1 << 10)).collider !=  null) {
//				//print("Raycast hit " + hit.collider.gameObject.name + " at " + hit.collider.transform.position);
//				List<Vector2> path = gm.getPath(tile, gm.getClosestTile(targetPositions[0]), false);
//				targetPositions.RemoveAt(0);
//				if (path.Count > 0) {
//					targetPositions.InsertRange(0, path);
//				}
//				else {
//					targetPositions.Clear();
//					int x = Mathf.RoundToInt(direction.normalized.x);
//					intDirection = new Vector2(x, 1 - x);
//					wander(true);
//				}
//			}
			if (Vector2.Distance((Vector2)transform.position, targetPositions[0]) <= .25) {
				targetPositions.RemoveAt(0);
				if (targetPositions.Count < 1) {
					return false;
				}
			}

			//Debug.DrawLine(transform.position, targetPositions[0]);

			Vector2 targetDirection = (targetPositions[0] - (Vector2)transform.position).normalized;
//			Debug.DrawLine(transform.position, targetPositions[0]);

			//direction = Vector2.Lerp(direction, targetDirection, 0.3f).normalized;
			float angle = Vector2.Angle(direction, targetDirection);
			float currAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
			float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(targetDirection.y, targetDirection.x);
			if (angle > 180) {
				angle = currAngle + rotationDegPerSecond * Time.deltaTime;
				if (angle > targetAngle) {
					angle = targetAngle;
				}
			}
			else {
				angle = currAngle - rotationDegPerSecond * Time.deltaTime;
				if (angle < targetAngle) {
					angle = targetAngle;
				}
			}
			direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));

			body.velocity = direction * speed * gm.gameSpeed;
			direction = body.velocity.normalized;

		}
		Tile newTile = gm.getClosestEmptyTile(transform.position);
		if (newTile != tile) {
			tile = newTile;
			switchedTile = true;
		}
		beingPushed = false;
		/*foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, radius)) {
			if (c.gameObject.name == "Guard")
				body.AddForce(-5*(c.transform.position-transform.position).normalized*
					radius/Mathf.Max(Mathf.Min(Vector2.Distance((Vector2)c.transform.position,(Vector2)transform.position),radius),0.001f));
		}*/
		return switchedTile;
	}

//	public bool canSee(Vector3 pos) {
//		bool view = canSee(pos, Mathf.Deg2Rad * 30, viewDistance);
//		if (!view) {
//			bool peripheral = canSee(pos, Mathf.Deg2Rad * 90, viewDistance / 2);
//			return peripheral;
//		}
//		else {
//			return view;
//		}
//	}

	public bool canSee(Vector2 pos) {
		Vector2 toObject = (pos - (Vector2)transform.position);
		RaycastHit2D rayHit = Physics2D.Raycast(transform.position, toObject.normalized, toObject.magnitude, obstacleLayerMask);
		if (rayHit.collider == null && toObject.magnitude <= viewDistance) {
			return true;
		}
		return false;
	}

	// angle in radians
	public bool canSee(Vector3 pos, float viewAngle, float maxDist) {
		Vector2 toObject = (pos - transform.position);
//		float angle = Vector2.Dot(direction, toObject);
//		if (angle <= 1 && angle >= Mathf.Cos(viewAngle)) { // cos 0 to cos 60
			RaycastHit2D rayHit = Physics2D.Raycast(transform.position, toObject.normalized, toObject.magnitude, LayerMask.NameToLayer("Wall") | LayerMask.NameToLayer("Room Objects"));
			if (rayHit.collider == null && toObject.magnitude <= maxDist) {
				return true;
			}
//		}
		return false;
	}

	public void applyFanForce(Vector2 force) {
		beingPushed = true;
		body.AddForce(force);
		targetPositions.Clear();
		int x = Mathf.RoundToInt(force.normalized.x);
		int y = 1 - x;
		intDirection.Set(x, y);
	}

	public void setOnFire(bool fire) {
		if (onFire && !fire) {
			timeOnFire = 0;
		}
		onFire = fire;
	}

	public float getViewDistance() {
		return viewDistance;
	}
}

