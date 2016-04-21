using UnityEngine;
using System.Collections;

public class ZombieSelection : MonoBehaviour {
	private bool mouseDown = false;
	private bool mouseClicked = false;
	private bool rightClick = false;
	private Vector2 mouseStart = Vector2.zero;
	private GameObject selectionBox;

	public SelectionState state;

	public enum SelectionState {
		Selecting, EndingSelection, BeginningSelection, Idle
	};

	void Awake() {
		selectionBox = new GameObject();
		selectionBox.name = "Selection Box";
		SpriteRenderer rend = selectionBox.AddComponent<SpriteRenderer>();
		rend.sortingLayerName = "UI";
//		selectionBox.layer = LayerMask.NameToLayer("UI");
		rend.sprite = Resources.Load<Sprite>("Sprites/Box");
		rend.color = new Color(1, 0, 0, .2f);
		selectionBox.SetActive(false);
	}

	public Collider2D[] getSelectedObjects() {
		mouseClicked = false;
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		bool mouse = false;
		if (Input.GetMouseButton(0)) {
			mouse = true;
			rightClick = false;
		}
		else if (Input.GetMouseButton(1)) {
			mouse = true;
			rightClick = true;
		}
		if (rightClick) {
			if (mouse && !mouseDown) {
				mouseStart = mousePos;
				mouseDown = true;
			}
			else if (!mouse && mouseDown) {
//				if (mousePos == mouseStart) {
				if (Vector2.Distance(mousePos, mouseStart) <= 2) {
					mouseClicked = true;
				}
				mouseDown = false;
			}
		}
		else {
			if (mouse) {
				if (mouseDown) {
					state = SelectionState.Selecting;
					return selectArea(mouseStart, mousePos);
				}
				else {
					state = SelectionState.BeginningSelection;
					mouseDown = true;
					mouseStart = mousePos;
				}
			}
			else {
				if (mouseDown) {
					state = SelectionState.EndingSelection;
				}
				else {
					state = SelectionState.Idle;
				}
				mouseDown = false;
				selectionBox.SetActive(false);
			}
		}
		return new Collider2D[0];
	}

	Collider2D[] selectArea(Vector2 corner1, Vector2 corner2) {
		selectionBox.SetActive(true);
		selectionBox.transform.position = (corner1 + corner2) / 2;
		selectionBox.transform.localScale = corner2 - corner1;
		return Physics2D.OverlapAreaAll(corner1, corner2);
	}

	public bool getMouseClicked() {
		return mouseClicked;
	}

	public Vector2 getMousePosInWorldCoords() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}

