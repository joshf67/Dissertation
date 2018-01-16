using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {
	
	public List<HumanLife> occupants;
	public List<room> rooms;

	public float cost;
	public float initialCost;
	public int costDays;

	public Vector3 position;
	public GameObject obj;
	public Housing ownerScript;
	public int ownerScriptIndex;
}