using UnityEngine;
using System.Collections;

public class ControlPoint : Tile {
	int survivorCount;
	int zombieCount;
	float controlState;
	float spawnClock;
	Owner currentOwner;

	public enum Owner {
		Zombie,
		Unclaimed,
		Survivor}

	;

	public const float troopSpawnTime = 15f;
	public const float zombieClaimPerSecond = 1f / 30f;
	public const float survivorClaimPerSecond = 1f / 10f;

	public void init(int x, int y, GameManager gm) {
		survivorCount = 0;
		zombieCount = 0;
		base.init(x, y, gm, 0, 0, true);
		currentOwner = Owner.Unclaimed;
		controlState = 0;
	}
	
	// Update is called once per frame
	public override void Update() {
		base.Update();
		survivorCount = getSurvivorList().Count;
		zombieCount = getZombieList().Count;
		if (survivorCount == 0) {
			if (zombieCount > 0) {
				if (currentOwner != Owner.Zombie) {
					controlState -= zombieCount * zombieClaimPerSecond * Time.deltaTime;
					if (controlState <= -1) {
						controlState = -1;
						currentOwner = Owner.Zombie;
						spawnClock = 0f;
					}
					else if (controlState <= 0) {
						currentOwner = Owner.Unclaimed;
					}
				}
			}
		}
		else if (zombieCount == 0) {
			if (survivorCount > 0) {
				if (currentOwner != Owner.Survivor) {
					controlState += survivorCount * survivorClaimPerSecond * Time.deltaTime;
					if (controlState >= 1) {
						controlState = 1;
						currentOwner = Owner.Survivor;
						spawnClock = 0f;
					}
					else if (controlState >= 0) {
						currentOwner = Owner.Unclaimed;
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
		spawnClock += Time.deltaTime;
		if (spawnClock >= troopSpawnTime) {
			spawnUnit(currentOwner);
		}
	}

	public virtual void setVisibility(bool visible) {
//		if (!visible) {
//			rend.sortingLayerName = "Foreground";
//			rend.sortingOrder = 3;
//			rend.color = Color.white;
//		}
//		else {
//			rend.sortingLayerName = "Default";
//			rend.sortingOrder = 0;
//		}
	}

	public void spawnUnit(Owner currentOwner) {
		if (currentOwner == Owner.Zombie) {
			gm.addGuard(posX, posY);
			gm.addGuard(posX, posY);
			gm.addGuard(posX, posY);
			gm.addGuard(posX, posY);
			gm.addGuard(posX, posY);
		}
		else if (currentOwner == Owner.Survivor) {
			gm.addSurvivor(posX, posY);
		}
		spawnClock = 0;
	}
}
