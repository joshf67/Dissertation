using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanLife : MonoBehaviour {

	public CurrentTime time;
	public NavMeshAgent agent;

	public GameObject shop;
	public GameObject work;
	public GameObject home;
	public GameObject entertainment;

	public List<Business> jobApplications;

	public float minDist;

	public float cash;
	public float income;

	public float maxFood;
	public float maxEnergy;
	public float maxHappiness;

	public float food;
	public float energy;
	public float happiness;

	public float foodDegregation;
    public float foodSleepDegregation;
    public float energyDegregation;
	public float foodCost;

    public Stats stats;
	public float timeWorking;

	public int state = 0;

	void Start() {
		agent = GetComponent<NavMeshAgent> ();
		agent.stoppingDistance = minDist;
		time = GameObject.FindObjectOfType<CurrentTime> ();
		//shop = GameObject.FindGameObjectWithTag ("Shop");
		//work = GameObject.FindGameObjectWithTag ("Work");
		entertainment = GameObject.FindGameObjectWithTag ("Entertainment");
		home = GameObject.FindGameObjectWithTag ("Home");
	}

	// Update is called once per frame
	void Update () {
        if (food <= -1000)
        {
            Destroy(gameObject);
        }

		switch (state) {
		case 0:
			if (stats.hasJob) {
				Work ();
			} else {
				FindJob ();
			}
			break;
		case 1:
			Shop ();
			break;
		case 2:
			Entertainment ();
			break;
		case 3:
			Sleep ();
			break;
        case 4:
            FindJob();
            break;
		case 5:
			GoHome ();
			break;
		}
		checkTime ();

        if (state == 3)
        {
            food -= foodSleepDegregation * time.timeMult * Time.deltaTime;
        } else
        {
            energy -= energyDegregation * time.timeMult * Time.deltaTime;
            food -= foodDegregation * time.timeMult * Time.deltaTime;
        }
	}

	void checkTime() {
        
		//sleep
		if ((time.CurrentHMS.x >= stats.sleepHours.x ||
			time.CurrentHMS.x >= 0) &&
			time.CurrentHMS.x < stats.sleepHours.y) {
			state = 3;
			return;
		}

		//work
		if (time.CurrentHMS.x >= (stats.workHours.x - stats.travelHours) &&
			time.CurrentHMS.x < stats.workHours.y && stats.hasJob) {
			if (food >= maxFood / 10) {
				state = 0;
				return;
			} else {
				//grab food
				state = 1;
				return;
			}
		} else if (!stats.hasJob) {
			if (food >= maxFood / 10) {
				state = 0;
				return;
			} else {
				//grab food
				state = 1;
				return;
			}
		}

		//shop
		if (time.CurrentHMS.x >= stats.workHours.y && 
			time.CurrentHMS.x < stats.sleepHours.x && stats.hasJob) {
			if (food <= maxFood / 10) {
				state = 1;
				return;
			}
		}

		//entertainment
		if (time.CurrentHMS.x >= stats.workHours.y && 
			time.CurrentHMS.x < stats.sleepHours.x && stats.hasJob &&
			happiness < maxHappiness/10) {
			if (food > maxFood / 10) {
				state = 2;
				return;
			}
		}

        //Find Job
		if (time.CurrentHMS.x >= stats.sleepHours.y &&
			time.CurrentHMS.x < stats.sleepHours.x && !stats.hasJob)
        {
            if (cash > foodCost)
            {
                state = 4;
                return;
            } else
            {
                state = 4;
                return;
            }
        }

		//go home
		state = 5;


    }

    void FindJob() {
		Business[] businesses = GameObject.FindObjectsOfType<Business> ();
		for (int a = 0; a < businesses.Length; a++) {
			Business bus = businesses [a];
			if (bus.findWorkers) {
				if (bus.testStats (stats)) {
					if (!bus.applications.Contains (this)) {
						bus.applications.Add (this);
					}
				}
			}
		}

		/*

		Business best;
		float currentBest = -Mathf.Infinity;

		//fast check of applications and find best
		foreach (Business bus in jobApplications) {
			
			if (bus.requiredStats.income > currentBest) {
				currentBest = bus.requiredStats.income;
				best = bus;
			}
		}

*/
			
    }

    void Sleep() {
		if (Vector3.Distance (agent.destination, home.transform.position) > minDist) {
			agent.SetDestination (home.transform.position);
		} else {
			if (Vector3.Distance (transform.position, home.transform.position) < minDist) {
				if (energy < maxEnergy) {
					energy += Time.deltaTime * time.timeMult;
				}
			}
		}
	}

	void GoHome() {
		if (Vector3.Distance (agent.destination, home.transform.position) > minDist) {
			agent.SetDestination (home.transform.position);
		} else {
			if (Vector3.Distance (transform.position, home.transform.position) < minDist) {
				if (energy < maxEnergy) {
					energy += (Time.deltaTime * time.timeMult)/2;
				}
				if (stats.wantKids) {
					if (stats.relationship.Count != 0) {
						if (stats.relationship [0].state == 5) {
							if (stats.relationship [0].stats.wantKids) {
								GameObject.FindObjectOfType<Reproduction> ().reproduce (this, stats.relationship [0]);
							}
						}
					}
				}
			}
		}
	}

	void Work() {
		if (Vector3.Distance (agent.destination, stats.job.transform.position) > minDist) {
			agent.SetDestination (stats.job.transform.position);
		} else {
			if (Vector3.Distance (transform.position, stats.job.transform.position) < minDist) {
				//energy -= energyDegregation * time.timeMult;
				//cash += Time.deltaTime * income * time.timeMult;
				if ((time.CurrentHMS.x >= stats.workHours.x || stats.workHours.y < stats.workHours.x) && (time.CurrentHMS.x < stats.workHours.y)) {
					timeWorking += time.timeMult * Time.deltaTime;
				}
			}
		}
	}

	public void Pay() {
		cash += stats.income * timeWorking;
		timeWorking = 0;
	}

	Business findClosestStore() {
		Business[] all = GameObject.FindObjectsOfType<Business> ();
		Business closest = null;
		float distance = Mathf.Infinity;
		foreach (Business bus in all) {
			if (bus.open) {
				if (Vector3.Distance(transform.position, bus.transform.position) < distance) {
					distance = Vector3.Distance (transform.position, bus.transform.position);
					closest = bus;
				}
			}
		}
		return closest;
	}

	void Shop() {
		if (shop && shop.GetComponent<Business> ().open) {
			if (Vector3.Distance (agent.destination, shop.transform.position) > minDist) {
				agent.SetDestination (shop.transform.position);
			} else {
				if (Vector3.Distance (transform.position, shop.transform.position) < minDist) {
					if (food < maxFood) {
						food = maxFood;
						cash -= foodCost;
					}
				}
			}
		} else {
			shop = findClosestStore ().gameObject;
		}
	}

	void Entertainment() {
		if (Vector3.Distance (agent.destination, entertainment.transform.position) > minDist) {
			agent.SetDestination (entertainment.transform.position);
		} else {
			if (Vector3.Distance (transform.position, entertainment.transform.position) < minDist) {
				happiness += Time.deltaTime * time.timeMult;
			}
		}
	}

	public void recalculateSleep() {
		stats.sleepHours.y = stats.workHours.x - stats.travelHours;
		if (stats.sleepHours.y - 8 < 0) {
			stats.sleepHours.x = 24 - (8 - stats.sleepHours.y);
		} else {
			stats.sleepHours.x = stats.sleepHours.y - 8;
		}
	}
}
	
[System.Serializable]
public struct Stats
{
	public List<HumanLife> relationship;
	public bool wantKids;
	public int age, intellegence, strength, dexterity;
	public bool hasJob;
	public bool inEducation;
	public string jobTitle;
	public Business job;
	public float income;
	public Vector2 wantedHours;
	public Vector2 workHours;
	public float travelHours;
	public Vector2 sleepHours;
}