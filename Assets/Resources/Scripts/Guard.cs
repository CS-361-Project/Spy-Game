using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Guard : MonoBehaviour {
	SpriteRenderer rend;
	Vector2 position;
	Vector2 direction;
	Vector2 lookingAt;
	float speed;
	Tile tile;
	GameManager gm;
	// Use this for initialization
	void init(Tile t, GameManager m) {
		gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
		rend.sortingOrder = 1;
		tile = t;
		position = t.transform.position;
		direction = new Vector2(1, 0);
		lookingAt = new Vector2(1, 0);
		speed = 2;
		gm = m;
	}
	
	// Update is called once per frame
	void Update() {
		position += direction * Time.deltaTime * speed;
		tile = gm.getClosestTile(position);
		lookingAt = direction;
		foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, 5)) {
			//TODO: Make sure it's not something boring like a wall
			Vector2 toObject = (c.transform.position - transform.position).normalized;
			float angle = Vector2.Dot(lookingAt, toObject);
			if (angle <= 0.15425145 || angle >= -0.15425145) { // -30 to 30 degrees
				if (Physics2D.Raycast(transform.position, c.transform.position - transform.position).collider == c) {
					// found object code
				}
			}
		}
	}

	void OnCollision(Collider2D c) {
		float sin = Mathf.Sin(Mathf.PI / 2);
		float cos = Mathf.Cos(Mathf.PI / 2);

		float tx = direction.x;
		float ty = direction.y;
		direction.x = (cos * tx) - (sin * ty);
		direction.y = (sin * tx) + (cos * ty); // rotate 90 degrees on collision
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
			rend.color = Color.yellow;
		}
	}
}

