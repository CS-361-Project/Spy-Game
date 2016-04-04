using UnityEngine;
using System;
using System.Collections;

public class Chemical : MonoBehaviour {
	bool state;
	Vector2 position;
	SpriteRenderer rend;
	// Use this for initialization
	void Start () {
		state = false;
		position = transform.position;
		gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Beaker");
		rend.sortingOrder = 1;
		rend.color = Color.green;
	}

	public class ChemicalEventArgs : EventArgs {
		public Vector2 position { get; set; }
		public bool state { get; set; }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void onFanToggled(object source, Fan.FanEventArgs args) {
		if (args.state) {
			rend.color = Color.yellow;
		}
		else {
			rend.color = Color.green;
		}
	}

	void OnMouseDown() {
		toggle();
	}

	public event EventHandler<ChemicalEventArgs> ChemicalToggled;


	public void setState(bool newState) {
		state = newState;
		onChemicalToggled();
	}

	public void toggle() {
		state = !state;
		if (state) {
			rend.color = Color.green;
		}
		else {
			rend.color = Color.yellow;
		}
		onChemicalToggled();
	}


	public virtual void onChemicalToggled() {
		if (ChemicalToggled != null) {
			print("Triggering toggle event.");
			ChemicalEventArgs args = new ChemicalEventArgs();
			args.state = this.state;
			args.position = this.position;
			ChemicalToggled(this, args);
		}
		else {
			print("Not triggering toggle event - no listeners.");
		}
	}
}
