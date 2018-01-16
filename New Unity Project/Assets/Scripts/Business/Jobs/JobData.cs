using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobData : MonoBehaviour {
	public HumanLife worker;
	public bool ignoreDistance;

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
