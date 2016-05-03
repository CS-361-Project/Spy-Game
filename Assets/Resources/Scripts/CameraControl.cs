using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	bool dragging = false;
	bool minimapMovement = false;
	Camera minimap;
	Vector2 lastPos = Vector2.zero;
	Vector2 camDimensions;
	float aspect;

	GameObject[] lines;
	float lineThickness = .5f;
	float z;

	LineRenderer lineRend;

	AudioSource audioSource;
	public AudioClip soundTrack;

	void Start(){
		z = -transform.position.z;
		audioSource = gameObject.AddComponent<AudioSource>();
		soundTrack = Resources.Load("Audio/Music/160412 Professor Quack (Long Edit)", typeof(AudioClip)) as AudioClip;
		audioSource.PlayOneShot(soundTrack);

		minimap = GameObject.Find("Minimap Camera").GetComponent<Camera>();

		lines = new GameObject[4];
		for (int i = 0; i < 4; i++) {
			lines[i] = new GameObject();
			SpriteRenderer rend = lines[i].AddComponent<SpriteRenderer>();
			rend.sprite = Resources.Load<Sprite>("Sprites/Box");
			rend.color = Color.white;
			rend.sortingLayerName = "UI";
			lines[i].transform.parent = transform;
			lines[i].transform.localPosition = new Vector3(0, 0, z);
		}

//		lineRend = gameObject.AddComponent<LineRenderer>();
//		lineRend.useWorldSpace = true;
//		lineRend.SetVertexCount(5);
//		lineRend.SetWidth(.2f, .2f);

		aspect = (float)Screen.width / (float)Screen.height;
		camDimensions = new Vector2(Camera.main.orthographicSize * aspect, Camera.main.orthographicSize);
		resetLines();
		Camera.main.orthographic = false;
	}

	void Update() {
		if (Input.GetMouseButton(1)) {
			Vector2 lastPosCopy = lastPos;
			Vector2 mousePos;
			if (!dragging) {
				minimapMovement = ZombieSelection.onMinimap(Input.mousePosition);
				if (minimapMovement) {
					mousePos = minimap.ScreenToWorldPoint(Input.mousePosition);
					transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
					dragging = true;
					lastPos = mousePos;
				}
			}
			else {
				if (minimapMovement) {
					mousePos = minimap.ScreenToWorldPoint(Input.mousePosition);
					transform.Translate(mousePos - lastPos);
					lastPos = mousePos;
					resetLines();
				}
			}
		}
		else {
			if (dragging) {
				dragging = false;
				minimapMovement = false;
			}
		}

		Vector2 moveDirection = getMoveDir();
		if (moveDirection != Vector2.zero) {
			transform.position += (Vector3)moveDirection * Camera.main.orthographicSize * .05f;
		}
	}

	Vector2 getMoveDir() {
		Vector2 result = Vector2.zero;
		if (Input.GetKey(KeyCode.W)) {
			result += Vector2.up;
		}
		if (Input.GetKey(KeyCode.A)) {
			result += Vector2.left;
		}
		if (Input.GetKey(KeyCode.S)) {
			result += Vector2.down;
		}
		if (Input.GetKey(KeyCode.D)) {
			result += Vector2.right;
		}
		return result.normalized;
	}

	void OnGUI() {
		if (Event.current.type == EventType.ScrollWheel) {
			float scroll = Event.current.delta.y;
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 1.5f * scroll, 1, 50);
			camDimensions = new Vector2(Camera.main.orthographicSize * aspect, Camera.main.orthographicSize);
			resetLines();
		}
	}

	void resetLines() {
		float size = Camera.main.orthographicSize;
		lines[0].transform.localPosition = new Vector3(0, camDimensions.y + lineThickness / 2, z);
		lines[0].transform.localScale = new Vector2((camDimensions.x + lineThickness) * 2, lineThickness);

		lines[1].transform.localPosition = new Vector3(camDimensions.x + lineThickness / 2, 0, z);
		lines[1].transform.localScale = new Vector2(lineThickness, (camDimensions.y + lineThickness) * 2);

		lines[2].transform.localPosition = new Vector3(0, -(camDimensions.y + lineThickness / 2), z);
		lines[2].transform.localScale = new Vector2((camDimensions.x + lineThickness) * 2, lineThickness);

		lines[3].transform.localPosition = new Vector3(-(camDimensions.x + lineThickness / 2), 0, z);
		lines[3].transform.localScale = new Vector2(lineThickness, (camDimensions.y + lineThickness) * 2);
	}
}
