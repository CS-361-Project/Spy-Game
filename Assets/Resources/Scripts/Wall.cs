using UnityEngine;
using System.Collections;

public class Wall : Tile {

	public override bool isPassable() {
		return false;
	}
	public void init(int x, int y, GameManager game) {
		base.init(x, y, game, 0, 0, false);
		rend.color = Color.black;
		BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
		coll.sharedMaterial = Resources.Load<PhysicsMaterial2D>("WallMaterial");
		gameObject.name = "Wall";
		flammable = false;
		gameObject.layer = LayerMask.NameToLayer("Wall");
	}

//	public override void applyFanForce(string direc, int fanPosX, int fanPosY) {
//	}
//
//	public override void removeFanForce() {
//	}

	public override void setVisibility(bool visible) {
		if (!visible) {
			rend.sortingLayerName = "Foreground";
			rend.sortingOrder = 3;
			rend.color = Color.black;
		}
		else {
			rend.sortingLayerName = "Default";
			rend.sortingOrder = 0;
			rend.color = new Color(.25f, .25f, .25f);
		}
	}

	public override void setVisibility(float percentVisible){
		if (percentVisible < 0) {
			percentVisible = 0;
		} else if (percentVisible > 1) {
			percentVisible = 1;
		}

		if (percentVisible > .01f) {
			rend.sortingLayerName = "Default";
			rend.sortingOrder = 0;
		} else {
			rend.sortingLayerName = "Foreground";
			rend.sortingOrder = 3;
		}
		if (percentVisible < .4f) {
			rend.color = Color.Lerp (Color.black, new Color (.25f, .25f, .25f), percentVisible / .4f);
		} else {
			rend.color = new Color (.25f, .25f, .25f);;
		}
	}
}

