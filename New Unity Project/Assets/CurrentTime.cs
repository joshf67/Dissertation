using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTime : MonoBehaviour {

	public Vector3 DWM;
	public Vector4 CurrentDWMY;
	public Vector3 HMS;
	public float timeMult;
	public Vector2 CurrentHMS;
	public float currentTime;
	public Text text;
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime * timeMult;
		//seconds
		while (currentTime >= HMS.z) {
			CurrentHMS.y += 1;
			currentTime -= HMS.z;
		}

		//minutes
		while (CurrentHMS.y >= HMS.y) {
			CurrentHMS.x += 1;
			CurrentHMS.y -= HMS.y;
		}

		//hours
		while (CurrentHMS.x >= HMS.x) {
			CurrentHMS.x -= HMS.x;
			CurrentHMS.y = 0;
			CurrentDWMY.x += 1;
		}

		//weeks
		if (CurrentDWMY.x >= DWM.z) {
			CurrentDWMY.y += 1;
			CurrentDWMY.x = 0;
		}

		//months
		if (CurrentDWMY.y >= DWM.y) {
			CurrentDWMY.z += 1;
			CurrentDWMY.y = 0;
		}

		//years
		if (CurrentDWMY.z >= DWM.x) {
			CurrentDWMY.w += 1;
			CurrentDWMY.z = 0;
		}
			
		string output = null;

		output += "Day:" + CurrentDWMY.x;
		output += "\n";
		output += "Week:" + CurrentDWMY.y;
		output += "\n";
		output += "Month:" + CurrentDWMY.z;
		output += "\n";
		output += "Year:" + CurrentDWMY.w;
		output += "\n";

		//offset text
		if (CurrentHMS.x < 10) {
			output += "0";
		}

		output += CurrentHMS.x + ":";

		//offset text
		if (CurrentHMS.y < 10) {
			output += "0";
		}



		output += CurrentHMS.y + ":";

		if (currentTime < 10) {
			output += "0";
		}

		output += Mathf.FloorToInt(currentTime);

		text.text = output;
		

	}
}
