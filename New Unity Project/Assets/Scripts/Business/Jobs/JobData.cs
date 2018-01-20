using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class deliveryDetails : JobData {
	public List<item> deliveryItems;
	public Vector3 destination;
	public HumanLife recipient;
	public bool deliveryToBeMade;
	public float startDeliveryTime;

	public deliveryDetails (HumanLife actor) : base(actor) {
	}
}