using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retail : Business {

	public List<ItemProduct> products;

	public override bool charge(Product prod, HumanLife person) {
		if (person.cash >= prod.cost) {
			person.cash -= prod.cost;
			cash += prod.cost;
			GameObject.FindObjectOfType<CurrentTime> ().addPurchase ();
			return true;
		} else {
			return false;
		}
	}

	public override bool chargeDelivery(Product prod, HumanLife person) {
		for (int a = 0; a < occupations.Count; a++) {
			if (occupations [a].delivery) {
				deliveryInfo details = new deliveryInfo();
				details.items = new List<item> ();
				details.items.Add (((ItemProduct)prod).data);
				details.recipient = person;
				((Delivery)occupations [a]).deliveryJobs.Add (details);
				GameObject.FindObjectOfType<CurrentTime> ().addPurchase ();
				return true;
			}
		}
		return false;
	}

	protected override List<Product> searchForProduct(shopTest data) {
		//setup return value
		List<Product> returnVal = new List<Product> ();
		//loop through all products
		foreach (ItemProduct prod in products) {
			//check if product is equal to item type
			if (prod.type == data.productType) {
				if (prod.cost <= data.cash) {
					//check if item name is null
					if (data.productName == "") {
						returnVal.Add (prod);
					} else {
						//check if item name is equal to product name
						if (string.Equals (prod.data.name, data.productName, System.StringComparison.OrdinalIgnoreCase)) {
							returnVal.Add (prod);
						}
					}
				}
			}
		}
		return returnVal;
	}

}
