using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services : Job {

	public float arbitaryIncomePerWorker = 1;

	public override bool work (HumanLife worker)
	{
		GetComponent<Business>().cash += 1 * Time.deltaTime;
		return true;
	}

}
