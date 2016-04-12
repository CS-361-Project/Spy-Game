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
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2000);
		Person p = hit.collider.gameObject.GetComponent<Person>();
		Vector2 hitPos = transform.position;
		if (p != null) {
			hitPos = p.transform.position;
			if (!alarmOn) {
				onMotionDetected();
			}
			alarmOn = true;
		}
		else if (hit.collider != null) {
			hitPos = hit.point;
			alarmOn = false;
		}
		rend.color = alarmOn ? red : green;
		setBeamEndpoint(hitPos);
	}

	public virtual void onMotionDetected() {
		if (MotionDetected != null) {
			LaserEventArgs args = new LaserEventArgs() { position = transform.position };
			MotionDetected(this, args);
		}
	}

	void setBeamEndpoint(Vector2 pos) {
		Tile startTile = gm.getClosestTile(transform.position);
		Tile endTile = gm.getClosestTile(pos);
		setLaser(startTile, endTile);

		laserBeam.transform.localScale = new Vector2(Vector2.Distance(pos, transform.position), 0.1f);
		laserBeam.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x));
	}

	void setLaser(Tile startTile, Tile endTile) {
		int x0 = startTile.posX;
		int x1 = endTile.posX;
		if (x0 > x1) {
			int temp = x1;
			x1 = x0;
			x0 = temp;
		}
		int y0 = startTile.posY;
		int y1 = endTile.posY;
		if (y0 < y1) {
			int temp = y1;
			y1 = y0;
			y0 = temp;
		}
		foreach (Tile t in lastCoveredTiles) {
			t.containsLaser = false;
		}
		lastCoveredTiles.Clear();
		for (int x = x0; x <= x1; x++) {
			for (int y = y0; y <= y1; y++) {
				Tile t = gm.getTile(x, y);
				t.containsLaser = true;
				lastCoveredTiles.Add(t);
			}
		}
	}
}

