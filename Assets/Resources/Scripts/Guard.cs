using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour {
	SpriteRenderer rend;
	// Use this for initialization
	void Start() {
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Guard");
		rend.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	public virtual void onFanToggled(object source, Fan.FanEventArgs args) {
		if (args.state) {
			rend.color = Color.red;
		}
		else {
			rend.color = Color.blue;
		}
	}
}

