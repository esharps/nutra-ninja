using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Timers;

/// <summary>
/// Initializes a countdown sequence at the beginning of each level
/// in which the game counts down from 5 seconds before starting the 
/// game (i.e. items begin spawning)
/// </summary>
public class TimerCountdown : MonoBehaviour {

	public float timeInitial = 60.0f;
	public Text timeText;
	public Text countdownText; 
	public static float currentTime;
	Timer myTimer;


	// Use this for initialization
	void Start () {

		myTimer = new System.Timers.Timer (1000);
		myTimer.Elapsed += (object sender, ElapsedEventArgs e) => timeInitial--;

	
		timeText = GetComponent<Text>();
		countdownText.text = "READY?";
		StartCoroutine ("StartGame", 5);
		//InvokeRepeating("CountDown", 1, 0.1f);
		StartCoroutine("GameTimer");
	}
	
	// Update is called once per frame
	/*void Update () {
	
		timeInitial -= Time.deltaTime;
		currentTime = timeInitial;

		timeText.text = "" + Math.Round(timeInitial, 1);

		if (timeInitial <= 0) {
			timeInitial = 0.0f;

		}
	}*/

	/// <summary>
	/// Want to initiate a countdown sequence to delay food spawn
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	private IEnumerator StartGame(int time)
	{
		yield return new WaitForSeconds(2.5f);

		for (int i = time; i > 0; i--)
		{
			countdownText.text = "" + i;
			yield return new WaitForSeconds(1.0f);
		}

		countdownText.text = "GO!";
		yield return new WaitForSeconds(1.0f);
		countdownText.text = "";

		//dynamicSpawner = new DynamicFoodSpawner();
	}

	//TODO Timing is still not perfect, look into another remedy for timer
	private IEnumerator GameTimer()
	{
		yield return new WaitForSeconds(8.5f);

		while (timeInitial > 0)
		{
			timeInitial -= Time.deltaTime;

			//timeInitial -= 0.01f;
			currentTime = timeInitial;
			timeText.text = "" + currentTime;
			//yield return new WaitForSeconds(0.01f);*/
		}

		countdownText.text = "GAME OVER!";
	}

	// Older version of timer from 2.0 build
	void CountDown()
    {
		myTimer = new System.Timers.Timer (1000);
		myTimer.Elapsed += (object sender, ElapsedEventArgs e) => timeInitial--;

		if (timeInitial <= 0)
			countdownText.text = "GAME OVER";
       
    }
	
}
