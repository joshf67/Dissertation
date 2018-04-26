using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class CalorieBurnTable : MonoBehaviour {

	[SerializeField]
	public List<CalorieDict> calorieBurn;

	//function that gets current calorie cost of action
	public float taskCost(string task) {
		int index = -1;

		//search for element and return index if it exists
		if (CalorieDict.Contains (calorieBurn, new CalorieDict(task, 0), out index)) {
			return calorieBurn [index].cost;
		}

		//log an error if the element doesn't exist
		Debug.LogError ("Task does not exist: " + task);
		return 0;
	}

	//function that returns the calorie cost per second
	public float TaskCostPerSecond(string task) {
		return taskCost (task) * Time.deltaTime;
	}

}

#if UNITY_EDITOR

[CustomEditor(typeof(CalorieBurnTable))]
[CanEditMultipleObjects]
public class CalorieBurnEditor : Editor
{

	string calorieBurnTask;
	float calorieBurnValue;

	public override void OnInspectorGUI()
	{

		serializedObject.Update ();

		//grab current calorie table and store it in a local variable
		CalorieBurnTable _target = (CalorieBurnTable)target;
		List<CalorieDict> tempDict = _target.calorieBurn;

		if (tempDict == null) {
			tempDict = new List<CalorieDict> ();
		}

		//save and load data
		EditorGUILayout.BeginHorizontal ();

		//setup directory
		string dir = Application.dataPath + Path.DirectorySeparatorChar
						+ "CalorieBurnData" + Path.DirectorySeparatorChar;

		//save calorie dict to file
		if (GUILayout.Button ("Save"))
		{
			//check if directory exists
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			//check if calorie data exists
			if (File.Exists(dir + "CalorieBurn.txt"))
			{
				File.Delete(dir + "CalorieBurn.txt");
			}

			StreamWriter writer = new StreamWriter (dir + "CalorieBurn.txt");
			string lineData;

			writer.WriteLine (tempDict.Count.ToString ());

			//save all calorie data to file
			for (int a = 0; a < tempDict.Count; a++) {
				lineData = "";
				lineData += tempDict [a].task;
				lineData += ":";
				lineData += tempDict [a].cost.ToString();
				writer.WriteLine (lineData);
			}

			writer.Close ();
		}

		//load calorie dict to file
		if (GUILayout.Button ("Load"))
		{
			//check if directory exists
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			//check if calorie data exists
			if (File.Exists(dir + "CalorieBurn.txt"))
			{
				StreamReader reader = new StreamReader (dir + "CalorieBurn.txt");

				tempDict.Clear ();
				string lineData = "";
				string[] lineDataSplit = new string[0];
				int count = int.Parse(reader.ReadLine ());

				//save all calorie data to table contents
				for (int a = 0; a < count; a++) {
					lineData = "";
					lineData = reader.ReadLine ();
					lineDataSplit = lineData.Split (':');
					tempDict.Add (new CalorieDict (lineDataSplit[0], float.Parse(lineDataSplit[1])));
				}

				reader.Close ();
			}

		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();

		//add new calorie burn index
		if (GUILayout.Button ("Add"))
		{
			int index = -1;
			//check if it already exists
			if (!CalorieDict.Contains (tempDict, new CalorieDict (calorieBurnTask, calorieBurnValue), out index)) {
				tempDict.Add (new CalorieDict (calorieBurnTask, calorieBurnValue));
			} else {
				//edit it if it does
				tempDict [index] = new CalorieDict(calorieBurnTask, calorieBurnValue);
			}
		}

		//display add data section
		calorieBurnTask = EditorGUILayout.TextField(calorieBurnTask);
		calorieBurnValue = EditorGUILayout.FloatField(calorieBurnValue);

		//setup new calorie dictionary
		CalorieDict tempDictChange = new CalorieDict ();

		EditorGUILayout.EndHorizontal ();

		//loop through displaying all calorie indexs
		for (int a = 0; a < tempDict.Count; a++) {
			EditorGUILayout.BeginHorizontal ();

			//store any changes make to current calorie value
			tempDictChange.task = EditorGUILayout.TextField (tempDict[a].task);
			tempDictChange.cost = EditorGUILayout.FloatField (tempDict[a].cost);


			if (tempDictChange.task != tempDict [a].task || tempDictChange.cost != tempDict [a].cost) {
				tempDict [a] = tempDictChange;
				break;
			}

			//display remove button to delete index
			if (GUILayout.Button ("Remove"))
			{
				tempDict.RemoveAt (a);
				break;
			}

			EditorGUILayout.EndHorizontal ();
		}
			
		//apply changes to calorieDict
		_target.calorieBurn = tempDict;
		serializedObject.ApplyModifiedProperties ();

	}

}


#endif

//hold data about calorieBurn
[System.Serializable]
public struct CalorieDict
{

	public string task;
	public float cost;

	//default constructor for calorie data
	public CalorieDict(string _task, float _cost)
	{
		task = _task;
		cost = _cost;
	}

	//override default comparison to custom one
	public static bool operator == (CalorieDict one, CalorieDict two)
	{
		return one.task == two.task;
	}

	//override default comparison to custom one
	public static bool operator != (CalorieDict one, CalorieDict two)
	{
		return one.task != two.task;
	}

	//function that checks if a element is within calorieDict
	public static bool Contains(List<CalorieDict> list, CalorieDict test, out int index)
	{
		//loop through and return true if element has been found
		for (int a = 0; a < list.Count; a++) {
			if (test == list[a]) {
				//update index if found
				index = a;
				return true;
			}
		}
		index = -1;
		return false;
	}

}