using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanLife : MonoBehaviour {

	public CurrentTime time;
	public NavMeshAgent agent;

	public GameObject entertainment;

	public List<Job> jobApplications;
	public List<Product> onSale;
	public List<Business> ignoreForCurrent;
	//public Business lastIgnore;
	public Business currentBusiness;
	public itemTypes lastItemSearchedFor;
	public string lastItemNameSearchFor;

	public List<item> inventory;

	public float minDist;

	public float cash;
	public float incomePerMonth;

	public bool hasInternet;
	public bool accessToInternet;

	public float maxFood;
	public float maxEnergy;
	public float maxHappiness;

	public float food;
	public float energy;
	public float happiness;

	public float foodDegregation;
    public float foodSleepDegregation;
    public float energyDegregation;

    public Stats stats;
	public float timeWorking;

	public int waitingForDelivery;

	public int tick = 0;
	public int lastShopTick = 0;

	public int state = 0;

	void Start() {
		agent = GetComponent<NavMeshAgent> ();
		agent.stoppingDistance = minDist;
		time = GameObject.FindObjectOfType<CurrentTime> ();
		entertainment = GameObject.FindGameObjectWithTag ("Entertainment");
		stats.ageDays = stats.age * 365;
	}

	// Update is called once per frame
	void Update () {
		if (stats.job) {
			if (stats.job.hours.workHours.x > stats.job.hours.workHours.y) {
				incomePerMonth = ((24 - stats.job.hours.workHours.x) + stats.job.hours.workHours.y) * stats.income * 28;
			} else {
				incomePerMonth = (stats.job.hours.workHours.y - stats.job.hours.workHours.x) * stats.income * 28;
			}
		}
        if (food <= -1000)
        {
            Destroy(gameObject);
        }

		switch (state) {
		case 0:
			if (stats.job) {
				Work ();
			} else {
				FindJob ();
			}
			break;
		case 1:
			eatFood ();
			break;
		case 2:
			gatherEnergy ();
			break;
		case 3:
			Sleep ();
			break;
		case 4:
			GoHome ();
			break;
		case 5:
			Entertainment ();
			break;
		}

		checkTime ();
		tick++;

        if (state == 3)
        {
            food -= foodSleepDegregation * time.timeMult * Time.deltaTime;
        } else
        {
            energy -= energyDegregation * time.timeMult * Time.deltaTime;
            food -= foodDegregation * time.timeMult * Time.deltaTime;
        }
	}

	bool testTime(float start, float end) {
		return (end < start && (time.CurrentHMS.x < end || time.CurrentHMS.x >= start)) || //test if time is after end or before start
				(start < end && time.CurrentHMS.x >= start && time.CurrentHMS.x < end); //test if time is within start and end
	}

	void checkTime() {
        
		//sleep
		if (testTime(stats.sleepHours.x,stats.sleepHours.y))
		{
			state = 3;
			return;
		}

		//work
		if (stats.job) {

			if (testTime (stats.job.hours.workHours.x - stats.travelHours, stats.job.hours.workHours.y)) {

				if (food >= maxFood / 10) {
					if (energy >= maxEnergy / 10) {
						state = 0;
					} else {
						if (testTime (stats.job.hours.breakTime.x, stats.job.hours.breakTime.y)) {
							//gather energy
							state = 2;
						} else {
							state = 0;
						}
					}
				} else {
					if (testTime (stats.job.hours.breakTime.x, stats.job.hours.breakTime.y)) {
						//grab food
						state = 1;
					} else {
						//work because no break
						state = 0;
					}
				}

				return;

			}
		} else {

			if (!stats.hasTask) {
				if (food >= maxFood / 10) {
					state = 0;
				} else {
					//grab food
					state = 1;
				}
				return;
			}

		}

		//entertainment needs a function

		//go home
		if (food >= maxFood / 2) {
			if (energy >= maxEnergy / 2) {
				state = 4;
				return;
			} else {
				state = 2;
				return;
			}
		} else {
			//grab food
			state = 1;
			return;
		}


    }

    void FindJob() {
		Business[] businesses = GameObject.FindObjectsOfType<Business> ();
		for (int a = 0; a < businesses.Length; a++) {
			Business bus = businesses [a];
			foreach (Job job in bus.occupations) {
				if (job.requiredWorkers > 0) {
					if (bus.testStats (stats, job)) {
						if (!job.applications.Contains (this)) {
							job.applications.Add (this);
							jobApplications.Add (job);
						}
					}
				}
			}
		}
    }

	public void resetApplications() {
		foreach (Job job in jobApplications) {
			job.removeApplication (this);
		}
		jobApplications.Clear ();
	}

    void Sleep() {
		if (stats.accomodation.home) {
			if (food <= maxFood / 10) {
				eatFood ();
				return;
			} else {
				if (Functions.checkDistance (agent, stats.accomodation.home.transform.position, minDist)) {
					if (Vector3.Distance (transform.position, stats.accomodation.home.transform.position) < minDist) {
						if (energy < maxEnergy) {
							energy += Time.deltaTime * time.timeMult;
						}
					}
				}
			}
		} else {
			if (!FindHome ()) {
				Debug.Log ("IM HOMELESS");
			}
		}
	}

	void GoHome() {
		if (stats.accomodation.home) {
			if (Functions.checkDistance(agent, stats.accomodation.home.transform.position, minDist)) {
					if (energy < maxEnergy) {
						energy += (Time.deltaTime * time.timeMult) / 2;
					}
				if (stats.wantKids) {
					foreach (relation rel in stats.relationship) {
						if (rel.relationType == 0) {
							if (rel.other.stats.wantKids) {
								if (rel.other.state == 5) {
									rel.other.stats.wantKids = false;
									stats.wantKids = false;
									GameObject.FindObjectOfType<Reproduction> ().reproduce (this, rel.other);
									foreach (HumanLife person in stats.accomodation.occupants) {
										person.stats.accomodation.requiredRooms++;
									}
									if (stats.accomodation.homeData.rooms.Count < stats.accomodation.requiredRooms) {
										sellHome ();
										foreach (HumanLife person in stats.accomodation.occupants) {
											person.resetHome ();
										}
									}
									break;
								}
							}
						}
					}
				}
			}
		} else {
			if (!FindHome ()) {
				Debug.Log ("IM HOMELESS");
			}
		}
	}

	void sellHome() {
		stats.accomodation.homeData.ownerScript.houses [stats.accomodation.homeData.ownerScriptIndex].data.occupants.Clear ();
	}

	public void resetHome() {
		stats.accomodation.home = null;
		stats.accomodation.homeData = null;
	}

	bool FindHome() {
		Shop(itemTypes.houses);
		return (stats.accomodation.homeData != null);
	}

	void Work() {
		if (stats.jobData) {
			if (stats.jobData.ignoreDistance) {
				if (stats.job.work (this)) {
					timeWorking += time.timeMult * Time.deltaTime;
					return;
				}
			}
		}
		if (Functions.checkDistance (agent, stats.job.transform.position, minDist)) {
			//energy -= energyDegregation * time.timeMult;
			//cash += Time.deltaTime * income * time.timeMult;
			if (stats.job.work (this)) {
				timeWorking += time.timeMult * Time.deltaTime;
			}
		}
	}

	public void Pay(Business business) {
		cash += stats.income * timeWorking;
		business.cash -= stats.income * timeWorking;
		timeWorking = 0;
	}

	Business findClosestStore(itemTypes itemTypeRequired, string itemNameRequired = "", List<Business> ignore = null) {
		Business[] all = GameObject.FindObjectsOfType<Business> ();
		Business closest = null;
		float distance = Mathf.Infinity;
		foreach (Business bus in all) {
			if (ignore != null && ignore.Count != 0) {
				if (ignore.Contains (bus)) {
					continue;
				}
			}
			if (bus.open) {
				List<Product> tempProducts = bus.searchProducts (new shopTest(itemTypeRequired, itemNameRequired, this));
				if (tempProducts != null) {
					if (tempProducts.Count != 0) {
						if (Vector3.Distance (transform.position, bus.transform.position) < distance) {
							distance = Vector3.Distance (transform.position, bus.transform.position);
							closest = bus;
							onSale.Clear ();
							foreach (Product prod in tempProducts) {
								onSale.Add (prod);
							}
						}
					}
				}
			}
		}
		return closest;
	}

	int lookForItem(itemTypes item) {
		for (int a = 0; a < inventory.Count; a++) {
			if (inventory [a].itemType == item) {
				return a;
			}
		}
		return -1;
	}

	void eatFood() {
		int index = lookForItem (itemTypes.food);
		if (index != -1) {
			food += inventory [index].effect;
			inventory.RemoveAt (index);
			return;
		} else {
			if (waitingForDelivery == 0) {
				Shop (itemTypes.food, "", true, true);
			}
		}
	}

	void gatherEnergy() {
		int index = lookForItem (itemTypes.energy);
		if (index != -1) {
			energy += inventory [index].effect;
			inventory.RemoveAt (index);
			return;
		} else {
			if (waitingForDelivery == 0) {
				Shop (itemTypes.energy, "", true, true);
			}
		}
	}

	bool testShop (Business shopBusiness, bool compareProductValue = false, bool delivery = false, bool online = false) {
		//loop through all possible products
		for (int a = 0; a < onSale.Count; a++) {

			//test if the product is still available
			if (shopBusiness.testProduct (onSale [a])) {

				//test if the item refills under the food required
				if (!compareProductValue || shopBusiness.compareProduct (onSale [a], this)) {
								
					//check if the person can buy the first item (best effect)
					if (a == 0) {
						if (shopBusiness.Buy (new purchaseOptions (onSale [a].index, delivery, online, this))) {
							return true;
						}

					} else {
								
						//check if previous item is better with relativly same cost
						if (onSale [a - 1].cost < onSale [a].cost * 1.5f && shopBusiness.testProduct (onSale [a - 1])) {
							if (shopBusiness.Buy (new purchaseOptions (onSale [a - 1].index, delivery, online, this))) {
								return true;
							}
						} 

						//if previous item is not better then buy current
						if (shopBusiness.Buy (new purchaseOptions (onSale [a].index, delivery, online, this))) {
							return true;
						}
					}
				}
			}
		}
		//add business to ignore list due to not having item
		ignoreForCurrent.Add (shopBusiness);
		return false;
	}
		

	void Shop(itemTypes itemType, string itemName = "", bool compareProductValue = false, bool attemptDelivery = false) {
		//check if search for new item, if so remove ignore list
		if (lastItemSearchedFor != itemType || lastItemNameSearchFor != itemName) {
			ignoreForCurrent.Clear ();
		}

		//check if last shop check was longer than 1 tick if so reset shop options
		if (tick - lastShopTick > 1) {
			ignoreForCurrent.Clear ();
			currentBusiness = null;
		}

		lastShopTick = tick;
		lastItemSearchedFor = itemType;
		lastItemNameSearchFor = itemName;

		if (currentBusiness) {
			//check if shop is open
			if (currentBusiness.open) {
				//TEMP DELIVERY TEST CHANGE LATER
				if (currentBusiness.online && attemptDelivery) {
					if (!testShop (currentBusiness, compareProductValue, true, true)) {
						if (currentBusiness.inStore) {
							if (Functions.checkDistanceBusiness (this, agent, currentBusiness, currentBusiness.transform.position, minDist)) {
								testShop (currentBusiness, compareProductValue, false, false);
								currentBusiness = null;
							}
						} else {
							currentBusiness = null;
						}
					} else {
						currentBusiness = null;
					}
				} else {
					//check if person is within range of business
					if (Functions.checkDistanceBusiness (this, agent, currentBusiness, currentBusiness.transform.position, minDist)) {
						testShop (currentBusiness, compareProductValue, false, false);
						currentBusiness = null;
					}
				}
			}
		} else {
			currentBusiness = findClosestStore (itemType, itemName, ignoreForCurrent);
		}
	}

	void Entertainment() {
		if (Functions.checkDistance (agent, entertainment.transform.position,minDist)) {
			if (Vector3.Distance (transform.position, entertainment.transform.position) < minDist) {
				happiness += Time.deltaTime * time.timeMult;
			}
		}
	}

	public void recalculateSleep() {
		stats.sleepHours.y = stats.job.hours.workHours.x - stats.travelHours;
		if (stats.sleepHours.y < 0) {
			stats.sleepHours.y = 24 + stats.sleepHours.y;
		}
		if (stats.sleepHours.y - 8 < 0) {
			stats.sleepHours.x = 24 - (8 - stats.sleepHours.y);
		} else {
			stats.sleepHours.x = stats.sleepHours.y - 8;
		}
	}

	public void updateAge() {
		stats.ageDays++;
		stats.age = (int)(stats.ageDays / 365);
	}
}