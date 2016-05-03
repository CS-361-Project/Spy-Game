using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zombie : Person {
	public static int tileViewDistance = 6;
	SpriteRenderer rend;

	float suspicion;
	int health;

	AlertIcon alert;

	[SerializeField]
	int patrolDirection;

	public bool selected;
	public bool chasingSurvivor;

	Color baseColor;
	Color selectionColor = new Color(.4f, .85f, 1f);

	int priority = 0;
	float attackClock;
	float attackCooldown;
	float actionClock;
	float ignoreSurvivorsTime;
	bool receivedCommand;
	bool ignoreSurvivors;

	// Use this for initialization
	public void init(Tile t, GameManager m, int priority) {
		base.init(t, m);
		t.addZombie(this);
		viewLayerMask = 1 << 9 | 1 << 10;

		baseColor = new Color(Random.Range(0, 55) / 255f, Random.Range(100, 255) / 255f, Random.Range(0, 14) / 255f);
		selectionColor = baseColor + new Color(.8f, 0, .4f);

		rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/Box"); 
		rend.color = baseColor;
		rend.sortingOrder = 1;

		gameObject.layer = LayerMask.NameToLayer("Guard");
		gameObject.tag = "Zombie";

		suspicion = 0.0f;
		actionClock = 0.0f;
		attackClock = 0.0f;
		attackCooldown = .25f;
		ignoreSurvivorsTime = 1f;
		receivedCommand = false;
		viewDistance = 6;

		targetPositions = new List<Vector2>();
		speed = 4f;
		health = 100;
		chasingSurvivor = false;
		ignoreSurvivors = false;
	}
	
	// Update is called once per frame
	void Update() {
		actionClock += Time.deltaTime * gm.gameSpeed;
		attackClock += Time.deltaTime * gm.gameSpeed;

		if (patrolDirection == 2) {
			patrolDirection = 0;
		}
		wander();

		Survivor closestSurvivor = null;
		float minDist = float.MaxValue;

//		if (actionClock > ignoreSurvivorsTime || (!receivedCommand) || !ignoreSurvivors) {
		if (!ignoreSurvivors) {
			receivedCommand = false;
			foreach (Survivor s in gm.getSurvivorList()) {
				float dist = Vector2.Distance (s.transform.position, this.transform.position);
				if (dist < viewDistance && dist < minDist) {
					Vector2 toObject = s.transform.position - transform.position;
					RaycastHit2D hit = Physics2D.Raycast (transform.position, toObject.normalized, dist, 1 << LayerMask.NameToLayer ("Wall"));
					if (hit.collider == null) {
						closestSurvivor = s;
						minDist = dist;
					}
				}
			}
		}
		if (closestSurvivor != null) {
			if (chasingSurvivor && targetPositions.Count > 0) {
				targetPositions[0] = closestSurvivor.transform.position;
			}
			else {
				targetPositions.Insert(0, closestSurvivor.transform.position);
				chasingSurvivor = true;
			}
		}
		else {
			chasingSurvivor = false;
		}

		Tile oldTile = tile;
		bool changedTile = move();
		if (changedTile) {
			oldTile.removeZombie(this);
			tile.addZombie(this);
		}

		Vector3 sumForce = Vector3.zero;
		Vector3 sumAvoidance = Vector3.zero;
		int neighborCount = 0;
		foreach (Tile t in tile.getNxNArea(3)) {
			if (t != null) {
				foreach (Zombie g in t.getZombieList()) {
					if (g != this) {
						float dist = Vector2.Distance(g.transform.position, transform.position);
						if (dist <= 0.3f) {
							sumForce += -10f * (g.transform.position - transform.position).normalized *
							radius / Mathf.Pow((Mathf.Max(Mathf.Min(dist, radius), .1f)), 2);
							neighborCount++;
						}
						if (dist <= 0.7f) {
							sumAvoidance -= (g.transform.position - transform.position).normalized *
								radius / Mathf.Pow((Mathf.Max(Mathf.Min(dist, radius), 0.3f)), 1.2f);
						}
					}
				}
			}
		}
		if (neighborCount > 0) {
			if (sumForce == Vector3.zero) {
				sumForce = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)).normalized;
			}
			sumForce = sumForce.normalized * 0.2f * Mathf.Min(sumForce.magnitude / (float)neighborCount, 3.23f);
			body.velocity = (Vector2)body.velocity.normalized + (Vector2)sumAvoidance.normalized * 0.6f;
			body.velocity = (Vector2)body.velocity.normalized * speed;
			body.velocity += (Vector2)sumForce;
		}
		//body.velocity = body.velocity.normalized * speed;
	}

	public void wander() {
		if (targetPositions.Count == 0) {
			body.velocity = Vector2.zero;
		}
	}

	public bool isPathing(){
		if (targetPositions.Count == 0)
			return false;
		if (chasingSurvivor)
			return false;
		return true;
	}

	public void startTimer(){
		actionClock = 0f;
		receivedCommand = true;
	}

	public void setIgnoreSurvivors(bool ignore) {
		ignoreSurvivors = ignore;
	}

	void OnCollisionStay2D(Collision2D coll) {
		if (attackClock >= attackCooldown && coll.gameObject.tag == "Survivor") {
			coll.gameObject.GetComponent<Survivor>().damage(5);
			attackClock = 0;
		}
	}

	public virtual void onFanToggled(object source, Fan.FanEventArgs args) {
		if (canSee(args.position)) {
			suspicion += .5f;
		}
	}

	public virtual void onBurnerToggled(object source, Burner.BurnerEventArgs args) {
		if (args.state) {
			if (canSee(args.position)) {
				suspicion += .5f;
				// also probably want to go turn that off
			}
		}
	}

	public virtual void onChemicalToggled(object source, Chemical.ChemicalEventArgs args) {
		if (canSee(args.position)) {
			suspicion += .5f;
			print("that's supsicious");
		}
	}

	public void onObjectShot(int damage) {
		health -= damage;
		if (health <= 0) {
			gm.removeZombie(this);
			Destroy(gameObject);
		}
	}

	public void setSelected(bool selected) {
		if (selected) {
			rend.color = selectionColor;
		}
		else {
			rend.color = baseColor;
		}
	}
}

