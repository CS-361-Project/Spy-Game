using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {
	protected Tile tile;
	protected GameManager gm;
	protected CircleCollider2D coll;
	protected Rigidbody2D body;

	protected Vector2 direction;
	protected List<Vector2> targetPositions;
	protected int currPosIndex;

	protected float speed;
	protected float viewDistance = 2.5f;

	protected int viewLayerMask = 1 << 10;

	protected bool beingPushed = false;

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
		targetPositions = gm.getPath(tile, gm.getFinishTile());
		currPosIndex = 0;
	}
	
	// Update is called once per frame
	public void move() {
		if (!beingPushed && currPosIndex < targetPositions.Count - 1) {
			Vector2 targetDirection = (targetPositions[currPosIndex + 1] - (Vector2)transform.position).normalized;
			Debug.DrawLine(transform.position, targetPositions[currPosIndex + 1]);

			direction = Vector2.Lerp(direction, targetDirection, 0.22f);

			body.velocity = direction * speed;
			direction = body.velocity.normalized;
			tile = gm.getClosestTile(transform.position);
			if (Vector2.Distance((Vector2)transform.position, targetPositions[currPosIndex + 1]) <= .1) {
				print("Reached point " + currPosIndex + ", moving to point " + (currPosIndex + 1));
				currPosIndex++;
			}
		}
		beingPushed = false;
	}

	public bool canSee(Vector3 pos) {
		Vector2 toObject = (pos - transform.position).normalized;
		float angle = Vector2.Dot(direction, toObject);
		if (angle <= 1 && angle >= 0.866025404) { // cos 0 to cos 60
			RaycastHit2D rayHit = Physics2D.Raycast(transform.position, toObject, viewDistance, viewLayerMask);
			if (rayHit.collider != null && rayHit.collider.transform.position == pos) {
				return true;
			}
		}
		return false;
	}

	public void applyFanForce(Vector2 force) {
		beingPushed = true;
		body.AddForce(force);
	}
}

