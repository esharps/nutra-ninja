using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DynamicFoodSpawner : MonoBehaviour {

	// Queues that dynamically track X, Y coordinates of player center of mass
	Queue<float> cmQueue_X;
	Queue<float> cmQueue_Y;

	// Value of countdown timer at which center of mass analysis begins
	public float beginDetection = 81.0f; // Three 3-second cycles

	double timeCurrent;

	// Way of preventing Unity from recording CM after EVERY frame (avg. 61 FPS)
	int frameOffset;

	float deltaTime = 0.0f;

	//Boundaries of the spawnable area based on Kinect camera limitations
	private const float minBoundaryX = 400;
	private const float maxBoundaryX = 1400;

	private const float maxBoundaryY = 1175;
	private const float minBoundaryY = -40;

	private const float centerX = 950;

	// Amount of time (seconds) elapsed b/w gameObject spawning
	public float spawnRate = 1.1f;

	private float nextFoodTime;

	float testSum;

	public Text difficultyDisp;
	public Text cmDisplay;

	public float spawnOffset = 250;

	//This is the velocity we would like to keep the player moving at
	public float minPlayerVelocity;

	// Minimum velocity player must maintain on EASY
	public float minVelocityEasy = 5;

	// Minimum velocity player must maintain on MEDIUM
	public float minVelocityMed = 6;

	// Minimum velocity player must maintain on HARD
	public float minVelocityHard = 8;

	//Slot holders for light-weight food items (slow moving)
	public GameObject lightFood1;
	public GameObject lightFood2;
	public GameObject lightFood3;
	public GameObject lightFood4;
	public GameObject lightFood5;
	public GameObject lightFood6;

	//Slot holders for medium-weight food items (faster moving)
	public GameObject medFood1;
	public GameObject medFood2;
	public GameObject medFood3;
	public GameObject medFood4;
	public GameObject medFood5;
	public GameObject medFood6;

	//Slot holders for heavy food items (very fast moving)
	public GameObject heavyFood1;
	public GameObject heavyFood2;
	public GameObject heavyFood3;
	public GameObject heavyFood4;
	public GameObject heavyFood5;
	public GameObject heavyFood6;

	// Current difficulty of dynamic level: 1 = EASY, 2 = MED, 3 = HARD
	public int GAME_STATE;


	// Use this for initialization
	void Start () {
		GAME_STATE = 1;
		//StartCoroutine(StartCountdown());

		cmQueue_X = new Queue<float>();
		cmQueue_Y = new Queue<float>();

		testSum = 0;

		timeCurrent = TimerCountdown.currentTime;

		// Collecting Kinect data every other frame
		frameOffset = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
		switch (GAME_STATE)
		{
		case 1:
			difficultyDisp.text = "EASY";
			break;
		case 2:
			difficultyDisp.text = "MEDIUM";
			break;
		case 3:
			difficultyDisp.text = "DIFFICULT";
			break;
		default:
			break;
		}

		cmDisplay.text = "(" + NinjaJointManager.spineMidX + ", " + NinjaJointManager.spineMidY + ")";

		/** Basic framerate calculation for hardware diagnostics*/
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float frameRate = 1 / deltaTime;

		//Debug.Log("Frame Rate: " + frameRate);

		if (TimerCountdown.currentTime < 90)
		{
			TrackPlayerCM();
		}

		if (TimerCountdown.currentTime > 0)
		{
			if (nextFoodTime < Time.time)
			{
				SpawnFoodDynamic();

				nextFoodTime = Time.time + spawnRate;
				spawnRate = Mathf.Clamp(spawnRate, 0.3f, 99f);
			}
		}

       /*
		if (frameOffset > 0)
        {
            cmQueue_X.Enqueue(NinjaJointManager.spineMidX);
            cmQueue_Y.Enqueue(NinjaJointManager.spineMidY);
        }
        // Should alternate between every other frame
        frameOffset = frameOffset * (-1);


        testSum += NinjaJointManager.spineMidX;

        //Debug.Log(NinjaJointManager.spineMidX);

        timeCurrent = Math.Round(TimerCountdown.currentTime, 2);
       
		//Debug.Log(timeCurrent);
		if (timeCurrent <= 90 && (timeCurrent % 3 == 0))
        {
            DetectAdjustment();

            cmQueue_X.Clear();
            cmQueue_Y.Clear();

            testSum = 0;
        }*/
	}

	/// <summary>
	/// Captures the x,y coordinate values player CM every other update
	/// frame and adds information to two queues. Every three seconds the
	/// queue data is analyzed by the DetectAdjustment method to determine 
	/// if the player is constantly moving
	/// </summary>
	public void TrackPlayerCM()
	{
		if (frameOffset > 0)
		{
			cmQueue_X.Enqueue(NinjaJointManager.spineMidX);
			cmQueue_Y.Enqueue(NinjaJointManager.spineMidY);
		}

		// Should alternate between every other frame
		frameOffset = frameOffset * (-1);


		testSum += NinjaJointManager.spineMidX;
		//Debug.Log("testSum: " + testSum);

		//Debug.Log(NinjaJointManager.spineMidX);

		timeCurrent = Math.Round(TimerCountdown.currentTime, 2);
		//Debug.Log("Queue Size: " + cmQueue_X.Count);

		//Debug.Log(timeCurrent);
		if (timeCurrent % 3 == 0)
		{
			DetectAdjustment();

			cmQueue_X.Clear();
			cmQueue_Y.Clear();

			testSum = 0;
		}
	}


	// Calculates the average velocity across a three-second time period
	// and determines if food spawning needs to be dynamically adjusted

	public void DetectAdjustment()
	{
		// Debug.Log(cmQueue_X.Count);

		float[] xCmArray = new float[cmQueue_X.Count];
		xCmArray = cmQueue_X.ToArray();

		float[] yCmArray = new float[cmQueue_Y.Count];
		yCmArray = cmQueue_Y.ToArray();

		//Debug.Log("Array Length: " + xCmArray.Length);

		//Sum of the change in position of CM along x-axis
		float positionSumX = 0;
		//Sum of the change in position of CM along y-axis
		float positionSumY = 0;

		//Temp storage location for value in for-loop
		float tempDisplace = 0;

		// Average displacement of player in pixels
		float average = 0;

		for (int i = 0; i < xCmArray.Length - 1; i++)
		{
			// Final position - initial position
			tempDisplace = xCmArray[i + 1] - xCmArray[i];

			// Cheat way of taking the absolute value
			if (tempDisplace < 0)
				tempDisplace = tempDisplace * (-1);

			// Add to the summation of change in positions
			positionSumX += tempDisplace;

		}
		//Debug.Log("Sum: " + positionSumX);

		// Converting from pixels/frame to pixels/second
		average = (positionSumX / xCmArray.Length)*30;

		Debug.Log("-----AVERAGE Displacement: " + average);

		// Too easy
		if (average < minPlayerVelocity && ScoreManager.currentScore > 100)
		{
			GAME_STATE = 2;
			spawnRate = 0.9f;
		}
		if (GAME_STATE == 2 && ScoreManager.currentScore > 200)
		{
			GAME_STATE = 3;
			spawnRate = 0.75f;
		}
		if (GAME_STATE == 3 && ScoreManager.currentScore < 0)
		{
			GAME_STATE = 2;
			spawnRate = 0.9f;
		}
		if (GAME_STATE == 2 && ScoreManager.currentScore < 0)
		{
			GAME_STATE = 1;
			spawnRate = 1.1f;
		}

	}

	void SpawnFoodDynamic()
	{
		float randPosX = UnityEngine.Random.Range(minBoundaryX, maxBoundaryX);

		//If player's body is currently left of center, spawn to the right
		if (NinjaJointManager.spineMidX < centerX)
			randPosX = Mathf.Clamp((randPosX + spawnOffset), minBoundaryX, maxBoundaryX);
		//If player's body is right of center, spawn towards the left
		else
			randPosX = Mathf.Clamp((randPosX - spawnOffset), minBoundaryX, maxBoundaryX);

		Vector3 spawnPos = new Vector3(randPosX, maxBoundaryY, 0);

		if (GAME_STATE == 1)
		{  
			GameObject randomFood = Instantiate(LightweightPrefabGenerator(), spawnPos, Quaternion.identity) as GameObject;
		}
		if(GAME_STATE == 2)
		{
			GameObject randomFood = Instantiate(MediumWeightPrefabGenerator(), spawnPos, Quaternion.identity) as GameObject;
		}
		if(GAME_STATE == 3)
		{
			GameObject randomFood = Instantiate(HeavyWeightPrefabGenerator(), spawnPos, Quaternion.identity) as GameObject;
		}

	}

	GameObject LightweightPrefabGenerator()
	{
		GameObject lightPrefab = null;

		//Generate number between 1 and 7 to select random light weight obj
		int randNumber = UnityEngine.Random.Range(1, 7);

		switch (randNumber)
		{
		case 1:
			lightPrefab = lightFood1;
			break;
		case 2:
			lightPrefab = lightFood2;
			break;
		case 3:
			lightPrefab = lightFood3;
			break;
		case 4:
			lightPrefab = lightFood4;
			break;
		case 5:
			lightPrefab = lightFood5;
			break;
		case 6:
			lightPrefab = lightFood6;
			break;
		default:
			break;
		}
		return lightPrefab;
	}

	GameObject MediumWeightPrefabGenerator()
	{
		GameObject mediumPrefab = null;

		//Generate number between 1 and 7 to select random medium weight obj
		int randNumber = UnityEngine.Random.Range(1, 13);

		switch (randNumber)
		{
		case 1:
			mediumPrefab = medFood1;
			break;
		case 2:
			mediumPrefab = medFood2;
			break;
		case 3:
			mediumPrefab = medFood3;
			break;
		case 4:
			mediumPrefab = medFood4;
			break;
		case 5:
			mediumPrefab = medFood5;
			break;
		case 6:
			mediumPrefab = medFood6;
			break;
		case 7:
			mediumPrefab = lightFood1;
			break;
		case 8:
			mediumPrefab = lightFood2;
			break;
		case 9:
			mediumPrefab = lightFood3;
			break;
		case 10:
			mediumPrefab = lightFood4;
			break;
		case 11:
			mediumPrefab = lightFood5;
			break;
		case 12:
			mediumPrefab = lightFood6;
			break;
		default:
			break;
		}
		return mediumPrefab;
	}

	GameObject HeavyWeightPrefabGenerator()
	{
		GameObject heavyPrefab = null;

		//Generate number between 1 and 7 to select random heavy weight obj
		int randNumber = UnityEngine.Random.Range(1, 19);

		switch (randNumber)
		{
		case 1:
			heavyPrefab = heavyFood1;
			break;
		case 2:
			heavyPrefab = heavyFood2;
			break;
		case 3:
			heavyPrefab = heavyFood3;
			break;
		case 4:
			heavyPrefab = heavyFood4;
			break;
		case 5:
			heavyPrefab = heavyFood5;
			break;
		case 6:
			heavyPrefab = heavyFood6;
			break;
		case 7:
			heavyPrefab = medFood1;
			break;
		case 8:
			heavyPrefab = medFood2;
			break;
		case 9:
			heavyPrefab = medFood3;
			break;
		case 10:
			heavyPrefab = medFood4;
			break;
		case 11:
			heavyPrefab = medFood5;
			break;
		case 12:
			heavyPrefab = medFood6;
			break;
		case 13:
			heavyPrefab = lightFood1;
			break;
		case 14:
			heavyPrefab = lightFood2;
			break;
		case 15:
			heavyPrefab = lightFood3;
			break;
		case 16:
			heavyPrefab = lightFood4;
			break;
		case 17:
			heavyPrefab = lightFood5;
			break;
		case 18:
			heavyPrefab = lightFood6;
			break;
		default:
			break;
		}
		return heavyPrefab;
	}

	private IEnumerator StartCountdown()
	{
		yield return new WaitForSeconds(9.5f);
	}
}
