using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that stores product data
public class Product : MonoBehaviour {

	//store basic product variables
	public string name;
	public float cost;
	public itemTypes type;
	public int index;

}

//enum setup for all product types
public enum itemTypes {
	none, food, hunger, energy, happiness, houses, utilities
};