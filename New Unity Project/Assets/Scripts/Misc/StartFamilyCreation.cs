using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFamilyCreation : MonoBehaviour {

	public int requiredRooms = 1;

	void Start() {
		HumanLife[] temp = gameObject.GetComponentsInChildren<HumanLife> ();
		for (int a = 0; a < temp.Length; a++) {
			for (int b = 0; b < temp.Length; b++) {
				temp [a].stats.accomodation.occupants.Add (temp[b]);
			}
			temp [a].stats.accomodation.requiredRooms = requiredRooms;
		}
	}
}
