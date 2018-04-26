using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTime : MonoBehaviour {

	//hold data for second, minute, hour, ect... maxes
	public Vector3 DWM = new Vector3(7,4,12);
	public Vector4 CurrentDWMY;
	public Vector3 HMS = new Vector3(60,60,24);

	//hold data for time manipulation
	public float timeMult = 1;
	public float speed = 1;

	//store current time
	public Vector2 CurrentHMS;
	public float currentTime;

	//hold data for UI
	public Text text;
	public GUIStyle boxStyle;
	public GUIStyle titleStyle;
	public GUIStyle displayStyle;
	public bool expandUI = false;

	// Update is called once per frame
	void Update () {
		Time.timeScale = speed;
		currentTime += Time.deltaTime * timeMult;

		//calculate seconds
		while (currentTime >= HMS.z) {
			CurrentHMS.y += 1;
			currentTime -= HMS.z;
		}

		//calculate minutes
		while (CurrentHMS.y >= HMS.y) {
			CurrentHMS.x += 1;
			CurrentHMS.y -= HMS.y;
		}

		//calculate hours
		while (CurrentHMS.x >= HMS.x) {
			CurrentHMS.x -= HMS.x;
			CurrentHMS.y = 0;
			CurrentDWMY.x += 1;
		}

		//calculate weeks
		if (CurrentDWMY.x >= DWM.z) {
			CurrentDWMY.y += 1;
			CurrentDWMY.x = 0;
			foreach (HumanLife person in GameObject.FindObjectsOfType<HumanLife>()) {
				person.updateAge ();
			}
		}

		//calculate months
		if (CurrentDWMY.y >= DWM.y) {
			CurrentDWMY.z += 1;
			CurrentDWMY.y = 0;
		}

		//calculate years
		if (CurrentDWMY.z >= DWM.x) {
			CurrentDWMY.w += 1;
			CurrentDWMY.z = 0;
		}
			
		string output = null;

		//store current date
		output += "Day:" + CurrentDWMY.x;
		output += "\n";
		output += "Week:" + CurrentDWMY.y;
		output += "\n";
		output += "Month:" + CurrentDWMY.z;
		output += "\n";
		output += "Year:" + CurrentDWMY.w;
		output += "\n";

		//store and display current time {

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

		//}

		//update and display global cash {
		float globalCash = 0;
		foreach(Business bus in GameObject.FindObjectsOfType<Business>()) {
			globalCash += bus.cash;
		}

		foreach(HumanLife hum in GameObject.FindObjectsOfType<HumanLife>()) {
			globalCash += hum.cash;
		}

		output += "\nGlobal Cash : " + ((int)globalCash);

		//}

		//display all data above
		text.text = output;

	}

	void OnGUI() {

		//test if the UI is expanded
		if (!expandUI) {

			//display background
			GUI.depth = 0;
			GUI.Box (new Rect (new Vector2 (400, 0), new Vector2 (320, 30)), "", boxStyle); 

			//display title
			GUI.depth = 1;
			//display buttons to modify expandUI variable
			if (GUI.Button (new Rect (new Vector2 (430, 5), new Vector2 (260, 20)), "Time Control")) {
				expandUI = !expandUI;
			}

		} 
		else {
		
			//display background
			GUI.depth = 0;
			GUI.Box (new Rect (new Vector2 (400, 0), new Vector2 (320, 140)), "", boxStyle); 

			//display title
			GUI.depth = 1;
			//display buttons to modify expandUI variable
			if (GUI.Button (new Rect (new Vector2 (430, 5), new Vector2 (260, 20)), "Time Control")) {
				expandUI = !expandUI;
			}

			//display time mult variable
			GUI.Label (new Rect (new Vector2 (460, 25), new Vector2 (320, 140)), "Time Mult: ", displayStyle); 
			GUI.Label (new Rect (new Vector2 (485, 65), new Vector2 (320, 140)), timeMult.ToString (), displayStyle); 

			//display buttons to modify above variable
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

			//display delta time variable
			GUI.Label (new Rect (new Vector2 (600, 25), new Vector2 (320, 140)), "Delta Mult: ", displayStyle); 
			GUI.Label (new Rect (new Vector2 (625, 65), new Vector2 (320, 140)), speed.ToString (), displayStyle); 

			//display buttons to modify above variable
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

}