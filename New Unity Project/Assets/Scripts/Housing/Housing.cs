using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Housing : Business {
	public List<HousingProduct> houses;

	void Start() {
		for(int a = 0; a < houses.Count; a++) {
			houses[a].data.position = transform.position;
			houses [a].data.obj = gameObject;
			houses [a].data.ownerScript = this;
			houses [a].data.ownerScriptIndex = a;
			houses [a].data.cost = houses [a].cost;
		}
	}

	/*
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
	*/

	protected override List<Product> searchForProduct(shopTest data) {
		//setup return value
		List<Product> returnVal = new List<Product> ();
		//loop through all products
		foreach (HousingProduct prod in houses) {
			if (prod.type == data.productType) {
				//check if product is equal to item type
				if (prod.data.occupants.Count == 0) {
					if (prod.data.cost < data.cash) {
						if (prod.data.rooms.Count >= data.person.stats.accomodation.requiredRooms) {
							returnVal.Add (prod);
						} 
					}
				}
			}
		}
		return returnVal;
	}


}