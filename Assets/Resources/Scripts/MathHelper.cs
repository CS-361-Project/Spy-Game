using UnityEngine;
using System.Collections;

public static class MathHelper {

	public static Vector2 rotate90(Vector2 v) {
		float tx = v.x;
		float ty = v.y;
		Vector2 w = new Vector2();
		w.x = -ty;
		w.y = tx;
		return w;
	}

	public static Vector2 rotate(Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
}

