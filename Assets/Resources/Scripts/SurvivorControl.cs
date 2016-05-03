using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurvivorControl : MonoBehaviour {
	GameManager gm;
	List<Survivor> survivorList;
	ControlPoint[] controlPoints;

	// Use this for initialization
	public void init (GameManager m) {
		
		gm = m;

		survivorList = gm.getSurvivorList();
		controlPoints = gm.getControlPoints();

		name = "Survivor Control";
		foreach (Survivor s in survivorList) {
			s.targetPositions = gm.getPath(s.tile, controlPoints[gm.findQuadrant(s.tile) - 1], false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		survivorList = gm.getSurvivorList();
		assignSurvivors();
	}

	public float getSecurityLevel(ControlPoint cp) {
		float survivorCount = cp.getIncomingSurvivorCount();
		float zombieCount = gm.countZombiesInArea(cp.posX, cp.posY, 6) + 1;
		return 6 * survivorCount / zombieCount + (cp.currentOwner == ControlPoint.Owner.Survivor ? .25f : -.25f);
	}

	public void assignSurvivors() {
		float[] zombieCount = new float[4];
		float[] target = new float[4];
		float[] numAssigned = new float[4];
		float totalZombies = 0;
		for (int i = 0; i < 4; i++) {
			zombieCount[i] = gm.countZombiesInArea(controlPoints[i].posX, controlPoints[i].posY, 6) + 1;
			totalZombies += zombieCount[i];
		}
		for (int i = 0; i < 4; i++) {
			target[i] = .15f * (float)survivorList.Count + (zombieCount[i] / totalZombies) * .4f * (float)survivorList.Count;
			numAssigned[i] = (float)controlPoints[i].getIncomingSurvivorCount();
		}
		bool canSendSurvivors = true;
		int numSurvivorsSent = 0;
		while (canSendSurvivors && numSurvivorsSent++ < 50) {
			int maxSourceIndex = 0;
			int maxDestIndex = 0;
			float minDiffChange = 0;
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (i != j && controlPoints[i].getIncomingSurvivors().Count > 0) {
						float oldSourceDiff = Mathf.Abs(target[i] - numAssigned[i]);
						float newSourceDiff = Mathf.Abs(target[i] - (numAssigned[i] - 1));
						float oldDestDiff = Mathf.Abs(target[j] - numAssigned[j]);
						float newDestDiff = Mathf.Abs(target[j] - (numAssigned[j] + 1));
//						print("Source diff at " + i + " before: " + oldSourceDiff + " after: " + newSourceDiff);
						float diffChange = (newSourceDiff + newDestDiff) - (oldSourceDiff + oldDestDiff);
						if (diffChange < minDiffChange) {
							maxSourceIndex = i;
							maxDestIndex = j;
							minDiffChange = diffChange;
						}
					}
				}
			}
			if (maxSourceIndex != 0 || maxDestIndex != 0) {
				sendReinforcements (controlPoints [maxSourceIndex], controlPoints [maxDestIndex]);
				numAssigned [maxSourceIndex]--;
				numAssigned [maxDestIndex]++;
			} 
			else {
				canSendSurvivors = false;
			}
//			int minSecurityIndex = 0;
//			int maxSecurityIndex = 0;
//			for (int i = 1; i < 4; i++) {
//				if (security[i] < security[minSecurityIndex]) {
//					minSecurityIndex = i;
//				}
//				if (security[i] > security[maxSecurityIndex]) {
//					maxSecurityIndex = i;
//				}
//			}
//			print("Min: " + security[minSecurityIndex]);
//			print("Max: " + security[maxSecurityIndex]);
////			if (security[minSecurityIndex] >= 1) {
////				canSendSurvivors = false;
////			}
////			else {
//			float newMinSecurity = security[minSecurityIndex] + (6f / zombieCount[minSecurityIndex]);
//			float newMaxSecurity = security[maxSecurityIndex] - (6f / zombieCount[maxSecurityIndex]);
//			if (newMaxSecurity < newMinSecurity) {
//				canSendSurvivors = false;
//				print("Stopping to avoid loop.");
//			}
//			else if (newMaxSecurity <= 1) {
//				canSendSurvivors = false;
//				print("Max dropping below 1, exiting.");
//			}
//			else {
//				sendReinforcements(controlPoints[maxSecurityIndex], controlPoints[minSecurityIndex]);
//				security[minSecurityIndex] = newMinSecurity;
//				security[maxSecurityIndex] = newMaxSecurity;
//			}
//			}
		}
	}

	public void sendReinforcements(ControlPoint source, ControlPoint dest){
//		print("Sending reinforcements from " + source.transform.position + " to " + dest.transform.position);
		List<Survivor> survivors = source.getIncomingSurvivors();
		Survivor closestSurvivor = null;
		float minDist = float.MaxValue;
		for (int i = survivors.Count - 1; i >= 0; i--) {
			Survivor s = survivors[i];
			if (s == null) {
				source.removeIncomingSurvivor(s);
				survivorList.Remove(s);
			}
			else {
				float dist = Vector2.Distance(s.transform.position, dest.transform.position);
				if (dist < minDist) {
					minDist = dist;
					closestSurvivor = s;
				}
			}
		}
		if (closestSurvivor != null) {
			closestSurvivor.setDestination(dest);
			Debug.DrawLine(closestSurvivor.transform.position, dest.transform.position);
		}
		else {
			print("Unable to send survivor: null");
		}
	}
}
