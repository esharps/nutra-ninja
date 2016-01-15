using UnityEngine;
using System.Collections;
using Windows.Kinect;

/// <summary>
/// Increments or decrements the player's score based
/// on whether the player has sliced a healthy/unhealthy
/// GameObject
/// @author Emily Sharp
/// </summary>
public class CollisionManager : MonoBehaviour {

	public int goodFoodScoreScheme = 10;
	public int badFoodScoreScheme = -10;

	// Keep track of number of objects player has slashed
	// Accessible to other classes
	public static int objectsSlashed;

	void Start()
	{
		objectsSlashed = 0;
	}

	// Hadnl
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "GoodFood") 
		{
			objectsSlashed += 1;
			ScoreManager.currentScore += goodFoodScoreScheme;
			Destroy(col.gameObject);
		}
		if (col.gameObject.tag == "BadFood")
		{
			ScoreManager.currentScore += badFoodScoreScheme;
			Destroy(col.gameObject);
		}
	}
}
