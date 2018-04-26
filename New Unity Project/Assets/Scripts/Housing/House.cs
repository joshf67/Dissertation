using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that stores house data
public class House : MonoBehaviour {

	//store how many rooms and occupants
	public List<HumanLife> occupants;
	public List<room> rooms;

	//store basic variables for rent
	public float cost;
	public float initialCost;
	public Vector3 costDays;
	public bool paymentMade;

	//store the owner, index and location of house
	public Vector3 position;
	public GameObject obj;
	public Housing ownerScript;
	public int ownerScriptIndex;
}