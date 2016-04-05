using UnityEngine;
using System.Collections;

public class Frank : MonoBehaviour {
	SpriteRenderer rend;
	bool onFire;
	enum states {smoking, drinking, talking, puking, shooting, running};

	Vector2 position;
	Vector2 direction;
	Vector2 lineOfSight;
	float speed;

	Tile currentTile;


	// Use this for initialization
	public void init (Tile t, GameManager man) {
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.magenta;
		rend.sortingOrder = 1;
		onFire = false;

		gameObject.AddComponent<BoxCollider2D> ();
		currentTile = t;
		position = currentTile.transform.position;
		direction = new Vector2 (1f, 1f, 0f);
		lineOfSight = direction;

	}

	void Wander(){

	}

	void lookAround(){

	}
	
	// Update is called once per frame
	void Update () {
		currentTile = GameManager.getClosestTile (transform.position);
	}
}
