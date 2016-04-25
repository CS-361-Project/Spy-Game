using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {
	public Camera cam;

	// Use this for initialization
	public void init(GameManager m) {
		cam = GetComponent<Camera>();
		cam.orthographicSize = Mathf.Max(m.width/2, m.height/2) + 1;
		cam.aspect = (float)m.width / (float)m.height;
		float screenAspect = (float)Screen.width / (float)Screen.height;
		cam.rect = new Rect(0, 0, .3f / screenAspect, .3f);
		transform.position = new Vector3(m.width / 2, m.height / 2, -9);
	}
}

