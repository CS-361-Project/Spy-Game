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
		if (cursor != null) {
			Destroy(cursor);
		}
		cursor = new GameObject();
		cursorRend = cursor.AddComponent<SpriteRenderer>();
		cursorRend.sprite = Resources.Load<Sprite>("Sprites/Mouse");
		cursorRend.sortingLayerName = "UI";
		cursorRend.sortingOrder = 4;
		cursor.layer = LayerMask.NameToLayer("Mouse");
		cursorRend.color = Color.red;

		cursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursor.transform.position += Vector3.back;
	}

	public void removeZombie(Guard g) {
		selection.Remove(g);
	}
	// Update is called once per frame
	void Update() {

		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursor.transform.position = new Vector3(mousePos.x, mousePos.y, -9);
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
				int preCount = selection.Count;
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
				int postCount = selection.Count;
				if (postCount > preCount) {
					playBeep();
				}
				break;
			case ZombieSelection.SelectionState.Idle:
				break;
		}

		attackSurvivors = !Input.GetKey (KeyCode.F);
		if (zombieSelector.getMouseClicked()) {
			bool overwritePath = !Input.GetKey (KeyCode.LeftShift);

			gm.moveTo (selection.ToList (), zombieSelector.getMousePosInWorldCoords (), overwritePath, attackSurvivors);
			roar();

		}

//		if (selection.Count != 0) {
//			if (!gm.audioCtrl.getSource3().isPlaying) {
//				gm.audioCtrl.playClip((int)AudioControl.Clip.zombieHoard, gm.audioCtrl.getSource3());
//			}
//		}
//		else {
//			if (gm.audioCtrl.getSource3().isPlaying) {
//				gm.audioCtrl.getSource3().Stop();
//			}
//		}
		Cursor.visible = attackSurvivors;
		cursorRend.enabled = !attackSurvivors;
	}

	public void playBeep() {
		gm.audioCtrl.playClip(AudioControl.Clip.beepSound, gm.audioCtrl.getSource3());
	}

	void roar(){
		int rand = Random.Range(0, 3);
		switch (rand) {
			case 0:
				gm.audioCtrl.playClip((int)AudioControl.Clip.zombieRoar1, gm.audioCtrl.getSource1());
				break;
			case 1: 
				gm.audioCtrl.playClip((int)AudioControl.Clip.zombieRoar2, gm.audioCtrl.getSource1());
				break;
			case 2:
				gm.audioCtrl.playClip((int)AudioControl.Clip.zombieRoar3, gm.audioCtrl.getSource1());
				break;
			default:
				print("huh? this shouldn't happen.");
				break;
		}

	}
}

