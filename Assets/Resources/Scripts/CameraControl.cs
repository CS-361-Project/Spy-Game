using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	bool dragging = false;
	Vector3 lastPos = Vector3.zero;
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
	}

	void Update() {
		if (Input.GetMouseButton(1)) {
			if (!dragging) {
				dragging = true;
			}
			else {
				transform.Translate(lastPos - Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			lastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			resetLines();
		}
		else {
			if (dragging) {
				dragging = false;
			}
		}
//		float scroll = Input.GetAxis("Mouse ScrollWheel");
//		if (scroll != 0 && Camera.main.orthographicSize >= 1 && Camera.main.orthographicSize <= 50) {
//			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 3 * scroll, 1, 50);
//		}
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
		lines[0].transform.localScale = new Vector2(camDimensions.x * 2, lineThickness);

		lines[1].transform.localPosition = new Vector3(camDimensions.x + lineThickness / 2, 0, z);
		lines[1].transform.localScale = new Vector2(lineThickness, camDimensions.y * 2);

		lines[2].transform.localPosition = new Vector3(0, -(camDimensions.y + lineThickness / 2), z);
		lines[2].transform.localScale = new Vector2(camDimensions.x * 2, lineThickness);

		lines[3].transform.localPosition = new Vector3(-(camDimensions.x + lineThickness / 2), 0, z);
		lines[3].transform.localScale = new Vector2(lineThickness, camDimensions.y * 2);
	}
}
