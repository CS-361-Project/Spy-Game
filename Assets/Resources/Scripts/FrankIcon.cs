using UnityEngine;
using System.Collections;

public class FrankIcon : MonoBehaviour {
	
	public void init(int icon){
		SpriteRenderer rend = gameObject.AddComponent<SpriteRenderer>();
		switch (icon) {
			case (int)Frank.behavior.smoking:
				rend.sprite = Resources.Load<Sprite> ("Sprites/smoking");
				break;
			case (int)Frank.behavior.drinking:
				rend.sprite = Resources.Load<Sprite> ("Sprites/cocktail");
				break;
			case (int)Frank.behavior.talking:
				rend.sprite = Resources.Load<Sprite> ("Sprites/cellphone");
				break;
			case (int)Frank.behavior.puking:
				rend.sprite = Resources.Load<Sprite> ("Sprites/puking");
				break;
		}
		rend.sortingLayerName = "Foreground";
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
