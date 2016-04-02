using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Fan> fanList;
	List<Guard> guardList;
	// Use this for initialization
	void Start() {
		fanList = new List<Fan>();
		guardList = new List<Guard>();
		for (int i = 0; i < 30; i++) {
			addGuard();
		}
		addFan(new Vector2(3, 0), new Vector2(-1, 0));
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	// NOTE: Can definitely come up with a better way to do this so we don't need seperate for loops for each type of object added

	// register each guard to be notified when new fan is toggled
	void addFan(Vector2 position, Vector2 direction) {
		GameObject fanObj = new GameObject();
		fanObj.transform.position = position;
		Fan fan = fanObj.AddComponent<Fan>();
		foreach (Guard g in guardList) {
			fan.FanToggled += g.onFanToggled;
		}
		fanList.Add(fan);
	}


	// register each guard to be notified when a fan is toggled
	void addGuard() {
		GameObject guardObj = new GameObject();
		guardObj.transform.position = new Vector2(Random.value * 10 - 5, Random.value * 10 - 5);
		Guard guard = guardObj.AddComponent<Guard>();
		foreach (Fan fan in fanList) {
			fan.FanToggled += guard.onFanToggled;
		}
		guardList.Add(guard);
	}
}

