using UnityEngine;
using System.Collections;

public class Turret : Survivor {
	Vector2 targetAimDirection;
	TurretState state;
	float timeSinceStartedShooting;
	float turretDegPerSecond;

	protected static float turretShotInterval;
	protected static float shootingTime;
	protected static int minTurretDamage;
	protected static int maxTurretDamage;

	enum TurretState {
		LockedOn, Shooting, Idle
	};

	// Use this for initialization
	public void init(Tile t, GameManager m, int priority){
		size = 1f;
		base.init(t, m, priority);
		health = 200;
//		turretShotInterval = .05f;
		shotDuration = .02f;
		rotationSpeed = .09f;
		viewDistance = 8f;

		state = TurretState.Idle;
		timeSinceStartedShooting = 0;
//		shootingTime = 3;
		turretDegPerSecond = 60;
//		minTurretDamage = 66;
//		maxTurretDamage = 81;

		rend.sprite = Resources.Load<Sprite>("Sprites/turret");
//		t.setPassable(false);
		Destroy(body);
		coll = gameObject.AddComponent<BoxCollider2D>();
		((BoxCollider2D)coll).size = new Vector2(.9f, .9f);
		transform.LookAt((Vector2)transform.position + aimDirection);

	}
	
	// Update is called once per frame
	public override void Update() {
		bulletObj.SetActive(false);
		if (state == TurretState.Shooting) {
			if (timeSinceStartedShooting >= shootingTime) {
				state = TurretState.Idle;
			}
			else {
				if (shotTimer >= turretShotInterval) {
					shootAt((Vector2)transform.position + aimDirection);
				}
				else if (shotTimer >= shotDuration) {
					bulletObj.SetActive(false);
				}
			}
		}
		else if (state == TurretState.LockedOn) {
			aimDirection = Vector3.RotateTowards(aimDirection, targetAimDirection, Mathf.Deg2Rad * turretDegPerSecond * Time.deltaTime, 1);
			float currAngle = Mathf.Rad2Deg * Mathf.Atan2(aimDirection.y, aimDirection.x);
			transform.eulerAngles = new Vector3(0, 0, currAngle - 90);
			if (aimDirection == targetAimDirection) {
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
				targetAimDirection = closestGuard.transform.position - transform.position;
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
				zomb.onObjectShot(Random.Range(minTurretDamage, maxTurretDamage));
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

	public static void setShootingTime(float t) {
		shootingTime = t;
	}

	public static void setTurretShotInterval(float i) {
		turretShotInterval = i;
	}

	public static void setTurretDamageRange(int min, int max) {
		minTurretDamage = min;
		maxTurretDamage = max;
	}
}
