using UnityEngine;
using System.Collections;

public class Frank : MonoBehaviour {
	
	bool onFire;
	enum states {smoking, drinking, talking, puking, shooting, running};

	Vector2 direction;
	Vector2 lineOfSight;

	Tile currentTile;


	// Use this for initialization
	void Start () {
		onFire = false;
		gameObject.AddComponent<BoxCollider2D> ();
		currentTile = GameManager.getClosestTile (transform.position);
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
