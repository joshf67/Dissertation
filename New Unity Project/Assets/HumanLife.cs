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

	public float minDist;

	public Vector2 workHours;
	public Vector2 sleepHours;

	public float cash;
	public float income;

	public float maxFood;
	public float maxEnergy;
	public float maxHappiness;

	public float food;
	public float energy;
	public float happiness;

	public float foodDegregation;
	public float energyDegregation;
	public float foodCost;

	public int state = 0;

	void Start() {
		agent = GetComponent<NavMeshAgent> ();
		agent.stoppingDistance = minDist;
	}

	// Update is called once per frame
	void Update () {
		switch (state) {
		case 0:
			Work ();
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
		}
		checkTime ();
	}

	void checkTime() {

		food -= foodDegregation * time.timeMult * Time.deltaTime;

		//sleep
		if ((time.CurrentHMS.x >= sleepHours.x ||
			time.CurrentHMS.x >= 0) &&
			time.CurrentHMS.x < sleepHours.y) {
			state = 3;
			return;
		}

		energy -= energyDegregation * time.timeMult * Time.deltaTime;

		//work
		if (time.CurrentHMS.x >= workHours.x && 
			time.CurrentHMS.x < workHours.y) {
			if (food >= maxFood / 10) {
				state = 0;
				return;
			} else {
				//grab food
				state = 1;
			}
		}

		//shop
		if (time.CurrentHMS.x >= workHours.y && 
			time.CurrentHMS.x < sleepHours.x) {
			if (food <= maxFood / 10) {
				state = 1;
				return;
			}
		}

		//entertainment
		if (time.CurrentHMS.x >= workHours.y && 
			time.CurrentHMS.x < sleepHours.x) {
			if (food > maxFood / 10) {
				state = 2;
				return;
			}
		}


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

	void Work() {
		if (Vector3.Distance (agent.destination, work.transform.position) > minDist) {
			agent.SetDestination (work.transform.position);
		} else {
			if (Vector3.Distance (transform.position, work.transform.position) < minDist) {
				//energy -= energyDegregation * time.timeMult;
				cash += Time.deltaTime * income * time.timeMult;
			}
		}
	}

	void Shop() {
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
}
