using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retail : Business {

	public List<ItemProduct> products;

	void Start() {
		foreach(ItemProduct prod in gameObject.GetComponents<ItemProduct>()) {
			products.Add (prod);
		}
		for(int a = 0; a < products.Count; a++) {
			products [a].index = a;
		}
	}

	protected override bool charge (purchaseOptions data)
	{
		if (data.buyer.cash >= products [data.index].cost) {


			if (data.delivery) {
				
				for (int a = 0; a < occupations.Count; a++) {
					if (occupations [a].delivery) {
						if (occupations [a].open) {
							deliveryInfo details = new deliveryInfo ();
							details.items = new List<item> ();
							details.items.Add (products [data.index].data);
							details.recipient = data.buyer;
							((Delivery)occupations [a]).deliveryJobs.Add (details);

							data.buyer.cash -= products [data.index].cost;
							data.buyer.waitingForDelivery++;
							GameObject.FindObjectOfType<CurrentTime> ().addPurchase ();

							cash += products [data.index].cost;
							return true;
						}
					}
				}

			} else if (inStore && !data.delivery) {
				data.buyer.cash -= products [data.index].cost;
				data.buyer.inventory.Add(products [data.index].data);
				GameObject.FindObjectOfType<CurrentTime> ().addPurchase ();
				cash += products [data.index].cost;
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
				if (prod.cost <= data.person.cash) {
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
		arrangeProduct (returnVal);
		return returnVal;
	}

	protected override void arrangeProduct (List<Product> data)
	{
		bool more = false;
		do {
			more = false;
			for (int a = 0; a < data.Count - 1; a++) {
				if (((ItemProduct)data [a]).data.effect < ((ItemProduct)data [a + 1]).data.effect) {
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
		switch (((ItemProduct)one).data.itemType) {
		case itemTypes.food:
			return ((ItemProduct)one).data.effect < (person.maxFood - person.food);
			break;
		case itemTypes.energy:
			return ((ItemProduct)one).data.effect < (person.maxEnergy - person.energy);
			break;
		case itemTypes.happiness:
			return ((ItemProduct)one).data.effect < (person.maxHappiness - person.happiness);
			break;
		}
		return false;
	}

	protected override bool compareCost (Product one, HumanLife person)
	{
		return (((ItemProduct)one).cost <= person.cash);
	}

	protected override bool checkProduct (Product one)
	{
		return true;
	}

	protected override void dailyCheck (Vector3 currentDate)
	{
		return;
	}
}