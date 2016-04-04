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
		gameObject.AddComponent<BoxCollider2D> ();
	}

	void Wander(){

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
