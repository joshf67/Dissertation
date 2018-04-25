﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Housing : Business {
	public List<HousingProduct> houses;

	public bool ownsAllHomes = false;

	void Start() {
		if (ownsAllHomes) {
			foreach(HousingProduct prod in GameObject.FindObjectsOfType<HousingProduct>()) {
				houses.Add (prod);
			}
		} else {
			foreach (HousingProduct prod in gameObject.GetComponentsInChildren<HousingProduct>()) {
				houses.Add (prod);
			}
		}
		for(int a = 0; a < houses.Count; a++) {
			houses[a].data.position = transform.position;
			houses [a].data.obj = gameObject;
			houses [a].data.ownerScript = this;
			houses [a].data.ownerScriptIndex = a;
			houses [a].data.cost = houses [a].cost;
			houses [a].index = a;
		}
	}

	protected override bool charge (purchaseOptions data)
	{
		//CHANGE THIS TO SOMETHING THAT TAKES MULTIPLE CASH IN
		if (compareCost(houses[data.index], data.buyer)) {
			for (int a = 0; a < data.buyer.stats.accomodation.occupants.Count; a++) {
				data.buyer.stats.accomodation.occupants [a].cash -= houses [data.index].data.initialCost / data.buyer.stats.accomodation.occupants.Count;
				houses [data.index].data.occupants.Add (data.buyer.stats.accomodation.occupants [a]);
				data.buyer.stats.accomodation.occupants [a].stats.accomodation.homeData = houses [data.index].data;
				data.buyer.stats.accomodation.occupants [a].stats.accomodation.home = houses [data.index].gameObject;
			}
			cash += houses [data.index].data.initialCost;
			return true;
		}
		return false;
	}

	protected override List<Product> searchForProduct(shopTest data) {
		//setup return value
		List<Product> returnVal = new List<Product> ();
		//loop through all products
		foreach (HousingProduct prod in houses) {
			if (prod.type == data.productType) {
				//check if product is equal to item type
				if (prod.data.occupants.Count == 0) {
					if (compareProduct(prod, data.person)) {
						if (prod.data.rooms.Count >= data.person.stats.accomodation.requiredRooms) {
							returnVal.Add (prod);
						} 
					}
				}
			}
		}
		arrangeProduct (returnVal);
		return returnVal;
	}

	protected override void arrangeProduct (List<Product> data)
	{
		bool more = false;
		do {
			more = false;
			for (int a = 0; a < data.Count - 1; a++) {
				if (((HousingProduct)data [a]).data.cost < ((HousingProduct)data [a + 1]).data.cost) {
					Product tempProduct = data [a];
					data [a] = data [a + 1];
					data [a + 1] = tempProduct;
					more = true;
				}
			}
		} while (more);
	}

	protected override bool compare (Product one, HumanLife person)
	{
		switch (one.type) {
		case itemTypes.houses:
			return ((HousingProduct)one).data.rooms.Count >= person.stats.accomodation.requiredRooms;
		}
		return false;
	}

	protected override bool compareCost (Product one, HumanLife person)
	{
		float allCash = 0;
		float allCashMonth = 0;
		for (int a = 0; a < person.stats.accomodation.occupants.Count; a++) {
			allCashMonth += person.stats.accomodation.occupants [a].incomePerMonth;
			allCash += person.stats.accomodation.occupants [a].cash;
		}
		allCashMonth /= 2;
		allCash /= 2;

		return (one.cost <= allCashMonth && ((HousingProduct)one).data.initialCost <= allCash);
	}

	protected override bool checkProduct (Product one)
	{
		return (houses [one.index].data.occupants.Count == 0);
	}

	protected override void dailyCheck (Vector3 currentDate)
	{
		for (int a = 0; a < houses.Count; a++) {
			if (houses [a].data.costDays == currentDate) {
				houses [a].data.paymentMade = true;
				if (houses [a].data.occupants != null) {
					for (int b = 0; b < houses [a].data.occupants.Count; a++) {
						houses [a].data.occupants [b].cash -= houses [a].data.cost / houses [a].data.occupants.Count;
					}
					cash += houses [a].data.cost;
				}
			} else {
				houses [a].data.paymentMade = false;
			}
		}
	}

}