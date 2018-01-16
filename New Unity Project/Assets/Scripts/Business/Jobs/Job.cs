using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job : MonoBehaviour {

	public int paymentDays;
	public bool paymentMade;

	public float pay;
	public jobDetails hours;

	public bool open;
	public bool automated;
	public bool delivery;

	public int requiredWorkers;
	public float jobSearchTime;

	public jobStats requiredStats;
	public List<HumanLife> applications;
	public List<JobData> workers;

	protected CurrentTime time;

	void Start() {
		time = GameObject.FindObjectOfType<CurrentTime> ();
	}

	public virtual bool work(HumanLife worker) {
		return true;
	}

	public virtual void addWorker(HumanLife worker) {
		workers.Add (new JobData(worker));
	}

	public void removeWorker(HumanLife worker) {
		for (int a = 0; a < workers.Count; a++) {
			if (workers [a].worker == worker) {
				workers.RemoveAt (a);
				return;
			}
		}
	}

}