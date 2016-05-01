using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LaserSensor : MonoBehaviour {
	List<Tile> lastCoveredTiles;
	GameObject laserBeam;
	GameManager gm;
	Vector2 direction;
	public EventHandler<LaserEventArgs> MotionDetected;
	bool alarmOn = false;
	SpriteRenderer rend;

	Color red = new Color(1, 0, 0, 0.8f);
	Color green = new Color(0, 1, 0, 0.8f);

	const int layerMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11;

	public class LaserEventArgs : EventArgs {
		public Vector2 position { get; set; }
	}
	// Use this for initialization
	public void init(GameManager m, Vector2 pos, Vector2 dir) {
		gm = m;
		laserBeam = new GameObject();
		laserBeam.transform.parent = transform;
		rend = laserBeam.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Beam");
		rend.color = red;
		rend.sortingLayerName = "Foreground";
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		transform.position = pos - dir/2.0f + dir*0.001f;
		direction = dir;
		lastCoveredTiles = new List<Tile>();
	}
	
	// Update is called once per frame
	void Update() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2000, layerMask);
		Person p = hit.collider.gameObject.GetComponent<Person>();
		Vector2 hitPos = transform.position;
		if (p != null) {
			hitPos = hit.point;
			if (!alarmOn) {
				onMotionDetected();
			}
			alarmOn = true;
		}
		else if (hit.collider != null) {
			hitPos = hit.point;
			alarmOn = false;
		}
		else {
			print("This shouldn't happen");
			hitPos = (Vector2)transform.position + direction * 2000;
		}
		rend.color = alarmOn ? red : green;
		setBeamEndpoint(hitPos);
		print("Setting beam endpoint : " + hitPos);
	}

	public virtual void onMotionDetected() {
		if (MotionDetected != null) {
			LaserEventArgs args = new LaserEventArgs() { position = transform.position };
			MotionDetected(this, args);
		}
	}

	void setBeamEndpoint(Vector2 pos) {
		Tile startTile = gm.getClosestEmptyTile(transform.position);
		Tile endTile = gm.getClosestEmptyTile(pos);
		setLaser(startTile, endTile);

		laserBeam.transform.localScale = new Vector2(Vector2.Distance(pos, transform.position), 0.1f);
		laserBeam.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x));
	}

	void setLaser(Tile startTile, Tile endTile) {
		foreach (Tile t in lastCoveredTiles) {
			t.containsLaser = false;
		}
		lastCoveredTiles.Clear();
		Vector2 dir = endTile.transform.position - startTile.transform.position;
		for (float i = 0; i <= 1; i+=1.0f/dir.magnitude) {
			Tile t = gm.getClosestEmptyTile(Vector2.Lerp(startTile.transform.position, endTile.transform.position, i));
			t.containsLaser = true;
			lastCoveredTiles.Add(t);
		}
	}
}

