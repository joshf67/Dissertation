using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Education : Job {

	public override bool work (HumanLife worker)
	{
		worker.stats.intellegenceGrowth += Time.deltaTime * time.timeMult;
		if (worker.stats.intellegenceGrowth > 100) {
			worker.stats.intellegenceGrowth = 0;
			worker.stats.intelligence++;
		}

		worker.stats.strengthGrowth += Time.deltaTime * time.timeMult;
		if (worker.stats.strengthGrowth > 100) {
			worker.stats.strengthGrowth = 0;
			worker.stats.strength++;
		}

		worker.stats.dexterityGrowth += Time.deltaTime * time.timeMult;
		if (worker.stats.dexterityGrowth > 100) {
			worker.stats.dexterityGrowth = 0;
			worker.stats.dexterity++;
		}

		return true;
	}

}
