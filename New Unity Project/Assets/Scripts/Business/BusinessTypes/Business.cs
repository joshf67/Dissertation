using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Business : MonoBehaviour {

	public float cash;
	public float rent;
	public float rentDays;

	public bool online;
	//public List<HumanLife> customersOnline;

	public bool inStore;
	//public List<HumanLife> customersInStore;

	public List<Job> occupations;
	public bool open;

	public GameObject buildingPosition;

	float currentDay = 0;

	bool firstTick = true;

	// Update is called once per frame
	void Update () {
		if (firstTick) {
			firstTick = false;
			foreach (Job job in gameObject.GetComponents<Job> ()) {
				occupations.Add (job);
			}
		}
		//save current day
		if (currentDay != GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY.x) {
			currentDay = GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY.x;
			dailyCheck (GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY);
			if (currentDay == rentDays) {
				cash -= rent;
			}
		}
		if (cash <= 0) {
			GameObject.Destroy (gameObject);
		}

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

			//test if workers are still alive
			for (int a = 0; a < job.workers.Count; a++) {
				if (job.workers [a].worker == null) {
					job.workers.RemoveAt (a);
					job.requiredWorkers++;
				}
			}

			//check if business requires workers
			if (job.requiredWorkers > 0) {
				if (job.jobSearchTime <= 0) {
					if (job.applications.Count != 0) {
						//test all current applications
						testApplications (job);
					}
					job.jobSearchTime = job.jobSearchResetTime;
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

	void sortApplicationsBasedOnStats(Job job) {
		//arrange based on total stats
		int ops = 0;
		bool more = false;
		do {
			more = false;
			//loop through all applications
			for (int a = 0; a < job.applications.Count - 1; a++) {

				ops++;


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
					job.applications [a] = temp;
					more = true;
				}
			}
		} while (more);
		bool pleaseWork = false;
	}

	void testApplications(Job job) {
		//arrange based on total stats
		sortApplicationsBasedOnStats (job);

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

			job.applications [0].resetApplications ();

			GameObject.FindObjectOfType<CurrentTime> ().addHours(job.hours.workHours);

			job.requiredWorkers--;
		}
	}

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

	//==============IMPORTANT========================
	public virtual bool Buy(purchaseOptions data) {
		return charge (data);
	}

	protected abstract bool charge (purchaseOptions data);


	public virtual List<Product> searchProducts(shopTest data) {
		List<Product> returnVal = searchForProduct (data);
		if (returnVal == null) {
			returnVal = new List<Product> ();
		}
		return returnVal;
	}

	protected abstract List<Product> searchForProduct (shopTest data);
	protected abstract void arrangeProduct (List<Product> data);

	public virtual bool comparePrice(Product one, HumanLife person) {
		return compareCost (one, person);
	}

	protected abstract bool compareCost(Product one, HumanLife person);

	public virtual bool compareProduct(Product one, HumanLife person) {
		return compare (one, person);
	}

	protected abstract bool compare(Product one, HumanLife person);

	public virtual bool testProduct(Product one) {
		return checkProduct (one);
	}

	protected abstract bool checkProduct(Product one);

	protected abstract void dailyCheck(Vector3 currentDate);
	//==============IMPORTANT========================

}