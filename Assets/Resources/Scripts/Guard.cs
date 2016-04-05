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
	BoxCollider2D coll;

	float speed;
	float suspicion;

	AlertIcon alert;

	// Use this for initialization
	public void init(Tile t, GameManager m) {
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;
		tile = t;
		position = t.transform.position;

		coll = gameObject.AddComponent<BoxCollider2D>();
		Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		body.isKinematic = true;
		gameObject.layer = LayerMask.NameToLayer("Guard");

		tile = t;
		transform.position = t.transform.position;
		transform.eulerAngles = Vector3.zero;
		direction = new Vector2(0, 1);
		lookingAt = new Vector2(0, 1);
		gm = m;

		suspicion = 0.0f;
		speed = 2.0f;
	}
	
	// Update is called once per frame
	void Update() {
		if (suspicion >= 1f) {
			if (alert == null) {
				GameObject alertObj = new GameObject();
				alertObj.name = "Alert";
				alert = alertObj.AddComponent<AlertIcon>();
				alert.transform.parent = transform;
				alert.transform.localPosition = Vector3.up;
			}
			suspicion -= .05f;
			if (suspicion < 1f) {
				Destroy(alert.gameObject);
			}
		}
		transform.position = (Vector2)transform.position + (direction * Time.deltaTime * speed);
		tile = gm.getClosestTile(transform.position);	
		lookingAt = direction.normalized;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 5)) {
			//TODO: Make sure it's not something boring like a wall
			if (c != coll && c.gameObject.name != "Wall") {
				Vector2 toObject = (c.transform.position - transform.position).normalized;
				float angle = Vector2.Dot(lookingAt, toObject);
//				print("Angle from guard to " + c.gameObject.name + " is " + angle);
				if (angle <= 1 && angle >= 0.866025404) { // 0 to 60
					if (Physics2D.Raycast(transform.position, toObject, 10, 1 << 9).collider == c) {
//						direction = toObject;
						print("Guard sees an object: " + c.gameObject.name);
						suspicion = 1.5f;
						Debug.DrawLine(c.transform.position, transform.position, new Color(angle / 2.0f + .5f, angle / 2f + .5f, angle / 2f + .5f));
						// found object code
					}
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
//		transform.position = (Vector2)transform.position - (direction * Time.deltaTime * speed); // get unstuck
//		float sin = Mathf.Sin(Mathf.PI / 2);
//		float cos = Mathf.Cos(Mathf.PI / 2);
		direction *= -1;
//		float tx = direction.x;
//		float ty = direction.y;
//		direction.x = (cos * tx) - (sin * ty);
//		direction.y = (sin * tx) + (cos * ty); // rotate 90 degrees on collision
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

