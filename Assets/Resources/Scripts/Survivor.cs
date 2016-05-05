using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: Get Survivors to head towards hubs
//TODO: Give survivors health that goes down when their collider overlaps a zombie collider and then their speed will be proprotional to their health also set timer to begin for them to turn into zombies


public class Survivor : Person {

	protected SpriteRenderer rend;
	protected GameObject bulletObj;
	protected Vector2 aimDirection;
	protected float size = .45f;
	protected float shotTimer;
	protected float shotDuration;
	public int priority;
	protected int health;

	protected static float rotationSpeed;
	protected static float shotInterval;
	protected static int minDamage, maxDamage;

	ControlPoint destination;

	Tile startTile, endTile;
	[SerializeField]
	int patrolDirection;

	AudioSource source;
	// Use this for initialization
	public void init(Tile t, GameManager m, int priority) {
		base.init(t, m);
		this.priority = priority;
		viewLayerMask = (1 << LayerMask.NameToLayer("Guard")) | (1 << LayerMask.NameToLayer("Wall"));
		obstacleLayerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Survivor"));
		viewDistance = 6f;

		gameObject.layer = LayerMask.NameToLayer("Survivor");
		gameObject.tag = "Survivor";

		source = gameObject.AddComponent<AudioSource>();

		shotTimer = 0;
//		shotInterval = .55f;
		shotDuration = .05f;
		health = 100;
		rotationSpeed = .3F;
//		minDamage = 30;
//		maxDamage = 55;

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Survivor");
		rend.color = Color.blue;
//		rend.sortingLayerName = "UI";
		rend.sortingOrder = 1;

		transform.localScale = new Vector3(size, size, 1);

		bulletObj = new GameObject();
		bulletObj.transform.parent = transform;
		bulletObj.transform.localPosition = Vector3.zero;
		SpriteRenderer bulletRend = bulletObj.AddComponent<SpriteRenderer>();
		bulletRend.sprite = Resources.Load<Sprite>("Sprites/Beam");
		bulletRend.sortingLayerName = "Foreground";
		bulletObj.SetActive(false);

		startTile = t;
		//endTile = m.getTile(4, 6);
//		patrolDirection = 0;
		//		targetPositions = gm.getPath(tile, endTile);
		setDestination(gm.getControlPoints()[Random.Range(0, 4)]);
//		targetPositions = new List<Vector2>();
		//Debug.DrawLine(tile.transform.position + new Vector3(-.5f, .5f, 0), tile.transform.position + new Vector3(.5f, -.5f, 0));
		//Debug.DrawLine(endTile.transform.position + new Vector3(-.5f, .5f, 0), endTile.transform.position + new Vector3(.5f, -.5f, 0));
		speed = 2f;
		aimDirection = Vector2.left;
	}

	// Update is called once per frame

	public virtual void Update() {
//		if (patrolDirection == 2) {
//			patrolDirection = 0;
//		}

		//Find the closest guard
		//List<Survivor> closestSurvivorList = new List<Survivor>();

		//Find the average direction of the closest survivors
//		if (closestSurvivorList.Count > 0) {
//			Vector2 averageDir = nextPoint();
//			foreach (Survivor s in closestSurvivorList) {
//				averageDir += s.nextPoint();
//			}
//			averageDir = averageDir / (closestSurvivorList.Count + 1);
//			targetPositions = gm.getPath(tile, gm.getClosestTile(averageDir), false);
//		}

//		if (shotTimer >= shotFrequency) {
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
			speed = 2;
		}
		else {
			Vector2 targetDirection = (closestGuard.transform.position - transform.position).normalized;
			float angle = Mathf.LerpAngle(Mathf.Rad2Deg * Mathf.Atan2(aimDirection.y, aimDirection.x), Mathf.Rad2Deg * Mathf.Atan2(targetDirection.y, targetDirection.x), rotationSpeed);
			aimDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
			if (shotTimer >= shotInterval) {
				shootAt((Vector2)transform.position + aimDirection);
				speed = 0;
			}
		}
//		}
		if (shotTimer >= shotDuration) {
			bulletObj.SetActive(false);
		}

//		if (patrolDirection == 1) {
//			if (targetPositions.Count <= 0) {
//				targetPositions = gm.getPath(endTile, startTile, false);
//				patrolDirection = -1;
//			}
//		}
//		else if (patrolDirection == -1) {
//			if (targetPositions.Count <= 0) {
//				targetPositions = gm.getPath(startTile, endTile, false);
//				patrolDirection = 1;
//			}
//		}
		wander();

//		int highestPrioritySurvivor = int.MaxValue;
//		Survivor prioritySurvivor = null;
//		foreach (Survivor s in gm.getSurvivorList()) {
//			if (s != this) {
//				float dist = Vector2.Distance(s.transform.position, this.transform.position);
//				if (dist < viewDistance) {
//					Vector2 toObject = s.transform.position - transform.position;
//					RaycastHit2D hit = Physics2D.Raycast(transform.position, toObject.normalized, dist, 1 << LayerMask.NameToLayer("Wall"));
//					if (hit.collider == null && s.priority < highestPrioritySurvivor) {
//						//closestSurvivorList.Add(s);
//						highestPrioritySurvivor = s.priority;
//						prioritySurvivor = s;
//					}
//				}
//			}
//		}
//		if (prioritySurvivor != null && prioritySurvivor.priority < priority/* && prioritySurvivor.nextPoint() != (Vector2) prioritySurvivor.transform.position*/) {
//			targetPositions = gm.getPath(tile, gm.getClosestTile(prioritySurvivor.lastPoint()), false);
//		}

