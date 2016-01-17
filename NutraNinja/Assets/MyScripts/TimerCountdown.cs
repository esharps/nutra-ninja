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
		StartCoroutine("GameTimer");
	}

	/// <summary>
	/// Want to initiate a countdown sequence that uses a 
	/// coroutine to delay the food spawning gameplay; once
	/// countdown is complete, the timer starts and gameplay begins
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
	}
		
	private IEnumerator GameTimer()
	{
		yield return new WaitForSeconds(8.5f);

		while (timeInitial > 0)
		{
			//timeInitial -= Time.deltaTime;

			timeInitial -= 0.1f;
			currentTime = timeInitial;
			timeText.text = "" + Math.Round(currentTime, 1);
			yield return new WaitForSeconds(0.1f);
		}

		countdownText.text = "GAME OVER!";
	}
		
	
}
