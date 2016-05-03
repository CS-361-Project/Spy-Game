using UnityEngine;
using System.Collections;

public class Wall : Tile {
	GameObject wallObj;
	Material wallMaterial;
	public override bool isPassable() {
		return false;
	}
	public void init(int x, int y, GameManager game) {
		base.init(x, y, game, 0, 0, false);

//		rend.color = Color.black;
		BoxCollider2D coll = gameObject.AddComponent<BoxCollider2D>();
		coll.sharedMaterial = Resources.Load<PhysicsMaterial2D>("WallMaterial");
		gameObject.name = "Wall";
		flammable = false;
		gameObject.layer = LayerMask.NameToLayer("Wall");

		Destroy(rend);
		wallObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		wallObj.transform.parent = transform;
		wallObj.transform.localPosition = new Vector3(0, 0, -.5f);
		wallMaterial = wallObj.GetComponent<MeshRenderer>().material;
		wallMaterial.color = Color.black;
	}

//	public override void applyFanForce(string direc, int fanPosX, int fanPosY) {
//	}
//
//	public override void removeFanForce() {
//	}

	public override void setVisibility(bool visible) {
		if (!visible) {
//			rend.sortingLayerName = "Foreground";
//			rend.sortingOrder = 3;
//			rend.color = Color.black;
			wallMaterial.color = Color.black;
		}
		else {
//			rend.sortingLayerName = "Default";
//			rend.sortingOrder = 0;
//			rend.color = new Color(.25f, .25f, .25f);
			wallMaterial.color = new Color(.25f, .25f, .25f);
		}
	}
}

