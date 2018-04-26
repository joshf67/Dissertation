using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Functions : MonoBehaviour {

	//function to check distance to an object
	public static bool checkDistance(NavMeshAgent agent, Vector3 destination, float distance) {
		//check if distance to destination is larger than distance
		if (Vector3.Distance (agent.transform.position, destination) > distance) {
			//enable the navmesh agent 
			agent.enabled = true;
			//sets the navmesh agent's destination
			agent.SetDestination (destination);
			return false;
		}
		//if actor is within range disable navmesh agent
		agent.enabled = false;
		return true;
	}

	//function to check distance to a business
	public static bool checkDistanceBusiness(HumanLife actor, NavMeshAgent agent, Business business, Vector3 destination, float distance) {
		//check if distance to destination is larger than distance
		if (Vector3.Distance (agent.transform.position, destination) > distance) {
			//check if the business is not online or the actor doesn't have internet
			if (!(business.online && actor.accessToInternet)) {
				//enable the navmesh agent 
				agent.enabled = true;
				//sets the navmesh agent's destination
				agent.SetDestination (destination);
				return false;
			}
		}
		//if actor is within range disable navmesh agent
		agent.enabled = false;
		return true;
	}

}
