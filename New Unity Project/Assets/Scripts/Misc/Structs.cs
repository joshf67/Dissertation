using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//struct to store all furniture in a room
[System.Serializable]
public struct room {
	public List<Furniture> furniture;
}

//struct to store actors stats
[System.Serializable]
public struct Stats
{
	public List<Relation> relationship;
	public AccomodationData accomodation;
	public bool wantKids;

	public int age, ageDays, intelligence, strength, dexterity;
	public float intellegenceGrowth, strengthGrowth, dexterityGrowth;

	public bool inEducation;

	public Job job;
	public JobData jobData;
	public Business company;

	public float income;
	public float travelHours;
	public bool hasTask;

	public Vector2 sleepHours;
}

//struct to store job information for actors
[System.Serializable]
public struct JobDetails
{
	public Vector2 workHours;
	public Vector2 breakTime;
}

//struct to store relationships between actors
[System.Serializable]
public struct Relation
{
	public HumanLife other;
	public int relationType;

	public Relation(HumanLife _other, int type) {
		other = _other;
		relationType = type;
	}
}

//struct to store actors accomodation data
[System.Serializable]
public struct AccomodationData {
	public List<HumanLife> occupants;
	public int requiredRooms;
	public GameObject home;
	public House homeData;
}

//struct to store job's required stats
[System.Serializable]
public struct JobStats {
	public int intellegence;
	public int strength;
	public int dexterity;
	public Vector2 age;
}

//struct to store delivery data
[System.Serializable]
public struct DeliveryInfo {
	public List<Item> items;
	public List<float> itemPrices;
	public HumanLife recipient;
}

//struct to store data required for testing shops
//for products
[System.Serializable]
public struct ShopTest {
	public itemTypes productType;
	public string productName;
	public HumanLife person;

	public ShopTest (itemTypes type = 0, string name = "", HumanLife life = null) {
		productType = type;
		productName = name;
		person = life;
	}
}

//struct to store data for items
[System.Serializable]
public struct Item {

	public itemTypes itemType;
	public string name;
	public float effect;

}

//stuct to store data for use in buying
//products from stores/businesses
[System.Serializable]
public struct PurchaseOptions {

	public int index;
	public bool delivery;
	public bool online;
	public HumanLife buyer;

	public PurchaseOptions (int _index = 0, bool _delivery = false, bool _online = false, HumanLife _buyer = null) {
		index = _index;
		online = _online;
		delivery = _delivery;
		buyer = _buyer;
	}

}