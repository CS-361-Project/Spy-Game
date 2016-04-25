using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPoint : Tile {
	int survivorCount;
	List<Survivor> incomingSurvivorList;
	int zombieCount;
	float controlState;
	float spawnClock;
	public Owner currentOwner;

	public enum Owner {
		Zombie,
		Unclaimed,
		Survivor}
	;
	SurvivorControl sc;

	public const float zombieClaimPerSecond = 1f / 30f;
	public const float survivorClaimPerSecond = 1f / 10f;
	public const float revertClaimPerSecond = 1f/20f;

	public void init (int x, int y, GameManager gm, SurvivorControl control) {
		incomingSurvivorList = new List<Survivor>();
		survivorCount = 0;
		zombieCount = 0;
		base.init(x, y, gm, 0, 0, true);
		currentOwner = Owner.Unclaimed;
		controlState = 0;
		sc = control;
	}
	
	// Update is called once per frame
	public void Update() {
		//base.Update();
		survivorCount = getSurvivorList().Count;
		zombieCount = getZombieList().Count;
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
						controlState = -1;
						currentOwner = Owner.Zombie;
						gm.modifyZombieSpawnRate(.1f);
						spawnClock = 0f;
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
						controlState = 1;
						currentOwner = Owner.Survivor;
						gm.modifyZombieSpawnRate(.1f);
						spawnClock = 0f;
					}
					else if (controlState >= 0) {
						currentOwner = Owner.Unclaimed;
					}
					if (oldOwner == Owner.Zombie && oldOwner != currentOwner) {
						gm.modifyZombieSpawnRate(-.1f);
					}
				}
			}
		}
		if (controlState > 0) {
			rend.color = Color.Lerp(Color.white, Color.blue, controlState);
			// color lerp for survivors by controlState
		}
		else {
			// color lerp for zombies by -controlState
			rend.color = Color.Lerp(Color.white, Color.magenta, -controlState);
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
}
