using UnityEngine;
using System.Collections;

public class Turret : Survivor {
	float targetAngle;
	TurretState state;
	float timeSinceStartedShooting;
	float shootingTime;
	float turretDegPerSecond;

	enum TurretState {
		LockedOn, Shooting, Idle
	};

	// Use this for initialization
	public void init(Tile t, GameManager m, int priority){
		size = 1f;
		base.init(t, m, priority);
		health = 200;
		shotFrequency = .05f;
		shotDuration = .02f;
		rotationSpeed = .09f;
		viewDistance = 4.0f;

		state = TurretState.Idle;
		timeSinceStartedShooting = 0;
		shootingTime = 5;
		turretDegPerSecond = 20;

		rend.sprite = Resources.Load<Sprite>("Sprites/turret");
//		t.setPassable(false);
		Destroy(body);
		coll = gameObject.AddComponent<BoxCollider2D>();
		((BoxCollider2D)coll).size = new Vector2(.9f, .9f);

	}
	
	// Update is called once per frame
	public override void Update() {
		if (state == TurretState.Shooting) {
			if (timeSinceStartedShooting >= shootingTime) {
				state = TurretState.Idle;
			}
			else {
				if (shotTimer >= shotFrequency) {
					shootAt((Vector2)transform.position + aimDirection);
				}
				else if (shotTimer >= shotDuration) {
					bulletObj.SetActive(false);
				}
			}
		}
		else if (state == TurretState.LockedOn) {
			float currAngle = Mathf.Rad2Deg * Mathf.Atan2(aimDirection.y, aimDirection.x);
			float angle = Mathf.Abs(targetAngle - currAngle);
			bool clockWise = (targetAngle - currAngle < 180) || (targetAngle + 360 - currAngle < 180);
			if (angle > 180) {
				angle = 360 - angle;
			}
			if (clockWise) {
				currAngle -= turretDegPerSecond * Time.deltaTime;
				if (currAngle < targetAngle) {
					currAngle = targetAngle;
				}
			}
			else {
				currAngle += turretDegPerSecond * Time.deltaTime;
				if (currAngle > targetAngle) {
					currAngle = targetAngle;
				}
			}
			aimDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currAngle), Mathf.Sin(Mathf.Deg2Rad * currAngle));
			transform.eulerAngles = new Vector3(0, 0, currAngle - 90);
			if (currAngle == targetAngle) {
				state = TurretState.Shooting;
				timeSinceStartedShooting = 0;
			}
		}
		else if (state == TurretState.Idle) {
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
				state = TurretState.LockedOn;
				Vector2 toGuard = closestGuard.transform.position - transform.position;
				targetAngle = Mathf.Rad2Deg * Mathf.Atan2(toGuard.y, toGuard.x);
			}
		}
		shotTimer += Time.deltaTime;
		timeSinceStartedShooting += Time.deltaTime;
	}
		
	public override void shootAt(Vector2 pos) {
		bulletObj.SetActive(true);
		shotTimer = 0f;

		Vector2 toPos = pos - (Vector2)transform.position;
		Vector2 startPoint = (Vector2)transform.position + toPos.normalized * size / 4;
		RaycastHit2D hit = Physics2D.Raycast(startPoint, toPos, 2000, viewLayerMask);
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



	public override void damage(int damage) {
		health -= damage;
		if (health <= 0) {
			gm.removeSurvivor(this);
			tile.setPassable(true);
			Destroy(gameObject);
		}
	}
}
