using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : MonoBehaviour {
	SpriteRenderer rend;
<<<<<<< HEAD
	Vector2 position;
=======
>>>>>>> origin/master
	Vector2 direction;
	Vector2 lookingAt;
	float speed;
	Tile tile;
<<<<<<< HEAD
	GameManager gm;
	// Use this for initialization
	void init(Tile t, GameManager m) {
=======
	BoxCollider2D coll;
	GameManager gm;
	// Use this for initialization
	public void init(Tile t, GameManager m) {
>>>>>>> origin/master
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;
<<<<<<< HEAD
		tile = t;
		position = t.transform.position;
=======
		coll = gameObject.AddComponent<BoxCollider2D>();
		Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		body.isKinematic = true;

		tile = t;
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
>>>>>>> origin/master
		direction = new Vector2(1, 0);
		lookingAt = new Vector2(1, 0);
		speed = 2;
		gm = m;
	}
	
	// Update is called once per frame
	void Update() {
<<<<<<< HEAD
		position += direction * Time.deltaTime * speed;
		Tile nextTile = gm.getClosestTile(position);
		if (nextTile != tile) {
			if (nextTile.isPassable()) {
				tile = nextTile;
			}
			else {
				List<Tile> neighbors
			}
		}
=======
		transform.position = (Vector2)transform.position + (direction * Time.deltaTime * speed);
		tile = gm.getClosestTile(transform.position);
		lookingAt = direction;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 5)) {
			//TODO: Make sure it's not something boring like a wall
			if (c != coll) {
				Vector2 toObject = (c.transform.position - transform.position).normalized;
				float angle = Vector2.Dot(lookingAt, toObject);
				if (angle <= 0.15425145 || angle >= -0.15425145) { // -30 to 30 degrees
					if (Physics2D.Raycast(transform.position, c.transform.position - transform.position).collider == c) {
//						direction = toObject;
						print("I see an object: " + c.gameObject.name);
						// found object code
					}
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		print("Collided with: " + c.gameObject.name);
//		transform.position = (Vector2)transform.position - (direction * Time.deltaTime * speed); // get unstuck
//		float sin = Mathf.Sin(Mathf.PI / 2);
//		float cos = Mathf.Cos(Mathf.PI / 2);
		direction *= -1;
//		float tx = direction.x;
//		float ty = direction.y;
//		direction.x = (cos * tx) - (sin * ty);
//		direction.y = (sin * tx) + (cos * ty); // rotate 90 degrees on collision
>>>>>>> origin/master
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

