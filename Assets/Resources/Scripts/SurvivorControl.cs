using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurvivorControl : MonoBehaviour {
	GameManager gm;
	List<Survivor> survivorList;

	// Use this for initialization
	public void init (GameManager m) {
		
		gm = m;
		survivorList = gm.getSurvivorList ();
		name = "Survivor Control";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void underAttack(ControlPoint cp){

	}

	public void sendReinforcements(ControlPoint cp){
		Survivor closestSurvivor = null;
		float minDist = float.MaxValue;
		foreach (Survivor s in survivorList) {
			float dist = Vector2.Distance (s.transform.position, cp.transform.position);
			if (dist < minDist) {
				minDist = dist;
				closestSurvivor = s;
			}
		}

	}
}
