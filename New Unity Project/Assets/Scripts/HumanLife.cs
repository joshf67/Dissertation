using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//class that controls how the actors
//behave within the world
public class HumanLife : MonoBehaviour {

	//store basic variables
	public CurrentTime time;
	public NavMeshAgent agent;
	public CalorieBurnTable calorieBurnLookup;

	public List<Job> jobApplications;

	//store data for shopping
	public List<Product> onSale;
	public List<Business> ignoreForCurrent;
	public Business currentBusiness;
	public itemTypes lastItemSearchedFor;
	public string lastItemNameSearchFor;

	//store items within inventory
	public List<Item> inventory;

	//store cash variables
	public float cash;
	public float incomePerMonth;

	public bool hasInternet;
	public bool accessToInternet;

	//store max variables
	public float maxFood;
	public float maxEnergy;
	public float maxHappiness;

	//store needs variables
	public float targetFoodLevel;
	public float minimumFoodLevel;
	public float food;
	public float energy;
	public float happiness;

    public float energyDegregation;

	//store actors stats
    public Stats stats;

	//store other misc variables
	public float minDist;
	public float timeWorking;
	public bool randomiseStats = false;
	public int waitingForDelivery;
	public int tick = 0;
	public int lastShopTick = 0;
	public int state = 0;

	void Start() {
		//setup variables
		agent = GetComponent<NavMeshAgent> ();
		agent.stoppingDistance = minDist;
		time = GameObject.FindObjectOfType<CurrentTime> ();
		calorieBurnLookup = GameObject.FindObjectOfType<CalorieBurnTable> ();
		stats.ageDays = stats.age * 365;

		//randomise the base stats of the actor
		if (randomiseStats) {
			stats.dexterity = Random.Range (0, 10);
			stats.intelligence = Random.Range (0, 10);
			stats.strength = Random.Range (0, 10);
		}
	}

	// Update is called once per frame
	void Update () {
		//check if actor has a job
		if (stats.job) {
			//calculate actors income per month
			if (stats.job.hours.workHours.x > stats.job.hours.workHours.y) {
				incomePerMonth = ((24 - stats.job.hours.workHours.x) + stats.job.hours.workHours.y) * stats.income * 28;
			} else {
				incomePerMonth = (stats.job.hours.workHours.y - stats.job.hours.workHours.x) * stats.income * 28;
			}
		}

		//check if food is less than
		//-1000 if so kill the actor
        if (food <= -1000)
        {
            Destroy(gameObject);
        }

		//finite state machine for actor behaviour
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

		//update state
		checkTime ();
		tick++;

		//if actor is sleeping burn less food
        if (state == 3)
        {
			food -= calorieBurnLookup.TaskCostPerSecond("Sleeping") * time.timeMult;
        } else
        {
			//otherwise burn normal energy
            energy -= energyDegregation * time.timeMult * Time.deltaTime;

			//check if navigation agent is enable (actor is moving)
			if (agent.enabled) {

				//if so burn walking calories
				food -= calorieBurnLookup.TaskCostPerSecond("Walking") * time.timeMult;

			} else {
				//otherwise check if actor has a home
				bool foodDegraded = false;
				if (stats.accomodation.home != null) {
					//check if actor is within range of home
					if (Functions.checkDistance (agent, stats.accomodation.home.transform.position, minDist)) {
						//burn food by Resting amount
						foodDegraded = true;
						food -= calorieBurnLookup.TaskCostPerSecond("Resting") * time.timeMult;
					}
				} 
				//check if actor hasn't burned any food
				if (!foodDegraded) {
					//if so check if actor has a job
					if (stats.jobData != null) {
						//if so burn food by job type
						foodDegraded = true;
						food -= calorieBurnLookup.TaskCostPerSecond (stats.job.jobType) * time.timeMult;
					}
				}
			}

        }
	}

	//function to test if current time is between two values
	bool testTime(float start, float end) {
		return (end < start && (time.CurrentHMS.x < end || time.CurrentHMS.x >= start)) || //test if time is after end or before start
				(start < end && time.CurrentHMS.x >= start && time.CurrentHMS.x < end); //test if time is within start and end
	}

