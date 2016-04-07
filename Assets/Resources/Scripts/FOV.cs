using UnityEngine;
using System.Collections;

public class FOV : MonoBehaviour {

	// Use this for initialization
	public void init(float size) {
		SpriteRenderer rend = gameObject.AddComponent<SpriteRenderer>();
		rend.sprite = Resources.Load<Sprite>("Sprites/FOV");
		rend.sortingLayerName = "Foreground";
		rend.sortingOrder = 1;
		rend.color = new Color(.43137f, .7294f, .94118f, .2f);
		transform.localScale = Vector3.one * size;
		transform.localPosition = Vector3.zero;
	}
	public void setDirection(Vector2 dir) {
		float angle = Mathf.Atan2(dir.y, dir.x);
		transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle - 90);
	}
}

