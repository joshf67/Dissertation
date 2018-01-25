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

	public int[] hourPurchases = new int[24];
	
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
	}

}