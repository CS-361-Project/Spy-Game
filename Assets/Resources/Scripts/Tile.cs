using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	int fireTimer;
	SpriteRenderer rend;
	bool flammable;

	// Use this for initialization
	void init () {
		fireTimer = 0;
		gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 1;
		//TODO: find a good tile sprite
		//rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.yellow;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void checkForFire(){
		//if one of the neighbors is burning then start burning

		//if burning increase fire time
		fireTimer+=1;

	}
}
