using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : Job {

	public List<deliveryInfo> deliveryJobs;

	public override bool work (HumanLife worker)
	{
		deliveryDetails workerStats = (deliveryDetails)worker.stats.jobData;
		if (workerStats.deliveryToBeMade) {
			if (workerStats.startDeliveryTime <= 0) {
				if (Functions.checkDistance (worker.agent, workerStats.destination, worker.minDist)) {
					//add all delivery items to recipient
					foreach (item Item in workerStats.deliveryItems) {
						workerStats.recipient.inventory.Add (Item);
					}

					//reset delivery details
					workerStats.deliveryItems.Clear ();
					workerStats.recipient = null;
				}
			} else {
				workerStats.startDeliveryTime -= Time.deltaTime;
			}

			if (workerStats.deliveryItems.Count == 0) {
				if (Functions.checkDistance (worker.agent, worker.stats.job.transform.position, worker.minDist)) {
					//reset delivery data when worker back at work
					workerStats.deliveryToBeMade = false;
				}
			}
		} else {
			if (deliveryJobs.Count > 0) {
				workerStats.deliveryItems = deliveryJobs [0].items;
				workerStats.recipient = deliveryJobs [0].recipient;
				workerStats.deliveryToBeMade = true;
				workerStats.startDeliveryTime = 0;
				deliveryJobs.RemoveAt (0);
			}
		}
		return true;
	}

	public override void addWorker (HumanLife worker)
	{
		workers.Add (new deliveryDetails (worker));
		workers [workers.Count].ignoreDistance = true;
	}

}