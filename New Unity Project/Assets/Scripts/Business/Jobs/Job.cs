using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job : MonoBehaviour {

	//store data about paying workers
	public int paymentDays;
	public bool paymentMade;

	//store data about job information
	public string jobType;
	public float pay;
	public JobDetails hours;

	//store data about job type
	public bool open;
	public bool automated;
	public bool delivery;

	//store data about required workers
	//and time to find new workers
	public int requiredWorkers;
	public float jobSearchResetTime;
	public float jobSearchTime;

	//store data on workers/applicants and required stats
	public JobStats requiredStats;
	public List<HumanLife> applications;
	public List<JobData> workers;

	protected CurrentTime time;

	//grab time at start
	void Start() {
		time = GameObject.FindObjectOfType<CurrentTime> ();
	}

	//function to remove applicant from list
	public void RemoveApplication(HumanLife person) {
		//loop until applicant is found then remove it
		for (int a = 0; a < applications.Count; a++) {
			if (applications [a] == person) {
				applications.RemoveAt (a);
				return;
			}
		}
	}

	//virtual function for workers to work
	public virtual bool Work(HumanLife worker) {
		return true;
	}

	//function to add worker to list
	public virtual void AddWorker(HumanLife worker) {
		//setup worker data
		workers.Add (ScriptableObject.CreateInstance <JobData>());
		worker.stats.jobData = workers [workers.Count - 1];
		workers [workers.Count - 1].worker = worker;
	}

	//function to remove worker from list
	public void RemoveWorker(HumanLife worker, bool addToRequiredWorkers) {
		//loop until worker is found then remove it
		for (int a = 0; a < workers.Count; a++) {
			if (workers [a].worker == worker) {
				workers [a].worker.stats.job = null;
				workers [a].worker.stats.jobData = null;
				workers.RemoveAt (a);
				break;
			}
		}
		//add to required workers if needed
		if (addToRequiredWorkers) {
			requiredWorkers++;
			jobSearchTime = jobSearchResetTime;
		}
	}

}