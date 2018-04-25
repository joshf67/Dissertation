using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job : MonoBehaviour {

	public int paymentDays;
	public bool paymentMade;

	public string jobType;
	public float pay;
	public jobDetails hours;

	public bool open;
	public bool automated;
	public bool delivery;

	public int requiredWorkers;
	public float jobSearchResetTime;
	public float jobSearchTime;

	public jobStats requiredStats;
	public List<HumanLife> applications;
	public List<JobData> workers;

	protected CurrentTime time;

	void Start() {
		time = GameObject.FindObjectOfType<CurrentTime> ();
	}

	public void removeApplication(HumanLife person) {
		for (int a = 0; a < applications.Count; a++) {
			if (applications [a] == person) {
				applications.RemoveAt (a);
				return;
			}
		}
	}

	public virtual bool work(HumanLife worker) {
		return true;
	}

	public virtual void addWorker(HumanLife worker) {
		workers.Add (ScriptableObject.CreateInstance <JobData>());
		worker.stats.jobData = workers [workers.Count - 1];
		workers [workers.Count - 1].worker = worker;
	}

	public void removeWorker(HumanLife worker, bool addToRequiredWorkers) {
		for (int a = 0; a < workers.Count; a++) {
			if (workers [a].worker == worker) {
				workers [a].worker.stats.job = null;
				workers [a].worker.stats.jobData = null;
				workers.RemoveAt (a);
				break;
			}
		}
		if (addToRequiredWorkers) {
			requiredWorkers++;
			jobSearchTime = jobSearchResetTime;
		}
	}

}