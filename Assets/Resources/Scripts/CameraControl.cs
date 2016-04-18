using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	bool dragging = false;
	Vector3 lastPos = Vector3.zero;
	void Update() {
		if (Input.GetMouseButton(1)) {
			if (!dragging) {
				dragging = true;
			}
			else {
				transform.Translate(lastPos - Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			lastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
		else {
			if (dragging) {
				dragging = false;
			}
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0 && Camera.main.orthographicSize >= 1 && Camera.main.orthographicSize <= 50) {
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + 3 * scroll, 1, 50);
		}
	}
}
