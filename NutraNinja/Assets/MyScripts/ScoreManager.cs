using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Displays the player's current score via a UI Text object, as 
/// the currentScore static variable is updated by a call in the 
/// CollisionManager class
/// @author Emily Sharp
/// </summary>
public class ScoreManager : MonoBehaviour {

	public static int currentScore;
	Text scoreText;

	void Awake()
	{
		currentScore = 0;
		scoreText = GetComponent<Text>();
		scoreText.text = "" + currentScore;
	}
		
	// Update is called once per frame
	void Update()
	{
		scoreText.text = "" + currentScore;
	}

}
