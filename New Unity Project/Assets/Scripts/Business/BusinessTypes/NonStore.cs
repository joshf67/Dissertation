using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonStore : Business {

	//This class creates an empty business that does nothing,
	//however due to needing to override the functions below
	//these empty functions have been created

	protected override void UpdateBusiness () 
	{
		return;
	}

	protected override bool Charge (PurchaseOptions data)
	{
		return false;
	}

	protected override List<Product> SearchForProduct(ShopTest data) {
		return new List<Product> ();
	}

	protected override void ArrangeProduct (List<Product> data)
	{
		return;
	}

	protected override bool Compare (Product one, HumanLife person)
	{
		return false;
	}

	protected override bool CompareCost (Product one, HumanLife person)
	{
		return false;
	}

	protected override bool CheckProduct (Product one)
	{
		return false;
	}

	protected override void DailyCheck (Vector3 currentDate)
	{
		return;
	}

}