using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Business : MonoBehaviour {

	//basic vairables for businesses
	public float cash;
	public float rent;
	public float rentDays;
	public bool online;
	public bool inStore;

	//vairables for business jobs
	public List<Job> occupations;
	public bool open;

	//variable for where the enterance is
	public GameObject buildingPosition;

	//store current day and first tick
	float currentDay = 0;
	bool firstTick = true;

	// Update is called once per frame
	void Update () {
		//check if this is the first tick since start and add all jobs to list
		if (firstTick) {
			firstTick = false;
			foreach (Job job in gameObject.GetComponents<Job> ()) {
				occupations.Add (job);
			}
		}

		UpdateBusiness ();

		//save current day and do daily check
		if (currentDay != GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY.x) {
			currentDay = GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY.x;
			DailyCheck (GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY);

			//pay rent on rent day
			if (currentDay == rentDays) {
				cash -= rent;
			}
		}

		//check if the business has failed
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
						TestApplications (job);
					}
					job.jobSearchTime = job.jobSearchResetTime;
				} else {
					job.jobSearchTime -= Time.deltaTime;
				}
			}

			//check if job is open
			PollWorkers (job);

		}

		//check if any job in business is open
		open = TestOpen();
	}

	bool TestOpen() {
		//loop through all jobs and check if business is open
		foreach (Job job in occupations) {
			if (job.open || job.automated) {
				return true;
			}
		}
		return false;
	}

	//test actors stats against required job stats
	public bool TestStats(Stats stat, Job jobDetail) {
		//return true if actor has stats above required
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

	void SortApplicationsBasedOnStats(Job job) {
		//arrange based on total stats
		bool more = false;
		do {
			more = false;
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
					job.applications [a] = temp;
					more = true;
				}
			}
		} while (more);
	}

	void TestApplications(Job job) {
		//arrange based on total stats
		SortApplicationsBasedOnStats (job);

		//accept applications
		for (int a = 0; a < job.applications.Count; a++) {
			if (job.requiredWorkers == 0) {
				job.applications.Clear ();
				return;
			}

			job.AddWorker (job.applications [0]);

			//setup worker data
			job.applications [0].stats.income = job.pay;
			job.applications [0].stats.company = this;
			job.applications [0].stats.job = job;
			job.applications [0].recalculateSleep ();

			job.applications [0].resetApplications ();

			job.requiredWorkers--;
		}
	}

	//test if job currently has workers
	void PollWorkers(Job job) {
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

	//pay the rent cost
	public void PayCosts() {
		cash -= rent;
	}

	//==============IMPORTANT========================
	//the functions below are all virtual/abstract functions
	//that classes that inherit from business require to
	//work with other systems implemented within humanLife
	//most of the functions have two types an abstract and virtual
	//this is to get around most derived functions not being called

	public virtual bool Buy(PurchaseOptions data) {
		return Charge (data);
	}

	protected abstract bool Charge (PurchaseOptions data);

	protected abstract void UpdateBusiness ();

	public virtual List<Product> SearchProducts(ShopTest data) {
		List<Product> returnVal = SearchForProduct (data);
		if (returnVal == null) {
			returnVal = new List<Product> ();
		}
		return returnVal;
	}

	protected abstract List<Product> SearchForProduct (ShopTest data);
	protected abstract void ArrangeProduct (List<Product> data);

	public virtual bool ComparePrice(Product one, HumanLife person) {
		return CompareCost (one, person);
	}

	protected abstract bool CompareCost(Product one, HumanLife person);

	public virtual bool CompareProduct(Product one, HumanLife person) {
		return Compare (one, person);
	}

	protected abstract bool Compare(Product one, HumanLife person);

	public virtual bool TestProduct(Product one) {
		return CheckProduct (one);
	}

	protected abstract bool CheckProduct(Product one);

	protected abstract void DailyCheck(Vector3 currentDate);
	//==============IMPORTANT========================

}