﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {
	protected Tile tile;
	protected GameManager gm;
	protected CircleCollider2D coll;
	protected Rigidbody2D body;

	protected Vector2 direction;
	protected Vector2 intDirection;
	protected List<Vector2> targetPositions;

	protected float speed;
	protected float viewDistance = 2.5f;

	protected int viewLayerMask = 1 << 10;

	// Use this for initialization
	public virtual void init(Tile t, GameManager m) {
		tile = t;
		gm = m;

		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = new Vector3(0.7f, 0.7f, 1);

		coll = gameObject.AddComponent<CircleCollider2D>();
		body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		body.constraints = RigidbodyConstraints2D.FreezeRotation;

		speed = 1f;

		direction = new Vector2(1, 0);
		intDirection = new Vector2(1, 0);
		//targetPositions = gm.getPath(tile, gm.getFinishTile());
	}

	Vector2 nextPosition() {
		if (targetPositions.Count == 0) {
			return transform.position;
		}
		else {
			return targetPositions[0];
		}
	}

	public void wander() {
		if (targetPositions.Count == 0) {
			intDirection.Normalize();
			Vector2 dir = intDirection;
			print("IntDirection: " + intDirection);
			print("Transform + direction = " + ((Vector2)transform.position + intDirection));

			Tile nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			if (!nextTile.isPassable()) {
				print("Next tile is blocked");
				dir = MathHelper.rotate90(intDirection);
				nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			}
			if (!nextTile.isPassable()) {
				print("Right tile is blocked");
				dir = -MathHelper.rotate90(intDirection);
				nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			}
			if (!nextTile.isPassable()) {
				print("Left tile is blocked");
				dir = -intDirection;
				nextTile = gm.getClosestTile((Vector2)transform.position + dir);
			}
			intDirection = dir.normalized;
			targetPositions.Add(nextTile.transform.position);
			print("Next tile: " + nextTile.transform.position);
		}
	}
	
	// called once per frame
	public void move() {
		if (targetPositions.Count >= 1) {
			if (Vector2.Distance((Vector2)transform.position, targetPositions[0]) <= .1) {
				print("Removing point");
				targetPositions.RemoveAt(0);
				if (targetPositions.Count < 1) {
					return;
				}
			}

			Debug.DrawLine(transform.position, targetPositions[0]);

			Vector2 targetDirection = (targetPositions[0] - (Vector2)transform.position).normalized;
//			Debug.DrawLine(transform.position, targetPositions[0]);

//			direction = Vector2.Lerp(direction, targetDirection, 0.3f).normalized;
			float angle = Mathf.LerpAngle(Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x), Mathf.Rad2Deg * Mathf.Atan2(targetDirection.y, targetDirection.x), .3f);
			direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));

			body.velocity = direction * speed;
			direction = body.velocity.normalized;
			tile = gm.getClosestTile(transform.position);

		}
	}

	public bool canSee(Vector3 pos) {
		bool view = canSee(pos, Mathf.Deg2Rad * 60, viewDistance);
		bool peripheral = canSee(pos, Mathf.Deg2Rad * 180, viewDistance / 2);
		return view || peripheral;
	}

	// angle in radians
	public bool canSee(Vector3 pos, float viewAngle, float maxDist) {
		Vector2 toObject = (pos - transform.position);
		float angle = Vector2.Dot(direction, toObject);
		if (angle <= 1 && angle >= Mathf.Cos(viewAngle)) { // cos 0 to cos 60
			RaycastHit2D rayHit = Physics2D.Raycast(transform.position, toObject.normalized, toObject.magnitude, 1 << 10);
			if (rayHit.collider == null && toObject.magnitude <= maxDist) {
				return true;
			}
		}
		return false;
	}
}

