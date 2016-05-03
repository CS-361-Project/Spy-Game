using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ZombieControl : MonoBehaviour {
	ZombieSelection zombieSelector;
	GameManager gm;
	HashSet<Guard> selection;

	GameObject cursor;
	SpriteRenderer cursorRend;
	bool attackSurvivors;

	// Use this for initialization
	public void init(GameManager g) {
		gm = g;
		zombieSelector = new GameObject().AddComponent<ZombieSelection>();
		zombieSelector.gameObject.name = "Zombie Selection";
		Minimap minimap = GameObject.Find("Minimap Camera").GetComponent<Minimap>();
		zombieSelector.init(minimap.cam);
		selection = new HashSet<Guard>();
		name = "Zombie Control";



		//Set up Cursor

		cursor = new GameObject();
		cursorRend = cursor.AddComponent<SpriteRenderer>();
		cursorRend.sprite = Resources.Load<Sprite>("Sprites/Mouse");
		cursorRend.sortingLayerName = "UI";
		cursorRend.sortingOrder = 4;
		cursor.layer = LayerMask.NameToLayer("Mouse");
		cursorRend.color = Color.red;

		cursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public void removeZombie(Guard g) {
		selection.Remove(g);
	}
	// Update is called once per frame
	void Update() {

		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursor.transform.position = mousePos;
		cursorRend.transform.localScale = new Vector3(1,1,1) *  Camera.main.orthographicSize * .02f;


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

		attackSurvivors = Input.GetKey (KeyCode.F);
		if (zombieSelector.getMouseClicked()) {
			bool overwritePath = !Input.GetKey (KeyCode.LeftShift);

			gm.moveTo (selection.ToList (), zombieSelector.getMousePosInWorldCoords (), overwritePath, attackSurvivors);
			roar();

		}

		if (selection.Count != 0) {
			if (!gm.audioCtrl.getPointSource().isPlaying) {
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieHoard, gm.audioCtrl.getPointSource());
			}
		}
		else {
			if (gm.audioCtrl.getPointSource().isPlaying) {
				gm.audioCtrl.getPointSource().Stop();
			}
		}
		if (attackSurvivors) {
			Cursor.visible = false;
			cursorRend.enabled = true;
		} else {
			Cursor.visible = true;
			cursorRend.enabled = false;
		}
	}

	void roar(){
		int rand = Random.Range(0, 3);
		switch (rand) {
			case 0:
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieRoar1, gm.audioCtrl.getClickSource());
				break;
			case 1: 
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieRoar2, gm.audioCtrl.getClickSource());
				break;
			case 2:
				gm.audioCtrl.playClip((int)AudioControl.clips.zombieRoar3, gm.audioCtrl.getClickSource());
				break;
			default:
				print("huh? this shouldn't happen.");
				break;
		}

	}
}

