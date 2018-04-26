using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : Job {

	public List<DeliveryInfo> deliveryJobs;

	//run every tick
	void Update() {
		//check if job is closed
		if (open == false) {
			//gret delivery jobs count
			int delivJobs = deliveryJobs.Count;

			//loop through all delivery jobs and refund the buyer
			for (int a = 0; a < delivJobs; a++) {
				deliveryJobs [0].recipient.waitingForDelivery = 0;
				for (int b = 0; b < deliveryJobs [0].items.Count; b++) {
					deliveryJobs [0].recipient.cash += deliveryJobs [0].itemPrices [b];
				}
				deliveryJobs.RemoveAt (0);
			}
		}
	}

	//function that handles what happens when workers work
	public override bool Work (HumanLife worker)
	{
		//cast workers stats to delivery info
		DeliveryDetails workerStats = (DeliveryDetails)worker.stats.jobData;
		//check if worker has a current delivery
		if (workerStats.deliveryToBeMade) {
			//check if worker has delivery items
			if (workerStats.deliveryItems.Count != 0) {
				//check if worker is able to move
				if (workerStats.startDeliveryTime <= 0) {
					//check if recipient still exists
					if (workerStats.recipient) {
						//travel to recipent
						if (Functions.checkDistance (worker.agent, workerStats.destination, worker.minDist)) {
							//add all delivery items to recipient
							foreach (Item item in workerStats.deliveryItems) {
								workerStats.recipient.inventory.Add (item);
							}

							//reset delivery details
							workerStats.deliveryItems.Clear ();
							workerStats.recipient.waitingForDelivery--;
							workerStats.recipient = null;
						}
					} else {
						//clear current delivery data
						workerStats.deliveryItems.Clear ();
						workerStats.recipient = null;
					}
				} else {
					workerStats.startDeliveryTime -= Time.deltaTime;
				}

			} else {
				//travel to business location
				if (Functions.checkDistance (worker.agent, worker.stats.company.buildingPosition.transform.position, worker.minDist)) {
					//reset delivery data when worker back at work
					workerStats.deliveryToBeMade = false;
					workerStats.ignoreDistance = false;
				}
			}
		} else {
			//check if delivery jobs are available
			if (deliveryJobs.Count > 0) {
				//check if first delivery job has a recipient
				if (deliveryJobs [0].recipient) {
					//setup delivery information
					workerStats.deliveryItems = deliveryJobs [0].items;
					workerStats.recipient = deliveryJobs [0].recipient;
					workerStats.destination = deliveryJobs [0].recipient.transform.position;
					workerStats.deliveryToBeMade = true;
					workerStats.ignoreDistance = true;
					workerStats.startDeliveryTime = 0;
				}
				deliveryJobs.RemoveAt (0);
			}
		}
		return true;
	}

	//function to add worker to list
	public override void AddWorker (HumanLife worker)
	{
		//setup delivery data for worker
		workers.Add (ScriptableObject.CreateInstance <DeliveryDetails>());
		worker.stats.jobData = workers [workers.Count - 1];
		workers [workers.Count - 1].worker = worker;
	}

}