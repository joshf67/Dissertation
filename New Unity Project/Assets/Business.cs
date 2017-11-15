using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Business : MonoBehaviour {

	public bool findWorkers;
	public float timeForWorkers;
	public int requiredWorkers;
	public int currentWorkers;
	public List<HumanLife> workers;
	public List<HumanLife> applications;
    public Stats requiredStats;
	public float paymentTimeDays;
	public bool paymentMade = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindObjectOfType<CurrentTime> ().CurrentDWMY.x == paymentTimeDays && paymentMade == false) {
			paymentMade = true;
			foreach (HumanLife human in workers) {
				human.Pay ();
			}
		} else {
			paymentMade = false;
		}
		if (timeForWorkers <= 0) {
			if (applications.Count != 0) {
				testApplications ();
			}
		} else {
			timeForWorkers -= Time.deltaTime * GameObject.FindObjectOfType<CurrentTime> ().timeMult;
		}
	}

	public bool testStats(Stats stat) {
		if (stat.intellegence >= requiredStats.intellegence) {
			if (stat.dexterity >= requiredStats.dexterity) {
				if (stat.strength >= requiredStats.strength) {
					return true;
				}
			}
		}
		return false;
	}

	public bool work() {
		return true;
	}

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
			workers.Add (applications [accepted]);
			workers [workers.Count-1].stats.income = requiredStats.income;
			workers [workers.Count-1].stats.workHours = requiredStats.workHours;
			workers [workers.Count-1].stats.hasJob = true;
			workers [workers.Count - 1].stats.job = this;
			workers [workers.Count-1].recalculateSleep();
			accepted++;
		}
		for (int a = 0; a < applications.Count; a++) {
			applications.RemoveAt(0);
		}
		requiredWorkers -= accepted;
	}
}