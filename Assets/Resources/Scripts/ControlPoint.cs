using UnityEngine;
using System.Collections;

public class ControlPoint : Tile {
	int survivorCount;
	int zombieCount;

	enum Owner {
		Zombie, Survivor, Unclaimed
	};

	public const float takeoverTime = 5f;
	public const float troopOutputTime = 15f;

	public void init (int x, int y, GameManager gm) {
		survivorCount = 0;
		zombieCount = 0;
		base.init(x,y, gm, 0, 0, true);
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	void compareCounts(){
		base.getSurvivorList().Count;
		base.getZombieList().Count;
	}
}
