using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class easily sets up a family
public class StartFamilyCreation : MonoBehaviour {

	//store how many rooms in a house are required 
	public int requiredRooms = 1;

	//on start loop through all actors attached to this object
	//and add them to eachothers accomodation data then setup house limits
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
