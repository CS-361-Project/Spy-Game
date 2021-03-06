﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint : Tile {
	public int survivorCount;
	List<Survivor> incomingSurvivorList;
	public int zombieCount;
	float turretSpawnClock;
	float turretSpawnRate = 15;
	int remainingTurrets = 2;
	public float controlState;
	float spawnClock;
	public Owner currentOwner;
	GameObject alertRing;
	SpriteRenderer ringRend;
	float ringFlashTimer = 0;

	AudioSource source;
	float captureAnimationTimer = 0;
	float captureAnimationLength = 1;

	public enum Owner {
		Zombie,
		Unclaimed,
		Survivor}
	;

	SurvivorControl sc;

	public const float zombieClaimPerSecond = 1f / 30f;
	public const float survivorClaimPerSecond = 1f / 10f;
	public const float revertClaimPerSecond = 1f / 20f;

	public void init(int x, int y, GameManager gm, SurvivorControl control) {
		source = gameObject.AddComponent<AudioSource>();

		incomingSurvivorList = new List<Survivor>();
		survivorCount = 0;
		zombieCount = 0;
		turretSpawnClock = 0;
		base.init(x, y, gm, 0, 0, true);
		currentOwner = Owner.Unclaimed;
		controlState = 0;
		sc = control;

		alertRing = new GameObject();
		alertRing.transform.parent = transform;
		alertRing.transform.localPosition = Vector3.zero;
		alertRing.transform.localScale = Vector3.one * 2;
		ringRend = alertRing.AddComponent<SpriteRenderer>();
		ringRend.sprite = Resources.Load<Sprite>("Sprites/Ring");
		ringRend.color = Color.red;
		ringRend.sortingLayerName = "UI";
		alertRing.SetActive(false);
	}
	
	// Update is called once per frame
	public void Update() {
		//base.Update();
		survivorCount = getSurvivorList().Count;
		zombieCount = getZombieList().Count;

		bool contested = false;
		if (survivorCount == 0 && zombieCount == 0) {
			
			switch (currentOwner) {
				case Owner.Zombie:
					if (controlState > -1) {
						controlState -= revertClaimPerSecond * gm.gameSpeed * Time.deltaTime;
						if (controlState < -1) {
							controlState = -1;
						}
					}
					break;
				case Owner.Survivor:
					if (controlState < 1) {
						controlState += revertClaimPerSecond * gm.gameSpeed * Time.deltaTime;
						if (controlState > 1) {
							controlState = 1;
						}
					}
					break;
				case Owner.Unclaimed:
					if (controlState > 0) {
						controlState -= revertClaimPerSecond * gm.gameSpeed * Time.deltaTime;
						if (controlState < 0) {
							controlState = 0;
						}
					}
					else if (controlState < 0) {
						controlState += revertClaimPerSecond * gm.gameSpeed * Time.deltaTime;
						if (controlState > 0) {
							controlState = 0;
						}
					}
					break;
			}
		}
		else if (survivorCount == 0) {
			if (zombieCount > 0) {
				if (currentOwner != Owner.Zombie) {
					Owner oldOwner = currentOwner;
					controlState -= zombieCount * zombieClaimPerSecond * gm.gameSpeed * Time.deltaTime;
					if (controlState <= -1) {
						if (!source.isPlaying) {
							switch (Random.Range(0, 2)) {
								case 0:
									gm.audioCtrl.playClip((int)AudioControl.Clip.zombieCheer1, source);
									break;
								case 1:
									gm.audioCtrl.playClip((int)AudioControl.Clip.zombieCheer2, source);
									break;
								default:
									print("what");
									break;
							}
						}
						controlState = -1;
						currentOwner = Owner.Zombie;
						//HEY I THINK there is a issue with our calls to modify spawn rates, we never positibly modify the survivor spawn rate?
						gm.modifyZombieSpawnRate(.1f);
						spawnClock = 0f;
						captureAnimationTimer = 0;
					}
					else if (controlState <= 0) {
						currentOwner = Owner.Unclaimed;
					}
					if (oldOwner == Owner.Survivor && oldOwner != currentOwner) {
						gm.modifySurvivorSpawnRate(-.1f);
					}
				}
			}
		}
		else if (zombieCount == 0) {
			if (survivorCount > 0) {
				if (currentOwner != Owner.Survivor) {
					Owner oldOwner = currentOwner;
					controlState += survivorCount * survivorClaimPerSecond * gm.gameSpeed * Time.deltaTime;
					if (controlState >= 1) {
						if (source.isPlaying) {
							source.Stop();
						}
						captureSound();


						controlState = 1;
						currentOwner = Owner.Survivor;
						gm.modifySurvivorSpawnRate(.1f);
						captureAnimationTimer = 0;
						spawnClock = 0f;
						alertRing.SetActive(false);
					}
					else if (controlState >= 0) {
						currentOwner = Owner.Unclaimed;
						foreach (Tile t in getNxNArea(Guard.tileViewDistance * 2)) {
							t.checkVisibility ();
						}
						alertRing.SetActive(false);
					}
					else {
						// control point is being contested!

						contested = true;
					}
					if (oldOwner == Owner.Zombie && oldOwner != currentOwner) {
						gm.modifyZombieSpawnRate(-.1f);
					}
				}
			}
		}
		if (contested) {
			if (!source.isPlaying){
				gm.audioCtrl.playClip(Random.Range(0, gm.audioCtrl.getSurvivorSounds().Length), source);
			}
			alertRing.SetActive(true);
			alertRing.transform.localScale = Vector3.one;
			ringRend.color = Color.Lerp(Color.white, Color.red, Mathf.Sin(ringFlashTimer * 2 * Mathf.PI) / 2 + .5f);
			ringFlashTimer += Time.deltaTime;
		}
		else {
			alertRing.SetActive(false);
		}
		if (controlState > 0) {
			rend.color = Color.Lerp(Color.white, Color.cyan, controlState);
			// color lerp for survivors by controlState
		}
		else {
			// color lerp for zombies by -controlState
			rend.color = Color.Lerp(Color.white, Color.yellow, -controlState);
		}
		if (controlState >= 1) {
			turretSpawnClock += Time.deltaTime;
			if (turretSpawnClock >= turretSpawnRate && remainingTurrets > 0) {
				spawnTurret();
			}
		}
		if (captureAnimationTimer <= captureAnimationLength) {
			if (currentOwner == Owner.Zombie) {
				alertRing.SetActive(true);
				ringRend.color = Color.blue;
				float size = captureAnimationTimer * 10;
				alertRing.transform.localScale = new Vector3(size, size, 1);
			}
			else if (currentOwner == Owner.Survivor) {
				alertRing.SetActive(true);
				ringRend.color = Color.red;
				float size = captureAnimationTimer * 10;
				alertRing.transform.localScale = new Vector3(size, size, 1);
			}
		}
		captureAnimationTimer += Time.deltaTime;
	}

	void captureSound(){
		switch(Random.Range(0,3)){
			case 0:
				gm.audioCtrl.playClip((int)AudioControl.Clip.captureSound1, source);
				break;
			case 1:
				gm.audioCtrl.playClip((int)AudioControl.Clip.captureSound2, source);
				break;
			case 2:
				gm.audioCtrl.playClip((int)AudioControl.Clip.captureSound3, source);
				break;
			default:
				print("NO");
				break;
		}
	}

	public virtual void setVisibility(bool visible) {
		if (!visible) {
			rend.sortingLayerName = "Foreground";
			rend.sortingOrder = 3;
//			rend.color = Color.white;
		}
		else {
			rend.sortingLayerName = "Default";
			rend.sortingOrder = 0;
		}
	}

	public void addIncomingSurvivor(Survivor s) {
		incomingSurvivorList.Add(s);
	}

	public void removeIncomingSurvivor(Survivor s) {
		incomingSurvivorList.Remove(s);
	}

	public int getIncomingSurvivorCount() {
		return incomingSurvivorList.Count;
	}

	public List<Survivor> getIncomingSurvivors() {
		return incomingSurvivorList;
	}
 	void spawnTurret() {
		Tile[] area = getNxNEmptyTiles(5, false);
		Tile turret = area[Random.Range(0, area.Length)];
		gm.addTurret(turret.posX, turret.posY);
		turretSpawnClock = 0;
		remainingTurrets = remainingTurrets - 1;
	}
}
