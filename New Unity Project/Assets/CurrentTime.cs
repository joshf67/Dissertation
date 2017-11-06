using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTime : MonoBehaviour {

	public Vector3 HMS;
	public float timeMult;
	public Vector2 CurrentHMS;
	public float currentTime;
	public Text text;
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime * timeMult;
		//seconds
		if (currentTime >= HMS.z) {
			CurrentHMS.y += 1;
			currentTime -= HMS.z;
		}

		//minutes
		if (CurrentHMS.y >= HMS.y) {
			CurrentHMS.x += 1;
			CurrentHMS.y -= HMS.y;
		}

		//hours
		if (CurrentHMS.x >= HMS.x) {
			CurrentHMS.x = 0;
			CurrentHMS.y = 0;
		}

		string output = null;
		if (CurrentHMS.x < 10) {
			output += "0";
		}

		output += CurrentHMS.x + ":";

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
