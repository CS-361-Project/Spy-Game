using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	int fireTimer;
	SpriteRenderer rend;
	bool flammable;

	// Use this for initialization
	public void init () {
		fireTimer = 0;
		gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sortingOrder = 0;
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.color = Color.grey;

	}
	
	// Update is called once per frame
	void Update () {
		checkForFire();
	}

	void checkForFire(){
		//if one of the neighbors is burning then start burning


		//if burning increase fire time
		fireTimer+=1;

	}
}
