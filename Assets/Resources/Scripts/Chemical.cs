using UnityEngine;
using System;
using System.Collections;

public class Chemical : MonoBehaviour {
	public bool state;
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

		gameObject.layer = LayerMask.NameToLayer("Room Objects");
	}

	public class ChemicalEventArgs : EventArgs {
		public Vector2 position { get; set; }
		public bool state { get; set; }
	}

	// Update is called once per frame
	void Update () {
	
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
			rend.color = Color.white;
		}
		else {
			rend.color = Color.green;
		}
		onChemicalToggled();
	}


	public virtual void onChemicalToggled() {
		if (ChemicalToggled != null) {
			ChemicalEventArgs args = new ChemicalEventArgs();
			args.state = this.state;
			args.position = this.position;
			ChemicalToggled(this, args);
		}
	}
}
