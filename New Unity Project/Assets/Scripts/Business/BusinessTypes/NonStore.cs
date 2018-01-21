using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonStore : Business {

	protected override bool charge (purchaseOptions data)
	{
		return false;
	}

	protected override List<Product> searchForProduct(shopTest data) {
		return new List<Product> ();
	}

	protected override void arrangeProduct (List<Product> data)
	{
		return;
	}

	protected override bool compare (Product one, HumanLife person)
	{
		return false;
	}

	protected override bool compareCost (Product one, HumanLife person)
	{
		return false;
	}

	protected override bool checkProduct (Product one)
	{
		return false;
	}

}