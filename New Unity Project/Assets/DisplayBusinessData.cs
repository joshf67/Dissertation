using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBusinessData : MonoBehaviour {

	CurrentTime time = null;
	public GUIStyle BoxSkin;
	public GUIStyle TitleSkin;
	public GUIStyle DisplaySkin;
	public GUIStyle EmployeeSkin;

	Housing lastHousing = null;
	Housing housing = null;
	House currentHome = null;

	Business lastBusiness = null;
	Business bus = null;
	Job currentJob = null;

	HumanLife lastActor = null;
	HumanLife currentActor = null;

	public int countOfDisplayMax = 0;
	public int countOfDisplay = 0;
	public int countOfDisplayOffset = 0;
	public int offset = 0;
	public int offset2 = 0;

	void Start () {
		time = GameObject.FindObjectOfType<CurrentTime> ();
	}

	// Update is called once per frame
	void Update () {
		//raycast from mouse pointer to world space
		if (Input.GetMouseButton (0))
		{
			RaycastHit hit;
			if (Physics.Raycast (GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition), out hit))
			{
				//grab any connected script
				bus = null;
				housing = null;

				bus = hit.collider.GetComponent<Business> ();

				housing = hit.collider.GetComponent<Housing> ();

				currentActor = hit.collider.GetComponent<HumanLife> ();

				if (housing != null) {
					bus = null;
				}

				if (currentActor != null) {
					housing = null;
					bus = null;
				}

				//check if business has changed
				if (bus != null || housing != null || currentActor != null) {
					if (bus != lastBusiness || housing != lastHousing || currentActor != lastActor) {
						offset = 0;
						offset2 = 0;
						currentJob = null;
						currentHome = null;
					}
				}

				lastBusiness = bus;
				lastHousing = housing;
				lastActor = currentActor;
			}
		}
	}

	void OnGUI()
	{

		//check if business is selected
		if (bus != null) {
			
			//display background
			GUI.depth = 0;
			GUI.Box (new Rect (new Vector2 (0, 0), new Vector2 (210, 190)), "", BoxSkin); 

			//display business name
			GUI.depth = 1;
			GUI.TextField (new Rect (new Vector2 (5, 0), new Vector2 (190, 20)), bus.name, TitleSkin);




			if (currentJob != null) {

				//check if business is automated
				if (!currentJob.automated) {

					if (countOfDisplay >= currentJob.workers.Count) {
						countOfDisplay = currentJob.workers.Count;
						countOfDisplayOffset = 0;
					} else {
						countOfDisplay = countOfDisplayMax;
						countOfDisplayOffset = 1;
					}

					//display employee count
					GUI.Label (new Rect (new Vector2 (30, 50), new Vector2 (190, 20)), "Employees: "
					+ (offset2 + countOfDisplay) + " / " + (currentJob.workers.Count - countOfDisplayOffset), DisplaySkin);

					//allow scrolling of workers
					if (offset2 > 0) {
						if (GUI.Button (new Rect (new Vector2 (30, 80), new Vector2 (50, 20)), "Up")) {
							offset2--;
						}
					}

					//allow for back tracking
					if (GUI.Button (new Rect (new Vector2 (80, 80), new Vector2 (50, 20)), "Back")) {
						currentJob = null;
						return;
					}

					//allow scrolling of workers
					if (offset2 + countOfDisplay < currentJob.workers.Count - 1) {
						if (GUI.Button (new Rect (new Vector2 (30, 160), new Vector2 (50, 20)), "Down")) {
							offset2++;
						}
					}
					
					//loop through all workers currently being displayed
					for (int a = 0; a < countOfDisplay; a++) {
						if (offset + a < currentJob.workers.Count) {

							//display workers names
							GUI.Label (new Rect (new Vector2 (30, 100 + (a * 20)), new Vector2 (40, 20)), currentJob.workers [a + offset2].worker.name, DisplaySkin);

							//display fire button
							if (GUI.Button (new Rect (new Vector2 (130, 100 + (a * 20)), new Vector2 (50, 20)), "Fire")) {
								currentJob.removeWorker (currentJob.workers [a + offset2].worker, true);
								return;
							}
						}
					}


				} else {

					//display automated message
					GUI.Label (new Rect (new Vector2 (30, 50), new Vector2 (50, 20)), "Automated", DisplaySkin);

					//allow for back tracking
					if (GUI.Button (new Rect (new Vector2 (30, 80), new Vector2 (50, 20)), "Back")) {
						currentJob = null;
						return;
					}

				}

			} else {

				if (countOfDisplay >= bus.occupations.Count) {
					countOfDisplay = bus.occupations.Count;
					countOfDisplayOffset = 0;
				} else {
					countOfDisplay = countOfDisplayMax;
					countOfDisplayOffset = 1;
				}

				//display job count
				GUI.Label (new Rect (new Vector2 (30, 50), new Vector2 (190, 20)), "Jobs: "
					+ (offset + countOfDisplay) + " / " + (bus.occupations.Count - countOfDisplayOffset) + " Cash: " + (int)bus.cash, DisplaySkin);

				//allow scrolling of jobs
				if (offset > 0) {
					if (GUI.Button (new Rect (new Vector2 (30, 80), new Vector2 (50, 20)), "Up")) {
						offset--;
					}
				}

				//allow scrolling of jobs
				if (offset + countOfDisplay < bus.occupations.Count - 1) {
					if (GUI.Button (new Rect (new Vector2 (30, 160), new Vector2 (50, 20)), "Down")) {
						offset++;
					}
				}

				//display all houses
				for (int a = 0; a < countOfDisplay; a++) {

					if (offset + a < bus.occupations.Count) {
						if (GUI.Button (new Rect (new Vector2 (30, 100 + (a * 20)), new Vector2 (150, 20)), bus.occupations [a + offset].name)) {
							currentJob = bus.occupations [a + offset];
							return;
						}
					}

				}

			}
		}
		//check if house is picked
		else if (housing != null) {
			//display background
			GUI.depth = 0;
			GUI.Box (new Rect (new Vector2 (0, 0), new Vector2 (210, 190)), "", BoxSkin); 

			//display house name
			GUI.depth = 1;
			GUI.TextField (new Rect (new Vector2 (5, 0), new Vector2 (190, 20)), housing.name, TitleSkin);

			//check if home is selected
			if (currentHome != null) {

				if (countOfDisplay >= currentHome.occupants.Count) {
					countOfDisplay = currentHome.occupants.Count;
					countOfDisplayOffset = 0;
				} else {
					countOfDisplay = countOfDisplayMax;
					countOfDisplayOffset = 1;
				}

				//display occupant count
				GUI.Label (new Rect (new Vector2 (30, 50), new Vector2 (190, 20)), "Occupants: "
				+ (offset2 + countOfDisplay) + " / " + (currentHome.occupants.Count - countOfDisplayOffset), DisplaySkin);

				//allow for back tracking
				if (GUI.Button (new Rect (new Vector2 (80, 80), new Vector2 (50, 20)), "Back")) {
					currentHome = null;
					return;
				}

				//allow scrolling of actors
				if (offset2 > 0) {
					if (GUI.Button (new Rect (new Vector2 (30, 80), new Vector2 (50, 20)), "Up")) {
						offset2--;
					}
				}

				//allow scrolling of actors
				if (offset2 + countOfDisplay < currentHome.occupants.Count - 1) {
					if (GUI.Button (new Rect (new Vector2 (30, 160), new Vector2 (50, 20)), "Down")) {
						offset2++;
					}
				}

				//display all actors living here
				for (int a = 0; a < countOfDisplay; a++) {
					
					if (offset2 + a < currentHome.occupants.Count) {
						GUI.Label (new Rect (new Vector2 (30, 100 + (a * 20)), new Vector2 (40, 20)), currentHome.occupants [a + offset2].name, DisplaySkin);
					}

				}

			} else {

				if (countOfDisplay >= housing.houses.Count) {
					countOfDisplay = housing.houses.Count;
					countOfDisplayOffset = 0;
				} else {
					countOfDisplay = countOfDisplayMax;
					countOfDisplayOffset = 1;
				}

				//display house name
				GUI.depth = 1;
				GUI.TextField (new Rect (new Vector2 (5, 0), new Vector2 (190, 20)), housing.name, TitleSkin);

				GUI.Label (new Rect (new Vector2 (30, 45), new Vector2 (190, 20)), "Houses: "
					+ (offset + countOfDisplay) + " / " + (housing.houses.Count - countOfDisplayOffset), DisplaySkin);

				GUI.Label (new Rect (new Vector2 (30, 60), new Vector2 (190, 20)), "Cash: " + (int)housing.cash, DisplaySkin);

				//allow scrolling of actors
				if (offset > 0) {
					if (GUI.Button (new Rect (new Vector2 (30, 80), new Vector2 (50, 20)), "Up")) {
						offset--;
					}
				}

				//allow scrolling of actors
				if (offset + countOfDisplay < housing.houses.Count - 1) {
					if (GUI.Button (new Rect (new Vector2 (30, 160), new Vector2 (50, 20)), "Down")) {
						offset++;
					}
				}

				//display all houses
				for (int a = 0; a < countOfDisplay; a++) {

					if (offset + a < housing.houses.Count) {

						if (GUI.Button (new Rect (new Vector2 (30, 100 + (a * 20)), new Vector2 (150, 20)), housing.houses [a + offset].name)) {
							currentHome = housing.houses [a + offset].data;
						}

					}

				}

			}
			
		} else if (currentActor != null) {
			//display background
			GUI.depth = 0;
			GUI.Box (new Rect (new Vector2 (0, 0), new Vector2 (210, 190)), "", BoxSkin); 

			//display actor name
			GUI.depth = 1;
			GUI.TextField (new Rect (new Vector2 (5, 0), new Vector2 (190, 20)), currentActor.name, TitleSkin);

			//display stats
			GUI.Label (new Rect (new Vector2 (30, 50), new Vector2 (190, 20)), "Stats:   " + "Cash: " + currentActor.cash, DisplaySkin);

			GUI.Label (new Rect (new Vector2 (30, 70), new Vector2 (190, 20)), "Strength: " + currentActor.stats.strength, DisplaySkin);

			GUI.Label (new Rect (new Vector2 (30, 90), new Vector2 (190, 20)), "Intellegence: " + currentActor.stats.intelligence, DisplaySkin);
			
			GUI.Label (new Rect (new Vector2 (30, 110), new Vector2 (190, 20)), "Dexerity: " + currentActor.stats.dexterity, DisplaySkin);

			if (currentActor.stats.job != null) {
				GUI.Label (new Rect (new Vector2 (30, 130), new Vector2 (190, 20)), "Job: " + currentActor.stats.job.name, DisplaySkin);
				GUI.Label (new Rect (new Vector2 (30, 150), new Vector2 (190, 20)), "Income: " + currentActor.stats.job.pay + " Per Hour", DisplaySkin);
				GUI.Label (new Rect (new Vector2 (30, 170), new Vector2 (190, 20)), "Hours: " + currentActor.stats.job.hours.workHours, DisplaySkin);
			}

		}
	}
}