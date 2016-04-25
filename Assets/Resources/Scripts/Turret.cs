using UnityEngine;
using System.Collections;

public class Turret : Survivor {

	// Use this for initialization
	public void init(Tile t, GameManager m, int priority){
		base.init(t, m, priority);
		health = 200;
		shotFrequency = .15f;
		rotationSpeed = .1f;
		rend.sprite = Resources.Load<Sprite>("Sprites/turret");
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
				aimDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
				shootAt((Vector2)transform.position + aimDirection);
			}
		}

		if (shotTimer >= shotDuration) {
			bulletObj.SetActive(false);
		}

		shotTimer += Time.deltaTime;
	}
}
