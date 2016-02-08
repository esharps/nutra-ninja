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
    private Vector2 lastPosition;
    public GameObject splashIcon;

	// Keep track of number of objects player has slashed
	// Accessible to other classes
	public static int objectsSlashed;

	void Start()
	{
		objectsSlashed = 0;
	}

	// Handles player collision with GameObjects
	// TODO Object is replaced by a "splat" icon and fades out
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "GoodFood") 
		{
            lastPosition = transform.position;
			objectsSlashed += 1;
			ScoreManager.currentScore += goodFoodScoreScheme;
            
			Destroy(col.gameObject);
            GameObject splat = Instantiate(splashIcon, lastPosition, Quaternion.identity) as GameObject;

           // StartCoroutine(fadeOut(0.2f));
            //Destroy(splat);

		}
		if (col.gameObject.tag == "BadFood")
		{
			ScoreManager.currentScore += badFoodScoreScheme;
			Destroy(col.gameObject);
		}
	}

    IEnumerator fadeOut(float s)
    {
        yield return new WaitForSeconds(s);
    }
}
