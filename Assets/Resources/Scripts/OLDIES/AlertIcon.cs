using UnityEngine;
using System.Collections;

public class AlertIcon : MonoBehaviour {
	// Use this for initialization
	void Start() {
		SpriteRenderer rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Exclamation");
		rend.sortingLayerName = "Foreground";
	}
	
	// Update is called once per frame
	void Update() {
	
	}
}

