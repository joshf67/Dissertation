using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Business : MonoBehaviour {

    public Stats requiredStats;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public struct Stats
{
    public int intellegence, strength, dexterity;
}