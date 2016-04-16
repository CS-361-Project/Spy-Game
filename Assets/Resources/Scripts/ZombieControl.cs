﻿using UnityEngine;
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
		selection = new HashSet<Guard>();
		name = "Zombie Control";
	}

	// Update is called once per frame
	void Update() {
		Collider2D[] currSelection = zombieSelector.getSelectedObjects();
		bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		switch (zombieSelector.state) {
			case ZombieSelection.SelectionState.BeginningSelection:
				if (!shift) {
					foreach (Guard g in selection) {
						g.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
					}
					selection.Clear();
				}
				break;
			case ZombieSelection.SelectionState.EndingSelection:
				break;
			case ZombieSelection.SelectionState.Selecting:
				if (!shift) {
					foreach (Guard g in selection) {
						g.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
					}
					selection.Clear();
				}
				foreach (Collider2D coll in currSelection) {
					if (coll.gameObject.name == "Guard") {
						Guard g = coll.gameObject.GetComponent<Guard>();
						selection.Add(g);
						g.GetComponent<SpriteRenderer>().color = Color.green;
					}
				}
				break;
			case ZombieSelection.SelectionState.Idle:
				break;
		}
		if (zombieSelector.getMouseClicked()) {
			gm.moveTo(selection.ToList(), zombieSelector.getMousePosInWorldCoords());
		}
	}
}

