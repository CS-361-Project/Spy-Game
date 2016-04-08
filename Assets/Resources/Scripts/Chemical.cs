using UnityEngine;
using System;
using System.Collections;

public class Chemical : MonoBehaviour {
	public bool spilled;
	Vector2 position;
	SpriteRenderer rend;
	// Use this for initialization
	void Start () {
		spilled = false;
		position = transform.position;
		gameObject.AddComponent<BoxCollider2D>();
		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Fan");
		rend.sortingOrder = 1;
		rend.color = Color.green;

		gameObject.layer = LayerMask.NameToLayer("Room Objects");
	}

	public class ChemicalEventArgs : EventArgs {
		public Vector2 position { get; set; }
		public bool spilled { get; set; }
	}

	// Update is called once per frame
	void Update () {
	
	}
		
	void OnMouseDown() {
		toggle();
	}

	public event EventHandler<ChemicalEventArgs> ChemicalToggled;


	public void setState(bool newState) {
		spilled = newState;
		onChemicalToggled();
	}

	public void toggle() {
		spilled = !spilled;
		if (spilled) {
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
			args.spilled = this.spilled;
			args.position = this.position;
			ChemicalToggled(this, args);
		}
	}
}
