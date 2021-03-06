﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : Job {

	//function that handles what happens when workers work
	public override bool Work (HumanLife student)
	{
		//increase each statGrowth by delta time and levelup if above 100
		student.stats.intellegenceGrowth += Time.deltaTime * time.timeMult;
		if (student.stats.intellegenceGrowth > 100) {
			student.stats.intellegenceGrowth = 0;
			student.stats.intelligence++;
		}

		student.stats.strengthGrowth += Time.deltaTime * time.timeMult;
		if (student.stats.strengthGrowth > 100) {
			student.stats.strengthGrowth = 0;
			student.stats.strength++;
		}

		student.stats.dexterityGrowth += Time.deltaTime * time.timeMult;
		if (student.stats.dexterityGrowth > 100) {
			student.stats.dexterityGrowth = 0;
			student.stats.dexterity++;
		}

		return true;
	}

}