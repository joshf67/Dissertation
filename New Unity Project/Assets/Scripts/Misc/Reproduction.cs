﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that contains how the actors reproduce
public class Reproduction : MonoBehaviour {

	//store basic vairables needed for random
	//mutation and spawning
	public float chanceOfMutation = 10;
	public GameObject baseObject;

	public void Reproduce(HumanLife mate1, HumanLife mate2) {
		//create new child
		GameObject temp = GameObject.Instantiate (baseObject, mate1.transform.position, mate1.transform.rotation);
		HumanLife child = temp.GetComponent<HumanLife> ();

		//reset desire to have kids
		mate1.stats.wantKids = false;
		mate2.stats.wantKids = false;

		//setup accomodation data
		mate1.stats.accomodation.occupants.Add (child);
		mate2.stats.accomodation.occupants.Add (child);
		child.stats.accomodation.occupants.Add (child);
		child.stats.accomodation.occupants.Add (mate1);
		child.stats.accomodation.occupants.Add (mate2);
		child.stats.accomodation.requiredRooms = mate1.stats.accomodation.requiredRooms;

		//setup relationship data
		mate1.stats.relationship.Add(new Relation(child, 2));
		mate2.stats.relationship.Add(new Relation(child, 2));
		child.stats.relationship.Add(new Relation(mate1, 1));
		child.stats.relationship.Add(new Relation(mate2, 1));

		//calculate intellegence
		if (Random.Range (0, 100) <= chanceOfMutation) {
			child.stats.intelligence = Random.Range (0, (mate1.stats.intelligence + mate2.stats.intelligence) / 2);
		} else {
			child.stats.intelligence = (mate1.stats.intelligence + mate2.stats.intelligence) / 2;
		}

		//calculate strength
		if (Random.Range (0, 100) <= chanceOfMutation) {
			child.stats.strength = Random.Range (0, (mate1.stats.strength + mate2.stats.strength) / 2);
		} else {
			child.stats.strength = (mate1.stats.strength + mate2.stats.strength) / 2;
		}

		//calculate dexterity
		if (Random.Range (0, 100) <= chanceOfMutation) {
			child.stats.dexterity = Random.Range (0, (mate1.stats.dexterity + mate2.stats.dexterity) / 2);
		} else {
			child.stats.dexterity = (mate1.stats.dexterity + mate2.stats.dexterity) / 2;
		}
	}
}
