using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {
	Camera cam;

	// Use this for initialization
	public void init(GameManager m) {
		cam = GetComponent<Camera>();
		cam.orthographicSize = Mathf.Max(m.width/2, m.height/2);
		transform.position = new Vector3(m.width / 2, m.height / 2, -9);
	}
}

