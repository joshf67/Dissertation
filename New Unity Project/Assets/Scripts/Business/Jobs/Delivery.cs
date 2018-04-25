using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : Job {

	public List<deliveryInfo> deliveryJobs;

	void Update() {
		if (open == false) {
			int delivJobs = deliveryJobs.Count;
			for (int a = 0; a < delivJobs; a++) {
				deliveryJobs [0].recipient.waitingForDelivery = 0;
				for (int b = 0; b < deliveryJobs [0].items.Count; b++) {
					deliveryJobs [0].recipient.cash += deliveryJobs [0].itemPrices [b];
				}
				deliveryJobs.RemoveAt (0);
			}
		}
	}

	public override bool work (HumanLife worker)
	{
		deliveryDetails workerStats = (deliveryDetails)worker.stats.jobData;
		if (workerStats.deliveryToBeMade) {
			if (workerStats.deliveryItems.Count != 0) {
				if (workerStats.startDeliveryTime <= 0) {
					if (workerStats.recipient) {
						if (Functions.checkDistance (worker.agent, workerStats.destination, worker.minDist)) {
							//add all delivery items to recipient
							foreach (item Item in workerStats.deliveryItems) {
								workerStats.recipient.inventory.Add (Item);
							}

							//reset delivery details
							workerStats.deliveryItems.Clear ();
							workerStats.recipient.waitingForDelivery--;
							workerStats.recipient = null;
						}
					} else {
						workerStats.deliveryItems.Clear ();
						workerStats.recipient = null;
					}
				} else {
					workerStats.startDeliveryTime -= Time.deltaTime;
				}

			} else {
				if (Functions.checkDistance (worker.agent, worker.stats.company.buildingPosition.transform.position, worker.minDist)) {
					//reset delivery data when worker back at work
					workerStats.deliveryToBeMade = false;
					workerStats.ignoreDistance = false;
				}
			}
		} else {
			if (deliveryJobs.Count > 0) {
				if (deliveryJobs [0].recipient) {
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

	public override void addWorker (HumanLife worker)
	{
		workers.Add (ScriptableObject.CreateInstance <deliveryDetails>());
		worker.stats.jobData = workers [workers.Count - 1];
		workers [workers.Count - 1].worker = worker;
	}

}