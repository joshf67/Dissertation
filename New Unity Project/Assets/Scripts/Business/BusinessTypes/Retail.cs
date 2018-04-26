using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retail : Business {

	public List<ItemProduct> products;

	void Start() {
		//add all products on this object to local data
		foreach(ItemProduct prod in gameObject.GetComponents<ItemProduct>()) {
			products.Add (prod);
		}
		//setup each products local index
		for(int a = 0; a < products.Count; a++) {
			products [a].index = a;
		}
	}

	//function that tests and charges if it possible to buy items
	protected override bool Charge (PurchaseOptions data)
	{
		//check if the buyer has enough cash
		if (data.buyer.cash >= products [data.index].cost) {

			//check if the type of purchase is delivery
			if (data.delivery) {

				//loop through all jobs
				for (int a = 0; a < occupations.Count; a++) {
					//check if current job is a delivery job
					if (occupations [a].delivery) {
						//check if the current job is open
						if (occupations [a].open) {

							//setup delivery data required
							DeliveryInfo details = new DeliveryInfo ();
							details.items = new List<Item> ();
							details.itemPrices = new List<float> ();
							details.items.Add (products [data.index].data);
							details.recipient = data.buyer;
							details.itemPrices.Add (products [data.index].cost);
							((Delivery)occupations [a]).deliveryJobs.Add (details);

							//remove cash from the buyer and add cash to this store
							data.buyer.cash -= products [data.index].cost;
							data.buyer.waitingForDelivery++;

							cash += products [data.index].cost;
							return true;
						}
					}
				}

			} //if customer is in shop check if the shop has a job that sells instore
			else if (inStore && !data.delivery) {
				//remove cash from the buyer and add cash to this store
				data.buyer.cash -= products [data.index].cost;
				data.buyer.inventory.Add(products [data.index].data);
				cash += products [data.index].cost;
				return true;
			}

		}
		return false;
	}

	//function to search for specific products
	protected override List<Product> SearchForProduct(ShopTest data) {
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
				if (((ItemProduct)data [a]).data.effect < ((ItemProduct)data [a + 1]).data.effect) {
					//swap the products around
					Product tempProduct = data [a];
					data [a] = data [a + 1];
					data [a + 1] = tempProduct;
					more = true;
				}
			}
		} while (more);
	}

	//function to check if products effects are below persons needs
	protected override bool Compare (Product one, HumanLife person)
	{
		switch (((ItemProduct)one).data.itemType) {
		case itemTypes.food:
			return ((ItemProduct)one).data.effect < (person.maxFood - person.food);
		case itemTypes.energy:
			return ((ItemProduct)one).data.effect < (person.maxEnergy - person.energy);
		case itemTypes.happiness:
			return ((ItemProduct)one).data.effect < (person.maxHappiness - person.happiness);
		}
		return false;
	}

	//test if the person can afford the product
	protected override bool CompareCost (Product one, HumanLife person)
	{
		return (((ItemProduct)one).cost <= person.cash);
	}

	protected override bool CheckProduct (Product one)
	{
		return true;
	}

	protected override void DailyCheck (Vector3 currentDate)
	{
		return;
	}

	protected override void UpdateBusiness () 
	{
		return;
	}
}