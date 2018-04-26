using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that stores job information for worker
public class JobData : ScriptableObject {
	public HumanLife worker;
	public bool ignoreDistance;
	public Vector2 workHours;
	public Vector2 breakHours;
	public float pay;

	public JobData (HumanLife actor) {
		worker = actor;
	}
}

//class that stores delivery data for each delivery purchase
public class DeliveryDetails : JobData {
	public List<Item> deliveryItems;
	public Vector3 destination;
	public HumanLife recipient;
	public bool deliveryToBeMade;
	public float startDeliveryTime;

	public DeliveryDetails (HumanLife actor) : base(actor) {
	}
}