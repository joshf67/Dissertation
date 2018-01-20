using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour {

	public string name;
	public float cost;
	public itemTypes type;

}

public enum itemTypes {
	food, houses
};