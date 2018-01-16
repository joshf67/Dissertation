using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Functions : MonoBehaviour {

	public static bool checkDistance(NavMeshAgent agent, Vector3 destination, float distance) {
		if (Vector3.Distance (agent.transform.position, destination) > distance) {
			agent.SetDestination (destination);
			return false;
		}
		return true;
	}

	public static bool checkDistanceBusiness(HumanLife actor, NavMeshAgent agent, Business business, Vector3 destination, float distance) {
		if (Vector3.Distance (agent.transform.position, destination) > distance) {
			if (!(business.online && actor.accessToInternet)) {
				agent.SetDestination (destination);
				return false;
			}
		}
		return true;
	}

}
