using UnityEngine;
using System.Collections;

public class LevelObject : MonoBehaviour {
	protected Collider2D coll;

	public virtual void onObjectShot() {
		print(gameObject.name + " was shot!");
	}

	public virtual void MouseClicked() {

	}
}

