using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

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

	// Use this for initialization
	void Start () {
	
		timeText = GetComponent<Text>();
		countdownText.text = "READY?";
		StartCoroutine ("StartGame", 5);
		//InvokeRepeating("CountDown", 1, 0.1f);
		StartCoroutine("GameTimer", timeInitial);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
			timeInitial -= 0.01f;
			currentTime = timeInitial;
			timeText.text = "" + Math.Round(timeInitial, 1);
			yield return new WaitForSeconds(0.01f);
		}

		countdownText.text = "GAME OVER!";
	}

	// Older version of timer from 2.0 build
	/*void CountDown()
    {
        timeInitial -= 0.1f;
        currentTime = timeInitial;
        timeText.text = "" + Math.Round(timeInitial, 1);

        if(timeInitial <= 0) {
            CancelInvoke();

            Debug.Log("Time is Up");
        }
    }*/
	
}
