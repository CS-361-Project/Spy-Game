using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ZombieControl : MonoBehaviour {
	ZombieSelection zombieSelector;
	GameManager gm;
	HashSet<Guard> selection;

	// Use this for initialization
	public void init(GameManager g) {
		gm = g;
		zombieSelector = new GameObject().AddComponent<ZombieSelection>();
		zombieSelector.gameObject.name = "Zombie Selection";
		Minimap minimap = GameObject.Find("Minimap Camera").GetComponent<Minimap>();
		zombieSelector.init(minimap.cam);
		selection = new HashSet<Guard>();
		name = "Zombie Control";
	}

	public void removeZombie(Guard g) {
		selection.Remove(g);
	}
	// Update is called once per frame
	void Update() {
		Collider2D[] currSelection = zombieSelector.getSelectedObjects();
		bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		switch (zombieSelector.state) {
			case ZombieSelection.SelectionState.BeginningSelection:
				if (!shift) {
					foreach (Guard g in selection) {
						g.setSelected(false);
					}
					selection.Clear();
				}
				break;
			case ZombieSelection.SelectionState.EndingSelection:
				break;
			case ZombieSelection.SelectionState.Selecting:
				if (!shift) {
					foreach (Guard g in selection) {
						g.setSelected(false);
					}
					selection.Clear();
				}
				foreach (Collider2D coll in currSelection) {
					if (coll.gameObject.name == "Guard") {
						Guard g = coll.gameObject.GetComponent<Guard>();
						selection.Add(g);
						g.setSelected(true);
					}
				}
				break;
			case ZombieSelection.SelectionState.Idle:
				break;
		}
		if (zombieSelector.getMouseClicked()) {
			if (Input.GetKey(KeyCode.LeftShift)) {
				gm.moveTo(selection.ToList(), zombieSelector.getMousePosInWorldCoords(), false);
				roar();

			}
			else {
				gm.moveTo(selection.ToList(), zombieSelector.getMousePosInWorldCoords(), true);
				roar();
			}
		}
	}

	void roar(){
		int rand = Random.Range(0, 2);
		switch (rand) {
			case 0:
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieRoar1);
				break;
			case 1: 
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieRoar2);
				break;
			case 2:
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieRoar3);
				break;
			default:
				print("huh? this shouldn't happen.");
				break;
		}

	}
}

