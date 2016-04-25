using UnityEngine;
using System.Collections;

public class ZombieSelection : MonoBehaviour {
	private bool mouseDown = false;
	private bool mouseClicked = false;
	private bool rightClick = false;
	private bool minimapSelection = false;
	private Vector2 mouseStart = Vector2.zero;
	private Vector2 screenStart = Vector2.zero;
	private GameObject selectionBox;

	private Camera minimap;

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

	public void init(Camera minimapCam) {
		minimap = minimapCam;
	}

	public Collider2D[] getSelectedObjects() {
		mouseClicked = false;
		Vector2 mousePos;
		Vector2 screenPos = Input.mousePosition;

		if (state == SelectionState.Idle) {
			minimapSelection = onMinimap(screenPos);
		}

		if (minimapSelection) {
			mousePos = minimap.ScreenToWorldPoint(Input.mousePosition);
		}
		else {
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		bool currentMouse = false;
		if (Input.GetMouseButton(0)) {
			currentMouse = true;
			rightClick = false;
		}
		else if (Input.GetMouseButton(1)) {
			currentMouse = true;
			rightClick = true;
		}
		if (rightClick) {
			if (currentMouse && !mouseDown) {
				mouseStart = mousePos;
				screenStart = screenPos;
				mouseDown = true;
			}
			else if (!currentMouse && mouseDown) {
				float distMoved = Vector2.Distance(screenStart, screenPos);
				if (distMoved <= 2) {
					mouseClicked = true;
				}
				mouseDown = false;
			}
		}
		else {
			if (currentMouse) {
				if (mouseDown) {
					state = SelectionState.Selecting;
					return selectArea(mouseStart, mousePos);
				}
				else {
					state = SelectionState.BeginningSelection;
					mouseDown = true;
					mouseStart = mousePos;
					screenStart = screenPos;
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
		if (onMinimap(Input.mousePosition)) {
			return minimap.ScreenToWorldPoint(Input.mousePosition);
		}
		else {
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	public static bool onMinimap(Vector2 pos) {
		int maxY = (int)(.3f * Screen.height);
		if (pos.x <= maxY && pos.y <= maxY) {
			return true;
		}
		else {
			return false;
		}
	}
}