	void checkTime() {
        
		//sleep
		//test if time is between sleep hours
		if (testTime(stats.sleepHours.x,stats.sleepHours.y))
		{
			state = 3;
			return;
		}

		//work
		if (stats.job) {

			//check if current time is within work hours
			if (testTime (stats.job.hours.workHours.x - stats.travelHours, stats.job.hours.workHours.y)) {

				//check if food is larger than targetFoodlevel
				if (food >= targetFoodLevel) {
					//if so check if energy is greater than 10% maxEnergy
					if (energy >= maxEnergy / 10) {
						//if so work
						state = 0;
					} else {
						//otherwise test if it is break time
						if (testTime (stats.job.hours.breakTime.x, stats.job.hours.breakTime.y)) {
							//if so gather energy
							state = 2;
						} else {
							//otherwise keep working
							state = 0;
						}
					}
				} else {
					//otherwise test if it is break time
					if (testTime (stats.job.hours.breakTime.x, stats.job.hours.breakTime.y) || food <= 0) {
						//if so gather food
						state = 1;
					} else {
						//otherwise keep working
						state = 0;
					}
				}

				return;

			}
		} else {

			//otherwise check if the actor has a task
			if (!stats.hasTask) {
				//if not check if food is larger than target
				if (food >= targetFoodLevel) {
					//if so work
					state = 0;
				} else {
					//otherwise gather food
					state = 1;
				}
				return;
			}

		}

		//go home
		//check if food is larger than target
		if (food >= targetFoodLevel) {
			//if so check if energy is larger than target
			if (energy >= maxEnergy / 2) {
				//if so go home
				state = 4;
				return;
			} else {
				//otherwise gather energy
				state = 2;
				return;
			}
		} else {
			//otherwise gather food
			state = 1;
			return;
		}


    }

    void FindJob() {
		//find all businesses
		Business[] businesses = GameObject.FindObjectsOfType<Business> ();
		for (int a = 0; a < businesses.Length; a++) {
			Business bus = businesses [a];
			//loop through all jobs at business
			foreach (Job job in bus.occupations) {
				//check if business requires workers
				if (job.requiredWorkers > 0) {
					//test actors stats against required stats
					if (bus.TestStats (stats, job)) {
						//check if application has already been sent
						if (!job.applications.Contains (this)) {
							//if not add to application
							job.applications.Add (this);
							jobApplications.Add (job);
						}
					}
				}
			}
		}
    }

	public void resetApplications() {
		//loop through every jon application applied for
		//and remove self from application
		foreach (Job job in jobApplications) {
			job.RemoveApplication (this);
		}
		jobApplications.Clear ();
	}

    void Sleep() {
		//check if actor has a home
		if (stats.accomodation.home) {
			//check if food is less than minimum
			if (food <= minimumFoodLevel) {
				//call function to eat food
				eatFood ();
				return;
			} else {
				//otherwise check if distance to home is less than minimum
				if (Functions.checkDistance (agent, stats.accomodation.home.transform.position, minDist)) {
					//if so add to energy
					if (energy < maxEnergy) {
						energy += Time.deltaTime * time.timeMult;
					}
				}
			}
		} else {
			//otherwise find a home
			if (!FindHome ()) {
			}
		}
	}

	void GoHome() {
		//check if actor has a home
		if (stats.accomodation.home) {
			//check if actor is within range of home
			if (Functions.checkDistance (agent, stats.accomodation.home.transform.position, minDist)) {
				//give actor access to the internet
				accessToInternet = true;
				//check if energy is less than max and add to it if it is
				if (energy < maxEnergy) {
					energy += Time.deltaTime * time.timeMult;
				}
				/*
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
				*/
			}
		} else {
			//otherwise find a home
			if (!FindHome ()) {
			}
		}
	}

	//function that resets home data
	void sellHome() {
		stats.accomodation.homeData.ownerScript.houses [stats.accomodation.homeData.ownerScriptIndex].data.occupants.Clear ();
	}

	//function that resets local home data
	public void resetHome() {
		stats.accomodation.home = null;
		stats.accomodation.homeData = null;
	}

	//function that searches for a home
	bool FindHome() {
		//shop for house
		Shop(itemTypes.houses);
		return (stats.accomodation.homeData != null);
	}

	//function that handles actor working
	void Work() {
		//check if actor has job data
		if (stats.jobData) {
			//check if job doesn't require the actor to be within range
			if (stats.jobData.ignoreDistance) {
				//make actor work at business
				if (stats.job.Work (this)) {
					timeWorking += time.timeMult * Time.deltaTime;
					return;
				}
			}
		}
		//travel to job location
		if (Functions.checkDistance (agent, stats.company.buildingPosition.transform.position, minDist)) {
			//make actor work at business
			if (stats.job.Work (this)) {
				timeWorking += time.timeMult * Time.deltaTime;
			}
		}
	}

	//function that makes business pay actor for work
	public void Pay(Business business) {
		cash += stats.income * timeWorking;
		business.cash -= stats.income * timeWorking;
		timeWorking = 0;
	}

