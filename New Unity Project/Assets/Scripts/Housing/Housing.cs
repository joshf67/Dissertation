using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Housing : MonoBehaviour {
	public List<House> houses;

	void Start() {
		for(int a = 0; a < houses.Count; a++) {
			//houses [a].Setup (transform.position, gameObject, this, a);
			houses[a].position = transform.position;
			houses [a].obj = gameObject;
			houses [a].ownerScript = this;
			houses [a].ownerScriptIndex = a;
		}
	}

	public List<House> findHouse(float cash, int rooms = 0) {
		List<House> available = new List<House>();
		foreach (House home in houses) {
			if (home.occupants.Count == 0) {
				if (home.cost < cash) {
					if (home.rooms.Count >= rooms) {
						available.Add (home);
					}
				}
			}
		}
		return available;
	}


}