using UnityEngine;
using System.Collections;

/// <summary>
/// Destroys the food GameObjects as soon as they are out of 
/// view in the Unity scene
/// @author Emily Sharp
/// </summary>
public class FoodDestroyer : MonoBehaviour {

	public float minBoundaryY = -20;
	public float penalty = 10;

	// Update is called once per frame
	void Update () {
	
		if (transform.position.y < minBoundaryY) {

			//Remove instantiation
			Destroy(gameObject);

			//If player fails to hit an item, they are penalized
			ScoreManager.currentScore -= penalty;
		}
	}
}
