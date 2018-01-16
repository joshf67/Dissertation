using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanLife : MonoBehaviour {

	public CurrentTime time;
	public NavMeshAgent agent;

	public GameObject shop;
	public GameObject entertainment;

	public List<Job> jobApplications;
	public List<product> onSale;
	public List<Business> ignoreForCurrent;
	public int lastItemSearchedFor;
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

	public bool waitingForDelivery;

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
			Entertainment ();
			break;
		case 3:
			Sleep ();
			break;
		case 4:
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
					state = 0;
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

		//Shop needs a function

		//entertainment needs a function

		//go home
		if (food >= maxFood / 10) {
			state = 4;
			return;
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
		if (stats.accomodation.home) {
			if (Functions.checkDistance (agent, stats.accomodation.home.transform.position, minDist)) {
				if (Vector3.Distance (transform.position, stats.accomodation.home.transform.position) < minDist) {
					if (energy < maxEnergy) {
						energy += Time.deltaTime * time.timeMult;
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
			if (Vector3.Distance (agent.destination, stats.accomodation.home.transform.position) > minDist) {
				agent.SetDestination (stats.accomodation.home.transform.position);
			} else {
				if (Vector3.Distance (transform.position, stats.accomodation.home.transform.position) < minDist) {
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
			}
		} else {
			if (!FindHome ()) {
				Debug.Log ("IM HOMELESS");
			}
		}
	}

	void sellHome() {
		stats.accomodation.homeData.ownerScript.houses [stats.accomodation.homeData.ownerScriptIndex].occupants.Clear ();
	}

	public void resetHome() {
		stats.accomodation.home = null;
		stats.accomodation.homeData = null;
	}

	bool FindHome() {
		List<House> availableHomes = new List<House>();
		float availableCash = 0;

		foreach (HumanLife person in stats.accomodation.occupants) {
			availableCash += person.incomePerMonth;
		}
		availableCash /= 2;

		foreach (Housing houses in GameObject.FindObjectsOfType<Housing>()) {
			foreach (House potentialHome in houses.findHouse(availableCash, stats.accomodation.requiredRooms)) {
				availableHomes.Add (potentialHome);
			}
		}

		bool more = false;
		do {
			more = false;
			for (int a = 0; a < availableHomes.Count - 1; a++) {
				float distance = 0;
				float distanceToNext = 0;
				foreach (HumanLife person in stats.accomodation.occupants) {
					distance += Vector3.Distance (person.stats.job.transform.position, availableHomes [a].position);
					distanceToNext += Vector3.Distance (person.stats.job.transform.position, availableHomes [a + 1].position);
				}
				if (availableHomes [a].cost + distance > availableHomes [a + 1].cost + distanceToNext) {
					House tempHouse = availableHomes [a];
					availableHomes [a] = availableHomes [a + 1];
					availableHomes [a + 1] = tempHouse;
					more = true;
				}
			}
		} while (more);

		if (availableHomes.Count != 0) {
			foreach (HumanLife person in stats.accomodation.occupants) {
				person.stats.accomodation.homeData = availableHomes [0].ownerScript.houses[availableHomes[0].ownerScriptIndex];
				person.stats.accomodation.home = person.stats.accomodation.homeData.obj;
			}
			stats.accomodation.homeData.occupants = stats.accomodation.occupants;
			return true;
		}
		return false;
	}

	void Work() {
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

	Business findClosestStore(int itemTypeRequired, string itemNameRequired = "", List<Business> ignore = null) {
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
				List<product> tempProducts = bus.searchForItem (itemTypeRequired, itemNameRequired);
				if (tempProducts.Count != 0) {
					if (Vector3.Distance (transform.position, bus.transform.position) < distance) {
						distance = Vector3.Distance (transform.position, bus.transform.position);
						closest = bus;
						onSale.Clear ();
						foreach (product prod in tempProducts) {
							onSale.Add (prod);
						}
					}
				}
			}
		}
		return closest;
	}

	void arrangeBasedOnAffect(List<product> products) {
		bool more = false;
		do {
			more = false;
			for (int a = 0; a < products.Count - 1; a++) {
				if (products [a].data.effect < products [a + 1].data.effect) {
					product tempProduct = products [a];
					products [a] = products [a + 1];
					products [a + 1] = tempProduct;
					more = true;
				}
			}
		} while (more);
	}

	//try to buy item from business
	bool buyItem(Business shopBussiness, product prod, bool delivery) {
		//TEMP DELIVERY TEST REMOVE LATER
		if (delivery) {
			//check if person can afford item
			if (shopBussiness.charge (prod, this)) {
				waitingForDelivery = true;
				return true;
			}
		} else {
			//check if person can afford item
			if (shopBussiness.charge (prod, this)) {
				//add item to inventory (temp)
				inventory.Add (prod.data);
				return true;
			}
		}
		return false;
	}

	void eatFood() {
		for (int a = 0; a < inventory.Count; a++) {
			if (string.Equals (inventory[a].name, "food", System.StringComparison.OrdinalIgnoreCase)) {
				food += inventory [a].effect;
				inventory.RemoveAt (a);
				return;
			}
		}
		if (!waitingForDelivery) {
			Shop (0, "food", maxFood, food);
		}
	}

	void testShop (Business shopBusiness, float maxValue = -1, float currentValue = -1, bool delivery = false) {
		//arrange items based on effects (highest first)
		arrangeBasedOnAffect (onSale);
		//loop through all possible products
		for (int a = 0; a < onSale.Count; a++) {

			//check if person can afford item
			if (cash >= onSale [a].cost) {

				//test if the item refills under the food required
				if ((maxValue - currentValue > onSale [a].data.effect) || maxValue == -1) {
					//check if the person can buy the first item (best effect)
					if (a == 0) {
						if (buyItem (shopBusiness, onSale [0], delivery)) {
							return;
						}
					} else {
						//check if previous item is better with relativly same cost
						if (onSale [a - 1].cost < onSale [a].cost * 1.5f) {
							if (buyItem (shopBusiness, onSale [a], delivery)) {
								return;
							}
						} else {
							//if previous item is not better then buy current
							if (buyItem (shopBusiness, onSale [a], delivery)) {
								return;
							}
						}
					}
				}
			}
		}
		//add business to ignore list due to not having item
		ignoreForCurrent.Add (shopBusiness);
		shop = null;
	}

	void Shop(int itemType, string itemName = "", float maxValue = -1, float currentValue = -1) {
		//check if search for new item, if so remove ignore list
		if (lastItemSearchedFor != itemType || lastItemNameSearchFor != itemName) {
			ignoreForCurrent.Clear ();
		}
		lastItemSearchedFor = itemType;
		lastItemNameSearchFor = itemName;

		if (shop) {
			//check if shop is open
			Business shopBusiness = shop.GetComponent<Business> ();
			if (shopBusiness.open) {
				//TEMP DELIVERY TEST CHANGE LATER
				if (shopBusiness.online) {
					testShop (shopBusiness, maxValue, currentValue, true);
				} else {
					//check if person is within range of business
					if (Functions.checkDistanceBusiness (this, agent, shopBusiness, shop.transform.position, minDist)) {
						testShop (shopBusiness, maxValue, currentValue, false);
					}
				}
			}
		} else {
			Business tempShop = findClosestStore (itemType, itemName, ignoreForCurrent);
			if (tempShop != null) {
				shop = tempShop.gameObject;
			}
		}
	}

	/*
	void Shop() {
		if (shop && shop.GetComponent<Business> ().open) {
			if (Vector3.Distance (agent.destination, shop.transform.position) > minDist) {
				agent.SetDestination (shop.transform.position);
			} else {
				if (Vector3.Distance (transform.position, shop.transform.position) < minDist) {
					if (food < maxFood) {
						if (onSale.Count == 0) {
							if (shop.GetComponent<Business> ().charge (onSale [0], this)) {
								food += onSale [0].data.effect;
							}
						} else {
							arrangeBasedOnAffect (onSale);
							for (int a = 0; a < onSale.Count; a++) {
								if (cash >= onSale [a].cost) {
									if (maxFood - food > onSale [a].data.effect) {
										if (a == 0) {
											if (shop.GetComponent<Business> ().charge (onSale [a], this)) {
												food += onSale [a].data.effect;
												return;
											}
										} else {
											if (onSale [a - 1].cost < onSale [a].cost * 1.5f) {
												if (shop.GetComponent<Business> ().charge (onSale [a - 1], this)) {
													food += onSale [a].data.effect;
													return;
												}
											} else {
												if (shop.GetComponent<Business> ().charge (onSale [a], this)) {
													food += onSale [a].data.effect;
													return;
												}
											}
										}
									}
								} else {
									if (a == onSale.Count) {
										shop = findClosestStore (0,"",shop.GetComponent<Business> ()).gameObject;
									}
								}
							}
						}
					}
				}
			}
		} else {
			shop = findClosestStore (0).gameObject;
		}
	}
	*/

	void Entertainment() {
		if (Functions.checkDistance (agent, entertainment.transform.position,minDist)) {
			if (Vector3.Distance (transform.position, entertainment.transform.position) < minDist) {
				happiness += Time.deltaTime * time.timeMult;
			}
		}
	}

	/*
	bool checkDistance(Vector3 destination, float distance) {
		if (Vector3.Distance (transform.position, destination) > distance) {
			agent.SetDestination (destination);
			return false;
		}
		return true;
	}

	bool checkDistanceBusiness(Business business, Vector3 destination, float distance) {
		if (Vector3.Distance (transform.position, destination) > distance) {
			if (!(business.online && accessToInternet)) {
				agent.SetDestination (destination);
				return false;
			}
		}
		return true;
	}
	*/

	public void recalculateSleep() {
		stats.sleepHours.y = stats.job.hours.workHours.x - stats.travelHours;
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