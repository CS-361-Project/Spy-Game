using UnityEngine;
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

	public float radius = 1f;

	protected int viewLayerMask = 1 << 10;

	protected bool beingPushed = false;
	protected bool onFire = false;
	protected float timeOnFire = 0.0f;

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

	public void wander(bool avoidLasers) {
		if (targetPositions.Count == 0) {
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
		}
//		else {
//			print("Not wandering..." + targetPositions.Count + " better things to do.");
//		}
	}
	
	// called once per frame
	public void move() {
		if (onFire) {
			timeOnFire += Time.deltaTime;
			if (timeOnFire >= 5) {

			}
		}
		if (!beingPushed && targetPositions.Count >= 1) {
			Vector2 toObject = targetPositions[0] - (Vector2)transform.position;
			RaycastHit2D hit;
			if (!gm.getClosestTile(targetPositions[0]).isPassable()) {
				if (targetPositions.Count > 1) {
					targetPositions.RemoveAt(0);
				}
//				else {
//					targetPositions[0] = (Vector2)transform.position + 2 * toObject;
//				}
			}
			if ((hit = Physics2D.Raycast(transform.position, toObject.normalized, toObject.magnitude, 1 << 10)).collider !=  null) {
				print("Raycast hit " + hit.collider.gameObject.name + " at " + hit.collider.transform.position);
				List<Vector2> path = gm.getPath(tile, gm.getClosestTile(targetPositions[0]), false);
				targetPositions.RemoveAt(0);
				if (path.Count > 0) {
					targetPositions.InsertRange(0, path);
				}
				else {
					targetPositions.Clear();
					int x = Mathf.RoundToInt(direction.normalized.x);
					intDirection = new Vector2(x, 1 - x);
					wander(true);
				}
			}
			if (Vector2.Distance((Vector2)transform.position, targetPositions[0]) <= .1) {
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
		beingPushed = false;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, radius)) {
			if (c.gameObject.name == "Guard")
				body.AddForce(-(c.transform.position-transform.position).normalized*
					radius/Mathf.Max(Mathf.Min(Vector2.Distance((Vector2)c.transform.position,(Vector2)transform.position),radius),0.001f));
		}
	}

	public bool canSee(Vector3 pos) {
		bool view = canSee(pos, Mathf.Deg2Rad * 30, viewDistance);
		if (!view) {
			bool peripheral = canSee(pos, Mathf.Deg2Rad * 90, viewDistance / 2);
			return peripheral;
		}
		else {
			return view;
		}
	}

	// angle in radians
	public bool canSee(Vector3 pos, float viewAngle, float maxDist) {
		Vector2 toObject = (pos - transform.position);
		float angle = Vector2.Dot(direction, toObject);
		if (angle <= 1 && angle >= Mathf.Cos(viewAngle)) { // cos 0 to cos 60
			RaycastHit2D rayHit = Physics2D.Raycast(transform.position, toObject.normalized, toObject.magnitude, LayerMask.NameToLayer("Wall") | LayerMask.NameToLayer("Room Objects"));
			if (rayHit.collider == null && toObject.magnitude <= maxDist) {
				return true;
			}
		}
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

	public void onFire(){

	}
}

