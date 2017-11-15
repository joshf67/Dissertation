﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour {

	public float chanceOfMutation = 10;
	public GameObject baseObject;

	public void reproduce(HumanLife mate1, HumanLife mate2) {
		GameObject temp = GameObject.Instantiate (baseObject, mate1.transform.position, mate1.transform.rotation);
		HumanLife child = temp.GetComponent<HumanLife> ();

		mate1.stats.wantKids = false;
		mate2.stats.wantKids = false;

		//intellegence
		if (Random.Range (0, 100) <= chanceOfMutation) {
			child.stats.intellegence = Random.Range (0, (mate1.stats.intellegence + mate2.stats.intellegence) / 2);
		} else {
			child.stats.intellegence = (mate1.stats.intellegence + mate2.stats.intellegence) / 2;
		}

		//strength
		if (Random.Range (0, 100) <= chanceOfMutation) {
			child.stats.strength = Random.Range (0, (mate1.stats.strength + mate2.stats.strength) / 2);
		} else {
			child.stats.strength = (mate1.stats.strength + mate2.stats.strength) / 2;
		}

		//dexterity
		if (Random.Range (0, 100) <= chanceOfMutation) {
			child.stats.dexterity = Random.Range (0, (mate1.stats.dexterity + mate2.stats.dexterity) / 2);
		} else {
			child.stats.dexterity = (mate1.stats.dexterity + mate2.stats.dexterity) / 2;
		}
	}
}
