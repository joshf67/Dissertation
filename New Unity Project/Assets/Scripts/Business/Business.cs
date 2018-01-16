using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Business : MonoBehaviour {

	public float cash;
	public float rent;

	public List<product> products;

	public bool online;
	//public List<HumanLife> customersOnline;

	public bool inStore;
	//public List<HumanLife> customersInStore;

	public List<Job> occupations;
	public bool open;

	
	// Update is called once per frame
	void Update () {
		//save current day
		float currentDay = GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY.x;

		//loop through all jobs
		foreach (Job job in occupations) {

			//check if current day is the same day as payment day
			if (currentDay == job.paymentDays && job.paymentMade == false) {
				job.paymentMade = true;
				//pay all workers
				foreach (JobData jobData in job.workers) {
					jobData.worker.Pay (this);
				}
			} else if (currentDay != job.paymentDays) {
				job.paymentMade = false;
			}

			//check if business requires workers
			if (job.requiredWorkers > 0) {
				if (job.jobSearchTime <= 0) {
					if (job.applications.Count != 0) {
						//test all current applications
						testApplications (job);
					}
				} else {
					job.jobSearchTime -= Time.deltaTime;
				}
			}

			//check if job is open
			pollWorkers (job);

		}

		//check if any job in business is open
		open = testOpen();
	}

	bool testOpen() {
		foreach (Job job in occupations) {
			if (job.open || job.automated) {
				return true;
			}
		}
		return false;
	}

	public bool charge(product prod, HumanLife person) {
		if (person.cash >= prod.cost) {
			person.cash -= prod.cost;
			cash += prod.cost;
			return true;
		} else {
			return false;
		}
	}

	public bool chargeDelivery(product prod, HumanLife person) {
		for (int a = 0; a < occupations.Count; a++) {
			if (occupations [a].delivery) {
				deliveryInfo details = new deliveryInfo();
				details.items.Add (prod.data);
				details.recipient = person;
				((Delivery)occupations [a]).deliveryJobs.Add (details);
				return true;
			}
		}
		return false;
	}

	public bool testStats(Stats stat, Job jobDetail) {
		if (stat.age >= jobDetail.requiredStats.age.x && stat.age < jobDetail.requiredStats.age.y) {
			if (stat.intelligence >= jobDetail.requiredStats.intellegence) {
				if (stat.dexterity >= jobDetail.requiredStats.dexterity) {
					if (stat.strength >= jobDetail.requiredStats.strength) {
						return true;
					}
				}
			}
		}
		return false;
	}

	void testApplications(Job job) {
		//arrange based on total stats
		bool more = false;
		do {
			//loop through all applications
			for (int a = 0; a < job.applications.Count - 1; a++) {
				//store stats
				Stats currentStats = job.applications [a].stats;
				Stats nextStats = job.applications [a + 1].stats;
				//calculate total score
				float currentStatsTotal = currentStats.dexterity + currentStats.intelligence + currentStats.strength;
				float nextStatsTotal = nextStats.dexterity + nextStats.intelligence + nextStats.strength;

				//test scores
				if (currentStatsTotal < nextStatsTotal) {
					HumanLife temp = job.applications [a + 1];
					job.applications [a + 1] = job.applications [a];
					job.applications [a + 1] = temp;
					more = true;
				}
			}
		} while (more);

		//accept applications
		for (int a = 0; a < job.applications.Count; a++) {
			if (job.requiredWorkers == 0) {
				job.applications.Clear ();
				return;
			}

			job.addWorker (job.applications [0]);

			//setup worker data
			job.applications [0].stats.income = job.pay;
			job.applications [0].stats.company = this;
			job.applications [0].stats.job = job;
			job.applications [0].recalculateSleep ();

			job.applications.RemoveAt (0);
			job.requiredWorkers--;
		}
	}

	/*
	void testApplications() {
		List<int> arranged = new List<int>();
		foreach (HumanLife human in applications) {
			int value = human.stats.intellegence - requiredStats.intellegence;
			value += human.stats.strength - requiredStats.strength;
			value += human.stats.dexterity - requiredStats.dexterity;
			arranged.Add (value);
		}
		for (int a = 0; a < arranged.Count - 1; a++) {
			if (arranged [a] < arranged [a + 1]) {
				int value = arranged [a];
				HumanLife hum = applications [a];

				applications [a] = applications [a + 1];
				arranged [a] = arranged [a + 1];
				arranged [a + 1] = value;
				applications [a + 1] = hum;
			}
		}
		int accepted = 0;
		for (int a = 0; a < requiredWorkers; a++) {
			if (a < applications.Count) {
				if (applications [a].stats.hasJob) {
					applications.RemoveAt (a);
					a--;
				} else {
					workers.Add (applications [a]);
					workers [workers.Count - 1].stats.income = requiredStats.income;
					workers [workers.Count - 1].stats.workHours = requiredStats.workHours;
					workers [workers.Count - 1].stats.hasJob = true;
					workers [workers.Count - 1].stats.company = this;
					workers [workers.Count - 1].recalculateSleep ();
					accepted++;
				}
			} else {
				break;
			}
		}
		for (int a = 0; a < accepted; a++) {
			applications.RemoveAt (0);
		}
		requiredWorkers -= accepted;
	}
	*/

	void pollWorkers(Job job) {
		if (!job.automated) {
			job.open = false;
			foreach (JobData jobData in job.workers) {
				if (jobData.worker.state == 0) {
					job.open = true;
				}
			}
		} else {
			job.open = true;
		}
	}

	public void payCosts() {
		cash -= rent;
	}

	public List<product> searchForItem(int itemType, string itemName = "") {
		//setup return value
		List<product> returnVal = new List<product> ();
		//loop through all products
		foreach (product prod in products) {
			//check if product is equal to item type
			if (prod.data.type == itemType) {
				//check if item name is null
				if (itemName == "") {
					returnVal.Add (prod);
				} else {
					//check if item name is equal to product name
					if (string.Equals(prod.data.name,itemName,System.StringComparison.OrdinalIgnoreCase)) {
						returnVal.Add (prod);
					}
				}
			}
		}
		return returnVal;
	}
}

