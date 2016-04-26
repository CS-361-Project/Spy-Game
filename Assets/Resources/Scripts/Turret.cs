using UnityEngine;
using System.Collections;

public class Turret : Survivor {

	Tile t;

	// Use this for initialization
	public void init(Tile t, GameManager m, int priority){
		size = 1f;
		base.init(t, m, priority);
		this.t = t;
		health = 200;
		shotFrequency = .15f;
		rotationSpeed = .01f;
		rend.sprite = Resources.Load<Sprite>("Sprites/turret");
		t.setPassable(false);
		Destroy(body);
		coll = gameObject.AddComponent<BoxCollider2D>();
		((BoxCollider2D)coll).size = new Vector2(.9f, .9f);

	}
	
	// Update is called once per frame
	public override void Update() {
		float closestDistance = float.MaxValue;
		Guard closestGuard = null;
		foreach (Tile t in tile.getNxNArea((int)viewDistance)) {
			foreach (Guard z in t.getZombieList()) {
				float dist = Vector2.Distance(z.transform.position, this.transform.position);
				if (dist < viewDistance && dist < closestDistance) {
					if (canSee(z.transform.position)) {
						closestDistance = dist;
						closestGuard = z;
					}
				}
			}
		}

		if (closestGuard == null) {
			bulletObj.SetActive(false);
		}

		else {
//			Vector2 pos = (Vector2) closestGuard.transform.position;
//			Vector2 toPos = pos - (Vector2)transform.position;
//			Vector2 startPoint = (Vector2)transform.position + toPos.normalized * size / 4;
//			Vector2 finalToPos = MathHelper.rotate(pos - startPoint, Random.Range(-6f, 6f));
//			RaycastHit hit = Physics2D.Raycast(startPoint, finalToPos, 2000, viewLayerMask);

			if (shotTimer >= shotFrequency) {
				Vector2 targetDirection = (closestGuard.transform.position - transform.position).normalized;
				float angle = Mathf.LerpAngle(Mathf.Rad2Deg * Mathf.Atan2(aimDirection.y, aimDirection.x), Mathf.Rad2Deg * Mathf.Atan2(targetDirection.y, targetDirection.x), rotationSpeed);
				transform.eulerAngles = new Vector3(0, 0, angle - 90);
				aimDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
				if (Vector2.Angle(aimDirection, targetDirection) < 15) {
					shootAt((Vector2)transform.position + aimDirection);
				}
			}
		}

		if (shotTimer >= shotDuration) {
			bulletObj.SetActive(false);
		}

		shotTimer += Time.deltaTime;
	}
		
	public override void shootAt(Vector2 pos) {
		bulletObj.SetActive(true);
		shotTimer = 0f;

		Vector2 toPos = pos - (Vector2)transform.position;
		Vector2 startPoint = (Vector2)transform.position + toPos.normalized * size / 4;
		Vector2 finalToPos = MathHelper.rotate(pos - startPoint, Random.Range(-10f, 10f));
		RaycastHit2D hit = Physics2D.Raycast(startPoint, finalToPos, 2000, viewLayerMask);
		Vector2 toHit = hit.point - startPoint;

		bulletObj.transform.position = startPoint;
		bulletObj.transform.localScale = new Vector2(toHit.magnitude / size, 0.1f);
		bulletObj.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(toHit.y, toHit.x));

		if (hit.collider != null) {
			Guard zomb = hit.collider.gameObject.GetComponent<Guard>();
			if (zomb != null) {
				zomb.onObjectShot(Random.Range(61, 81));
			}
		}
	}


	public void destroyTurret(){
		Destroy(gameObject);
		//set the tile it is on to passable again
		t.setPassable(true);
	}
}
