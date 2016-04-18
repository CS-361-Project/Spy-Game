using UnityEngine;
using System.Collections;

public class ControlPoint : Tile {
	int survivorCount;
	int zombieCount;
	float controlState;
	Owner currentOwner;

	enum Owner {
		Zombie, Unclaimed, Survivor
	};

	public const float takeoverTime = 5f;
	public const float troopOutputTime = 15f;
	public const float zombieClaimCoefficient = .05f;
	public const float survivorClaimCoefficient = .25f;

	public void init (int x, int y, GameManager gm) {
		survivorCount = 0;
		zombieCount = 0;
		base.init(x,y, gm, 0, 0, true);
		currentOwner = Owner.Unclaimed;
		controlState = 0;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
		survivorCount = getSurvivorList().Count;
		zombieCount = getZombieList().Count;
		if (survivorCount == 0) {
			if (zombieCount > 0) {
				if (currentOwner != Owner.Zombie) {
					controlState -= zombieCount * zombieClaimCoefficient;
					if (controlState <= -1) {
						controlState = -1;
						currentOwner = Owner.Zombie;
					}
					else
					if (controlState <= 0) {
						currentOwner = Owner.Unclaimed;
					}
				}
			}
		}
		else if (zombieCount == 0) {
			if (survivorCount > 0) {
				if (currentOwner != Owner.Survivor) {
					controlState += survivorCount * survivorClaimCoefficient;
					if (controlState >= 1) {
						controlState = 1;
						currentOwner = Owner.Survivor;
					}
					else
					if (controlState >= 0) {
						currentOwner = Owner.Unclaimed;
					}
				}
			}
		}
		if (controlState > 0) {
			// color lerp for survivors by controlState
		}
		else {
			// color lerp for zombies by -controlState
		}
	}

	bool checkIfBeingCaptured() {
		
	}
}
