using UnityEngine;
using System.Collections;

/// <summary>
/// Basic level in which food GameObjects are instantiated
/// at randomly generated positions along the x-axis 
/// @author Emily Sharp
/// </summary>
public class RandomFoodSpawner : MonoBehaviour {

	//Slot holders for food GameObjects to be instantiated 
	public GameObject food1;
	public GameObject food2;
	public GameObject food3;
	public GameObject food4;
	public GameObject food5;
	public GameObject food6;

	private float nextFoodTime;

	// The amount of time in seconds b/w gameobject spawning
	public float spawnRate;

	//Boundaries of the spawnable area based on Kinect camera limitations
	private const float minBoundaryX = 400;
	private const float maxBoundaryX = 1400;

	private const float maxBoundaryY = 1150;
	private const float minBoundaryY = -20;

	public static int numFoodSpawned;

	// Use this for initialization
	void Start () {
		numFoodSpawned = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (TimerCountdown.currentTime > 0)
		{
			if (nextFoodTime < Time.time)
			{
				SpawnFood(GenerateRandomPrefab());

				nextFoodTime = Time.time + spawnRate;
				Debug.Log(nextFoodTime);
				spawnRate = Mathf.Clamp(spawnRate, 0.3f, 99f);
			}
		}
		else
		{
			//Stop spawning items
		}
	}

	// Uses random number generator to select which type of 
	// GameObject to spawn (food item)
	GameObject GenerateRandomPrefab()
	{
		GameObject randPrefab = null;

		int randNumber = Random.Range(1, 7);
		switch (randNumber)
		{
		case 1:
			randPrefab = food1;
			break;
		case 2:
			randPrefab = food2;
			break;
		case 3:
			randPrefab = food3;
			break;
		case 4:
			randPrefab = food4;
			break;
		case 5:
			randPrefab = food5;
			break;
		case 6:
			randPrefab = food6;
			break;
		default:
			break;
		}
		return randPrefab;
	}

	// Instantiates food item at randomly generated position along x-axis
	void SpawnFood(GameObject randPrefab)
	{
		float randPosX = Random.Range(minBoundaryX, maxBoundaryX);
		Vector3 spawnPos = new Vector3(randPosX, maxBoundaryY, 0);

		GameObject randomFood = Instantiate(randPrefab, spawnPos, Quaternion.identity) as GameObject;
	}

}
