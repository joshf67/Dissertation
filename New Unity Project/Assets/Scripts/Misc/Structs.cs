using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct room {
	public List<Furniture> furniture;
}

[System.Serializable]
public struct Stats
{
	public List<relation> relationship;
	public accomodationData accomodation;
	public bool wantKids;

	public int age, ageDays, intelligence, strength, dexterity;
	public float intellegenceGrowth, strengthGrowth, dexterityGrowth;

	public bool inEducation;

	public string jobTitle;
	public Job job;
	public JobData jobData;
	public Business company;

	public float income;
	public float travelHours;
	public bool hasTask;

	public Vector2 sleepHours;
}

[System.Serializable]
public struct jobDetails
{
	public Vector2 workHours;
	public Vector2 breakTime;
}

[System.Serializable]
public struct relation
{
	public HumanLife other;
	public int relationType;

	public relation(HumanLife _other, int type) {
		other = _other;
		relationType = type;
	}
}

[System.Serializable]
public struct accomodationData {
	public List<HumanLife> occupants;
	public int requiredRooms;
	public GameObject home;
	public House homeData;
}

[System.Serializable]
public struct jobStats {
	public int intellegence;
	public int strength;
	public int dexterity;
	public Vector2 age;
}

[System.Serializable]
public struct deliveryInfo {
	public List<item> items;
	public HumanLife recipient;
}

[System.Serializable]
public struct shopTest {
	public itemTypes productType;
	public string productName;
	public float cash;
	public HumanLife person;

	public shopTest (itemTypes type = 0, string name = "", float money = 0, HumanLife life = null) {
		productType = type;
		productName = name;
		cash = money;
		person = life;
	}
}

[System.Serializable]
public struct item {

	public int itemType;
	public string name;
	public float effect;

}

/*
[System.Serializable]
public struct House {

	public List<HumanLife> occupants;
	public List<room> rooms;

	public float cost;
	public float initialCost;
	public int costDays;

	public Vector3 position;
	public GameObject obj;
	public Housing ownerScript;
	public int ownerScriptIndex;
}
*/