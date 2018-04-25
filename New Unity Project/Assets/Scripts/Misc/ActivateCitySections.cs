using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCitySections : MonoBehaviour {

	public List <GameObject> toActivate = new List<GameObject>();
	public float sectionSpawnTimer = 0;
	float currentTime = 0;
	
	// Update is called once per frame
	void Update () {
		//destroy this script if it is not in use
		if (toActivate.Count == 0) {
			Destroy (this);
		}

		//delay spawing to timer
		if (currentTime <= sectionSpawnTimer) {
			currentTime++;
		} else {
			//set object to active and then remove it from list
			currentTime = 0;
			int random = Random.Range (0, toActivate.Count - 1);
			toActivate [random].SetActive (true);
			toActivate.RemoveAt (random);
		}
	}
}