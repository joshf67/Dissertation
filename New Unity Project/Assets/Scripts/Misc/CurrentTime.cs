using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTime : MonoBehaviour {

	public Vector3 DWM = new Vector3(7,4,12);
	public Vector4 CurrentDWMY;
	public Vector3 HMS = new Vector3(60,60,24);
	public float timeMult = 1;
	public float speed = 1;
	public Vector2 CurrentHMS;
	public float currentTime;
	public Text text;
	public GUIStyle boxStyle;
	public GUIStyle titleStyle;
	public GUIStyle displayStyle;

	public int[] hourPurchases = new int[24];
	public int[] activeWorkHours = new int[24];
	
	// Update is called once per frame
	void Update () {
		Time.timeScale = speed;
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
			foreach (HumanLife person in GameObject.FindObjectsOfType<HumanLife>()) {
				person.updateAge ();
			}
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

		float globalCash = 0;
		foreach(Business bus in GameObject.FindObjectsOfType<Business>()) {
			globalCash += bus.cash;
		}

		foreach(HumanLife hum in GameObject.FindObjectsOfType<HumanLife>()) {
			globalCash += hum.cash;
		}

		output += "\nGlobal Cash : " + ((int)globalCash);

		text.text = output;

	}

	public void addPurchase() {
		hourPurchases[(int)CurrentHMS.x]++;
	}

	public void addHours(Vector2 hours) {
		if (hours.x < hours.y) {
			for (int a = (int)hours.x; a < hours.y; a++) {
				activeWorkHours [a]++;
			}
		} else {
			for (int a = (int)hours.x; a < 24; a++) {
				activeWorkHours [a]++;
			}
			for (int a = 0; a < hours.x; a++) {
				activeWorkHours [a]++;
			}
		}
	}

	public void OnDrawGizmos() {
		float highest = 0;
		foreach (int i in hourPurchases) {
			if (i > highest) {
				highest = i;
			}
		}
		if (highest == 0) {
			highest = 1;
		}
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, hourPurchases[0]/highest, 0));
		for (int a = 0; a < 23; a++) {
			Gizmos.DrawLine(transform.position + new Vector3(-a, hourPurchases[a]/highest, 0), transform.position + new Vector3(-a - 1, hourPurchases[a + 1]/highest, 0));
		}

		float highestHr = 0;
		foreach (int i in activeWorkHours) {
			if (i > highestHr) {
				highestHr = i;
			}
		}
		highestHr /= 8;
		if (highestHr == 0) {
			highestHr = 1;
		}
		Gizmos.DrawLine(transform.position - new Vector3(0, 10, 0), transform.position + new Vector3(0, activeWorkHours[0]/highestHr, 0) - new Vector3(0, 10, 0));
		for (int a = 0; a < 23; a++) {
			Gizmos.DrawLine(transform.position + new Vector3(-a, activeWorkHours[a]/highestHr, 0) - new Vector3(0, 10, 0), transform.position + new Vector3(-a - 1, activeWorkHours[a + 1]/highestHr, 0) - new Vector3(0, 10, 0));
		}
	}

	void OnGUI() {
		//display background
		GUI.depth = 0;
		GUI.Box (new Rect (new Vector2 (400, 0), new Vector2 (320, 140)), "", boxStyle); 

		GUI.depth = 1;
		GUI.Label(new Rect (new Vector2 (400, 5), new Vector2 (320, 140)), "Time Control", titleStyle); 

		GUI.Label(new Rect (new Vector2 (460, 25), new Vector2 (320, 140)), "Time Mult: ", displayStyle); 

		GUI.Label(new Rect (new Vector2 (485, 65), new Vector2 (320, 140)), timeMult.ToString(), displayStyle); 

		if (GUI.Button (new Rect (new Vector2 (430, 100), new Vector2 (60, 30)), "Up")) {
			if (timeMult < 120) {
				timeMult += 6;
			}
		}

		if (GUI.Button (new Rect (new Vector2 (490, 100), new Vector2 (60, 30)), "Down")) {
			if (timeMult >= 6) {
				timeMult -= 6;
			}
		}

		GUI.Label(new Rect (new Vector2 (600, 25), new Vector2 (320, 140)), "Delta Mult: ", displayStyle); 

		GUI.Label(new Rect (new Vector2 (625, 65), new Vector2 (320, 140)), speed.ToString(), displayStyle); 

		if (GUI.Button (new Rect (new Vector2 (570, 100), new Vector2 (60, 30)), "Up")) {
			if (speed < 100) {
				speed += 5;
			}
		}

		if (GUI.Button (new Rect (new Vector2 (630, 100), new Vector2 (60, 30)), "Down")) {
			if (speed >= 5) {
				speed -= 5;
			}
		}

	}

}