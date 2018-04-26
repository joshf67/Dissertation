using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Housing : Business {
	//store all house products
	public List<HousingProduct> houses;

	public bool ownsAllHomes = false;

	protected override void UpdateBusiness () 
	{
		bool listHasUpdated = false;

		//if this business owns all homes grab all homes and add them to products
		if (ownsAllHomes) {
			//loop through all homes checking if they aren't in the current list
			foreach(HousingProduct prod in GameObject.FindObjectsOfType<HousingProduct>()) {
				if (!houses.Contains (prod)) {
					houses.Add (prod);
					listHasUpdated = true;
				}
			}
		} else {
			//else add any house that is a child of this object
			foreach (HousingProduct prod in gameObject.GetComponentsInChildren<HousingProduct>()) {
				if (!houses.Contains (prod)) {
					houses.Add (prod);
					listHasUpdated = true;
				}
			}
		}
		//check if the product list has been updated
		if (listHasUpdated) {
			//loop through all house products
			for (int a = 0; a < houses.Count; a++) {
				//setup house product data
				houses [a].data.position = transform.position;
				houses [a].data.obj = gameObject;
				houses [a].data.ownerScript = this;
				houses [a].data.ownerScriptIndex = a;
				houses [a].data.cost = houses [a].cost;
				houses [a].index = a;
			}
		}
	}

	//function that tests and charges if it possible to buy items
	protected override bool Charge (PurchaseOptions data)
	{
		//test if the buyer can afford the current product
		if (CompareCost(houses[data.index], data.buyer)) {
			//loop through all current occupants of purchase data and setup new house
			for (int a = 0; a < data.buyer.stats.accomodation.occupants.Count; a++) {
				//remove cash from every buyer equally
				data.buyer.stats.accomodation.occupants [a].cash -=
					houses [data.index].data.initialCost / data.buyer.stats.accomodation.occupants.Count;
				houses [data.index].data.occupants.Add (data.buyer.stats.accomodation.occupants [a]);
				data.buyer.stats.accomodation.occupants [a].stats.accomodation.homeData = houses [data.index].data;
				data.buyer.stats.accomodation.occupants [a].stats.accomodation.home = houses [data.index].gameObject;
			}
			//add purchase cost to business cash
			cash += houses [data.index].data.initialCost;
			return true;
		}
		return false;
	}

	//function to search for specific products
	protected override List<Product> SearchForProduct(ShopTest data) {
		//setup return value
		List<Product> returnVal = new List<Product> ();
		//loop through all products
		foreach (HousingProduct prod in houses) {
			if (prod.type == data.productType) {
				//check if product is equal to item type
				if (prod.data.occupants.Count == 0) {
					//test if the house is active
					if (prod.data.obj.activeSelf) {
						if (CompareProduct (prod, data.person)) {
							if (prod.data.rooms.Count >= data.person.stats.accomodation.requiredRooms) {
								returnVal.Add (prod);
							} 
						}
					}
				}
			}
		}
		ArrangeProduct (returnVal);
		return returnVal;
	}

	//function to sort products by best effect first
	protected override void ArrangeProduct (List<Product> data)
	{
		bool more = false;

		//loop until list is sorted
		do {
			more = false;
			//loop through list elements
			for (int a = 0; a < data.Count - 1; a++) {
				//test if the current product is worse than the next one
				if (((HousingProduct)data [a]).data.cost < ((HousingProduct)data [a + 1]).data.cost) {
					//swap the products around
					Product tempProduct = data [a];
					data [a] = data [a + 1];
					data [a + 1] = tempProduct;
					more = true;
				}
			}
		} while (more);
	}

	protected override bool Compare (Product one, HumanLife person)
	{
		switch (one.type) {
		case itemTypes.houses:
			return ((HousingProduct)one).data.rooms.Count >= person.stats.accomodation.requiredRooms;
		}
		return false;
	}

	//test if the person can afford the product
	protected override bool CompareCost (Product one, HumanLife person)
	{
		float allCash = 0;
		float allCashMonth = 0;

		//add up all buyers income per month
		for (int a = 0; a < person.stats.accomodation.occupants.Count; a++) {
			allCashMonth += person.stats.accomodation.occupants [a].incomePerMonth;
			allCash += person.stats.accomodation.occupants [a].cash;
		}

		//half the potential cost so the buyers can afford other items
		allCashMonth /= 2;
		allCash /= 2;

		return (one.cost <= allCashMonth && ((HousingProduct)one).data.initialCost <= allCash);
	}

	//check if a house has any occupants
	protected override bool CheckProduct (Product one)
	{
		return (houses [one.index].data.occupants.Count == 0);
	}

	//function that occurs every ingame day
	protected override void DailyCheck (Vector3 currentDate)
	{
		//loop through every house
		for (int a = 0; a < houses.Count; a++) {
			//check if the day is the same day as rent
			if (houses [a].data.costDays == currentDate) {
				//set rent to true so cost only happens once
				houses [a].data.paymentMade = true;
				//take cash off of all occupants and add it to current money
				if (houses [a].data.occupants != null) {
					for (int b = 0; b < houses [a].data.occupants.Count; a++) {
						houses [a].data.occupants [b].cash -= houses [a].data.cost / houses [a].data.occupants.Count;
					}
					cash += houses [a].data.cost;
				}
			} else {
				//reset rent after above
				houses [a].data.paymentMade = false;
			}
		}
	}

}