	Business findClosestStore(itemTypes itemTypeRequired, string itemNameRequired = "", List<Business> ignore = null) {
		//get all businesses in world
		Business[] all = GameObject.FindObjectsOfType<Business> ();
		Business closest = null;
		float distance = Mathf.Infinity;

		//loop through every business
		foreach (Business bus in all) {
			//check if business is within ignore list
			if (ignore != null && ignore.Count != 0) {
				if (ignore.Contains (bus)) {
					//if so skip this business
					continue;
				}
			}

			//check if business is open
			if (bus.open) {
				//search business for product
				List<Product> tempProducts = bus.SearchProducts (new ShopTest(itemTypeRequired, itemNameRequired, this));

				//check if product list exists
				if (tempProducts != null) {
					//check if products exists
					if (tempProducts.Count != 0) {
						//check if distance is less than current distance above
						if (Vector3.Distance (transform.position, bus.buildingPosition.transform.position) < distance) {
							//if so set closest business to be this
							distance = Vector3.Distance (transform.position, bus.transform.position);
							closest = bus;

							//add all business products to list
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

	//function that searches inventory for item
	int lookForItem(itemTypes item) {
		for (int a = 0; a < inventory.Count; a++) {
			if (inventory [a].itemType == item) {
				return a;
			}
		}
		return -1;
	}

	//function that deals with increasing food
	void eatFood() {
		//check inventory for food item
		int index = lookForItem (itemTypes.food);

		//if food item exists eat item and gain effects
		if (index != -1) {
			food += inventory [index].effect;
			inventory.RemoveAt (index);
			return;
		} else {
			//otherwise if the actor isn't wait for a delivery
			if (waitingForDelivery == 0) {
				//shop for food item
				Shop (itemTypes.food, "", false, true);
			}
		}
	}

	//function that deals with increasing energy
	void gatherEnergy() {
		//check inventory for energy item
		int index = lookForItem (itemTypes.energy);

		//if energy item exists drink item and gain effects
		if (index != -1) {
			energy += inventory [index].effect;
			inventory.RemoveAt (index);
			return;
		} else {
			//otherwise if the actor isn't wait for a delivery
			if (waitingForDelivery == 0) {
				//shop for energy item
				Shop (itemTypes.energy, "", true, true);
			}
		}
	}

	bool testShop (Business shopBusiness, bool compareProductValue = false, bool delivery = false, bool online = false) {
		//loop through all possible products
		for (int a = 0; a < onSale.Count; a++) {

			//test if the product is still available
			if (shopBusiness.TestProduct (onSale [a])) {

				//test if the item refills under the food required
				if (!compareProductValue || shopBusiness.CompareProduct (onSale [a], this)) {
								
					//check if the person can buy the first item (best effect)
					if (a == 0) {
						if (shopBusiness.Buy (new PurchaseOptions (onSale [a].index, delivery, online, this))) {
							return true;
						}

					} else {
								
						//check if previous item is better with relativly same cost
						if (onSale [a - 1].cost < onSale [a].cost * 1.5f && shopBusiness.TestProduct (onSale [a - 1])) {
							if (shopBusiness.Buy (new PurchaseOptions (onSale [a - 1].index, delivery, online, this))) {
								return true;
							}
						} 

						//if previous item is not better then buy current
						if (shopBusiness.Buy (new PurchaseOptions (onSale [a].index, delivery, online, this))) {
							return true;
						}
					}
				}
			}
		}
		if (!ignoreForCurrent.Contains (shopBusiness)) {
			//add business to ignore list due to not having item
			ignoreForCurrent.Add (shopBusiness);
		}
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
				//test if the business allows delivery
				if (currentBusiness.online && attemptDelivery) {
					//if buying product from store fails
					if (!testShop (currentBusiness, compareProductValue, true, true)) {
						//check if it is possible to buy in store
						if (currentBusiness.inStore) {
							//travel to store
							if (Functions.checkDistanceBusiness (this, agent, currentBusiness, currentBusiness.buildingPosition.transform.position, minDist)) {
								//buy product
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
					if (Functions.checkDistanceBusiness (this, agent, currentBusiness, currentBusiness.buildingPosition.transform.position, minDist)) {
						//try to buy product from store
						testShop (currentBusiness, compareProductValue, false, false);
						currentBusiness = null;
					}
				}
			}
		} else {
			//find the next closest store
			currentBusiness = findClosestStore (itemType, itemName, ignoreForCurrent);
		}
	}

	//function that deals with increasing happyness
	void Entertainment() {
	}

	//function that recalculates sleep to get 8 hours before work
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

	//function that adds days to age and updates year age
	public void updateAge() {
		stats.ageDays++;
		stats.age = (int)(stats.ageDays / 365);
	}
}