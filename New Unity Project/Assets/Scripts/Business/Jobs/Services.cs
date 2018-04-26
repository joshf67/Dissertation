using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services : Job {

	//variable that changes how much the company makes 
	//for a worker working
	public float arbitaryIncomePerWorker = 1;

	//function that handles what happens when workers work
	public override bool Work (HumanLife worker)
	{
		//add value above to business cash
		GetComponent<Business>().cash += 1 * Time.deltaTime;
		return true;
	}

}