	    Tile oldTile = tile;
		bool changedTile = move();
		if (changedTile) {
			oldTile.removeSurvivor (this);
			tile.addSurvivor (this);
			if (oldTile.transform.position == destination.transform.position) {
				targetPositions.Clear();
				targetPositions.Add(oldTile.transform.position);
			}
		}
		transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aimDirection.y, aimDirection.x));
		shotTimer += Time.deltaTime;
	}

	void wander() {
		if (targetPositions.Count == 0 && tile != destination) {
			List<Vector2> pathToDestination = gm.getPath(tile, destination, false);
//			print("Moving from " + transform.position + " to " + destination.transform.position + " with path length " + pathToDestination.Count);
			targetPositions = pathToDestination;
		}
		else {
			body.velocity = Vector2.zero;
		}
	}

	void turnToZombie() {
		GameObject zombie = new GameObject();
		zombie.AddComponent<Guard>();
		zombie.transform.position = transform.position;
		Destroy(gameObject);
	}

	public virtual void damage(int damage) {
		health -= damage;
		if (health <= 0) {
			deathSound();
			gm.removeSurvivor(this);
			Destroy(gameObject);

		}
	}

	void deathSound(){
		if (source.isPlaying) {
			source.Stop();
		}
		int rand = Random.Range(0, 5);
		switch(rand){
			case 0:
				gm.audioCtrl.playDeathClip((int)AudioControl.death.death1, gm.audioCtrl.getSource2());
				break;
			case 1:					
				gm.audioCtrl.playDeathClip((int)AudioControl.death.death2, gm.audioCtrl.getSource2());
				break;
			case 2:
				gm.audioCtrl.playDeathClip((int)AudioControl.death.death3, gm.audioCtrl.getSource2());
				break;
			case 3:
				gm.audioCtrl.playDeathClip((int)AudioControl.death.death4, gm.audioCtrl.getSource2());
				break;
			case 4:
				gm.audioCtrl.playDeathClip((int)AudioControl.death.death5, gm.audioCtrl.getSource2());
				break;
			default:
				print("NO" + rand);
				break;
		}
	}

	public virtual void shootAt(Vector2 pos) {
		if (!source.isPlaying) {
			shootSound();
		}

		bulletObj.SetActive(true);
		shotTimer = 0f;

		Vector2 toPos = pos - (Vector2)transform.position;
		Vector2 startPoint = (Vector2)transform.position + toPos.normalized * size / 4;
		Vector2 finalToPos = MathHelper.rotate(pos - startPoint, Random.Range(-6f, 6f));
		RaycastHit2D hit = Physics2D.Raycast(startPoint, finalToPos, 2000, viewLayerMask);
		Vector2 toHit = hit.point - startPoint;

		bulletObj.transform.position = startPoint;
		bulletObj.transform.localScale = new Vector2(toHit.magnitude / size, 0.1f);
		bulletObj.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(toHit.y, toHit.x));

		if (hit.collider != null) {
			Guard zomb = hit.collider.gameObject.GetComponent<Guard>();
			if (zomb != null) {
				zomb.onObjectShot(Random.Range(minDamage, maxDamage));
			}
		}
	}

	void shootSound(){
		switch(Random.Range(0,2)){
			case 0:
				gm.audioCtrl.playClip((int)AudioControl.clips.gunFire1, source);
				break;
			case 1:
				gm.audioCtrl.playClip((int)AudioControl.clips.gunFire2, source);
				break;
			default:
				print("NO");
				break;
		}
	}

	public void setDestination(ControlPoint cp) {
		if (destination != null) {
			destination.removeIncomingSurvivor(this);
		}
		destination = cp;
		cp.addIncomingSurvivor(this);
		targetPositions = gm.getPath(tile, cp, false);
	}

	public ControlPoint getDestination() {
		return destination;
	}

	public static void setDamageRange(int min, int max) {
		minDamage = min;
		maxDamage = max;
	}

	public static void setShotInterval(float i) {
		shotInterval = i;
	}
}