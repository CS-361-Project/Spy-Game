using UnityEngine;
using System;
using System.Collections;

public class LaserSensor : MonoBehaviour {
	GameObject laserBeam;
	Vector2 direction;
	public EventHandler<LaserEventArgs> MotionDetected;

	public class LaserEventArgs : EventArgs {
		public Vector2 position { get; set; }
	}
	// Use this for initialization
	public void init(Vector2 pos, Vector2 dir) {
		laserBeam = new GameObject();
		laserBeam.transform.parent = transform;
		SpriteRenderer rend = laserBeam.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Beam");
		rend.color = Color.red;
		rend.sortingLayerName = "Foreground";
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		transform.position = pos;
		direction = dir;
	}
	
	// Update is called once per frame
	void Update() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2000);
		Person p = hit.collider.gameObject.GetComponent<Person>();
		Vector2 hitPos = transform.position;
		if (p != null) {
			hitPos = p.transform.position;
			onMotionDetected(p);
		}
		else if (hit.collider != null) {
			hitPos = hit.point;
		}
		setBeamEndpoint(hitPos);
	}

	public virtual void onMotionDetected(Person p) {
		if (MotionDetected != null) {
			LaserEventArgs args = new LaserEventArgs() { position = p.transform.position };
			MotionDetected(this, args);
		}
	}

	void setBeamEndpoint(Vector2 pos) {
		laserBeam.transform.localScale = new Vector2(Vector2.Distance(pos, transform.position), 0.25f);
		laserBeam.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x));
	}
}

